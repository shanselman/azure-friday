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
- **Tailwind CSS** (CDN) with vanilla JS for episode filtering
- **LazyCache** for 4-hour in-memory caching of episode data
- **Polly** for HTTP retry + circuit breaker on the API client
- **Application Insights** for monitoring
- **Bicep** for infrastructure-as-code (in `infra/`)
- **GitHub Actions** for CI/CD (build, test, deploy on push to `master`)

## Running Locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Azure CLI](https://aka.ms/getazcli) (for deployment)
- [Azure Developer CLI](https://aka.ms/azd) (optional, for `azd up`)

### Quick Start

```bash
cd azure-friday.core
dotnet run
```

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
│   └── wwwroot/                 # Static assets (CSS, JS, images)
├── azure-friday.tests/          # xUnit integration + unit tests
├── infra/                       # Bicep infrastructure templates
├── azure.yaml                   # Azure Developer CLI config
└── .github/workflows/           # GitHub Actions CI/CD
```