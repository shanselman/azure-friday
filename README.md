# Azure Friday

The website for [azurefriday.com](https://azurefriday.com) — Scott Hanselman's weekly show about Azure.

## Architecture

```
┌─────────────────────────────┐     ┌──────────────────────────────┐     ┌──────────────────────┐
│  Microsoft Learn API        │     │  Azure Function              │     │  Azure Web App       │
│                             │     │  (azurefridayaggregator)     │     │  (this repo)         │
│  /api/hierarchy/shows/      │────►│                              │     │                      │
│    azure-friday/episodes    │     │  Runs every 6 hours          │     │  azurefriday.com     │
│  /api/video/public/v1/      │     │  Fetches all 500+ episodes   │     │  .NET 10 Razor Pages │
│    entries/batch            │     │  Generates JSON + RSS feeds  │     │                      │
└─────────────────────────────┘     │  Uploads to Blob Storage     │     │  Reads cached JSON   │
                                    └──────────┬───────────────────┘     │  from Blob Storage   │
                                               │                        │  via LazyCache (4hr)  │
                                    ┌──────────▼───────────────────┐     │                      │
                                    │  Azure Blob Storage          │────►│  /rss → redirect     │
                                    │  hanselstorage/output/       │     │  /rssaudio → redirect│
                                    │                              │     └──────────────────────┘
                                    │  azurefriday.json  (1.8 MB)  │
                                    │  azurefriday.rss   (1.1 MB)  │
                                    │  azurefridayaudio.rss (1.1MB)│
                                    └──────────────────────────────┘
```

### Two Repos

| Repo | Purpose | Deploys to |
|------|---------|------------|
| **[azure-friday](https://github.com/shanselman/azure-friday)** (this repo) | Web frontend — Razor Pages app that displays episodes | `its-azure-friday` Azure Web App |
| **[azurefridayaggregator](https://github.com/shanselman/azurefridayaggregator)** | Data pipeline — Azure Function that fetches episode data from Microsoft Learn API and generates JSON + RSS feeds | `AzureFridayDocstoJSON` Azure Function |

## Tech Stack

- **.NET 10** with minimal hosting (`Program.cs`, no Startup.cs)
- **Razor Pages** for the UI
- **Tailwind CSS** (build-time via CLI, no CDN) with vanilla JS for episode filtering
- **LazyCache** for 4-hour in-memory caching of episode data
- **Polly** for HTTP retry + circuit breaker on the API client
- **Application Insights** for monitoring
- **Bicep** for infrastructure-as-code (in `infra/`)
- **GitHub Actions** for CI/CD (build, test, deploy on push to `master`)

## Running Locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org) (for Tailwind CSS build)
- [Azure CLI](https://aka.ms/getazcli) (for deployment only)

### Quick Start

```powershell
# One-time setup (installs npm deps, builds Tailwind CSS, restores .NET packages, runs tests)
.\scripts\setup.ps1

# Run the site
dotnet run --project azure-friday.core
```

Or manually:

```bash
cd azure-friday.core
npm install              # Install Tailwind CSS
npm run build:css        # Build CSS (wwwroot/css/tailwind.css)
cd ..
dotnet run --project azure-friday.core
```

### CSS Development

When actively editing Tailwind classes in `.cshtml` or `.js` files, run the watcher in a second terminal:

```bash
cd azure-friday.core
npm run watch:css        # Rebuilds tailwind.css on every file change
```

> **Note:** `tailwind.css` is checked into git so `dotnet run` works immediately after clone.
> The watcher is only needed when you're changing Tailwind utility classes.
> On `dotnet publish`, CSS is always rebuilt fresh via the MSBuild target.

The app reads episode data from `https://hanselstorage.blob.core.windows.net/output/azurefriday.json` (configured in `appsettings.json`).

### Running Tests

```bash
dotnet test
```

23 integration and unit tests covering:
- Homepage, Privacy page, and 404 handling
- Episode ID redirects (`/?id=12` → `https://aka.ms/azfr/012`)
- Videos API endpoint (`/?handler=LoadVideos`) with cache-control headers
- Security headers (CSP, X-Frame-Options, HSTS, etc.)
- RSS feed redirects to Blob Storage
- Domain redirect (azurewebsites.net → azurefriday.com)
- Episode model serialization
- Cache purge functionality

## Deploying to Azure

### Option 1: Azure Developer CLI

```bash
azd up
```

### Option 2: GitHub Actions (automatic)

Push to `master` triggers the CI/CD pipeline which builds, tests, and deploys to the `its-azure-friday` Azure Web App.

### Required Secrets

- `itsazurefriday_1f35` — Azure Web App publish profile

## Key Endpoints

| Path | Behavior |
|------|----------|
| `/` | Homepage — displays episodes (loaded via JS from `/?handler=LoadVideos`) |
| `/?id=123` | Redirects to `https://aka.ms/azfr/123` |
| `/?handler=LoadVideos` | Returns episode JSON (4-hour cache) |
| `/rss` | 302 redirect → `hanselstorage.blob.core.windows.net/output/azurefriday.rss` |
| `/rssaudio` | 302 redirect → `hanselstorage.blob.core.windows.net/output/azurefridayaudio.rss` |
| `/?handler=PurgeCache` (POST) | Clears the in-memory episode cache |

## Project Structure

```
azure-friday/
├── azure-friday.core/           # Main web application
│   ├── Program.cs               # App startup (minimal hosting)
│   ├── Pages/                   # Razor Pages (Index, Privacy, Error, 404)
│   ├── Services/                # Data access layer
│   │   ├── IAzureFridayDB.cs    # Interface
│   │   ├── AzureFridayDB.cs     # LazyCache wrapper (4-hour TTL)
│   │   └── AzureFridayClient.cs # HTTP client for blob storage JSON
│   ├── wwwroot/                 # Static assets
│   │   ├── css/input.css        # Tailwind source + custom styles (GeoCities etc.)
│   │   ├── css/tailwind.css     # Built output (checked in, rebuilt on publish)
│   │   └── js/                  # site.js (app logic), theme-init.js (dark mode)
│   ├── tailwind.config.js       # Tailwind config (azure color palette)
│   └── package.json             # npm scripts: build:css, watch:css
├── azure-friday.tests/          # xUnit integration + unit tests
├── scripts/
│   └── setup.ps1                # One-time developer setup script
├── infra/                       # Bicep infrastructure templates
├── azure.yaml                   # Azure Developer CLI config
└── .github/workflows/           # GitHub Actions CI/CD
```