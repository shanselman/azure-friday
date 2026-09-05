# Azure Friday redesign plan

## Product goal

Turn Azure Friday from an episode index into the fastest, most inviting way to discover and watch practical Azure engineering conversations. Every visit should answer three questions quickly: **what should I watch now, why is it worth my time, and where can I keep watching?**

## What success looks like

- More homepage visitors start a video in their first session.
- More visitors watch a second episode rather than leaving after one click.
- Search and topic discovery help people find older evergreen episodes.
- YouTube, Microsoft Learn, and podcast destinations become intentional continuation paths instead of a row of miscellaneous links.
- The experience stays fast, accessible, crawlable, and useful without JavaScript.

## Audience and jobs to be done

| Audience | What they need | Best first action |
|---|---|---|
| Azure-curious developer | A quick, credible introduction without a large time commitment | Watch a short, approachable featured episode |
| Working Azure practitioner | Help with a current technology or architecture decision | Search or enter a topic collection |
| Returning viewer | The newest episode and a reason to continue watching | Watch latest, then receive a related recommendation |
| Podcast listener | A durable subscription path that fits an existing habit | Continue in Apple Podcasts, Spotify, Amazon, or RSS |
| Microsoft Learn visitor | A useful video companion to documentation and training | Open the relevant episode or curated topic page |

## Information architecture

1. **Editorial hero** — a clear promise, one primary action, and a secondary archive action.
2. **Featured/latest episode** — a large visual treatment that makes the newest episode feel like an event.
3. **Choose your path** — short routes for beginners, current Azure practitioners, and people looking for a specific topic.
4. **Browse the archive** — search, filters, and pagination for the complete catalog.
5. **Keep watching** — related episodes, curated playlists, and persistent subscription destinations.
6. **Subscribe intentionally** — explain the distinct value of video, audio, RSS, and Microsoft Learn instead of presenting undifferentiated platform buttons.

## Destination strategy

Outbound links should serve the viewer's intent rather than compete equally for attention:

- **Primary watch action:** use the canonical video destination selected for the show. If YouTube views are the primary growth metric, prefer the episode's YouTube URL and fall back to Microsoft Learn.
- **Learn more:** use the Microsoft Learn episode page for transcripts, supporting links, and related documentation.
- **Listen later:** group Spotify, Apple Podcasts, Amazon Music, and RSS under a clearly labeled audio action.
- **Subscribe:** link to the YouTube playlist/channel from persistent navigation and post-watch surfaces rather than making it compete with the featured episode.
- **Share:** generate canonical episode URLs and episode-specific social metadata so shared links communicate what the viewer will learn.

Before Phase 2 ships, confirm which platform owns the primary view metric and encode that choice as a single canonical-watch field in the aggregator rather than duplicating destination logic in the browser.

## Delivery phases

### Phase 1 — Make the homepage watch-first (this PR)

- Add a prominent latest-episode feature populated from the existing feed.
- Make the primary call to action unambiguous: **Watch the latest episode**.
- Add lightweight proof points and explain why the show is worth watching.
- Reframe the archive as a secondary discovery surface.
- Preserve existing links, filtering, pagination, dark mode, GeoCities mode, and security behavior.

### Phase 2 — Improve discovery data

- Enrich episode records with duration, presenters, topic tags, series/playlists, and a canonical watch destination.
- Add “Most watched,” “Recently updated,” “For beginners,” and topic rails.
- Add related-episode links on every episode card and a “watch next” module.
- Add server-rendered metadata and JSON-LD for the featured episode.

### Phase 3 — Build return visits and sharing

- Add share actions with useful, episode-specific Open Graph metadata.
- Add a lightweight “new episode” subscription prompt without interrupting playback intent.
- Add measurable events for feature CTA clicks, card starts, outbound destinations, search usage, and second-video starts.
- Test mobile-first layouts, thumbnail performance, and Core Web Vitals.

### Phase 4 — Connect the ecosystem

- Create curated landing pages for Azure fundamentals, AI, containers, data, security, and architecture.
- Make Microsoft Learn companion content visible when available.
- Give YouTube and podcast destinations dedicated, clearly labeled jobs: video, audio, and complete show archive.
- Experiment with embedded previews only where they improve completion without slowing the page.

## Detailed experience direction

### Homepage

- Keep the show promise concise and move the newest episode into the first meaningful content position.
- Use one dominant visual and one dominant action instead of giving six platforms equal visual weight.
- Follow the latest episode with three to five curated paths based on viewer intent, not internal product taxonomy.
- Preserve complete archive search below the editorial modules for long-tail discovery.
- Add a compact continuation module after each rail so viewers always have a sensible next click.

### Episode cards

- Show title, thumbnail, publication date, duration, topic, and presenter when reliable data exists.
- Make the entire card operable while retaining a visible action label for clarity.
- Prefer outcome-oriented summaries: what viewers will understand or be able to do after watching.
- Avoid loading every thumbnail eagerly; prioritize the featured image and lazy-load archive imagery.
- Reserve visual badges for useful distinctions such as “New,” “Beginner,” or a series name.

### Search and collections

- Keep free-text search immediate and forgiving.
- Add filter chips only after a stable topic taxonomy exists.
- Publish crawlable topic pages for high-intent subjects instead of relying exclusively on client-side filtering.
- Curate “Start here” collections manually at first; automate recommendations only when enough trustworthy metadata exists.
- Preserve query and filter state in the URL so results can be bookmarked and shared.

### Mobile

- Put the latest episode, title, and watch action above secondary platform choices.
- Use horizontal rails sparingly; never hide the complete archive behind gesture-only navigation.
- Keep touch targets at least 44px and avoid hover-dependent information.
- Test the first-load experience on a constrained connection, not only desktop broadband.

## Measurement plan

The redesign should be judged by viewing behavior rather than page cosmetics. Establish a baseline before broad changes and instrument only the events needed to answer product questions.

| Funnel stage | Metric | Event or signal |
|---|---|---|
| Arrival | Homepage engagement rate | meaningful scroll, search, or outbound episode click |
| First watch | Featured CTA click-through rate | featured episode click / homepage sessions |
| Discovery | Archive and collection usage | searches, filters, collection opens, zero-result searches |
| Continuation | Second-video intent | related or next-episode click after an episode click |
| Retention | Subscription intent | YouTube, podcast, and RSS subscription destination clicks |
| Quality | Friction and speed | Core Web Vitals, image failures, API errors, and empty search rate |

Segment results by device, referrer, new versus returning visitor, and destination. Do not treat raw outbound clicks as completed views; combine site events with destination analytics where available.

## Technical approach

- Continue using the cached episode JSON as the source of truth during the first phase.
- Extend the aggregator contract deliberately with optional fields for duration, presenters, topics, series, canonical watch URL, Learn URL, and audio URL.
- Server-render the featured episode and initial archive content when practical so search engines and no-script visitors receive useful content.
- Add `VideoObject`, `ItemList`, and breadcrumb structured data only from fields that are present and verified.
- Keep JavaScript progressive: filtering and pagination may enhance the page, but navigation and primary episode discovery should remain links.
- Centralize outbound URL selection and click instrumentation so cards, featured modules, and collections behave consistently.

## Design principles

- **One primary action per viewport.** Avoid making every destination look equally important.
- **Editorial before inventory.** Lead with a recommendation, then expose the catalog.
- **Progressive disclosure.** Show the useful summary first; keep the full archive available.
- **Trust the content.** Use real episode imagery and plain language instead of decorative UI.
- **Fast by default.** Keep the current cached JSON architecture and avoid shipping a video player or analytics-heavy framework prematurely.
- **Accessible and resilient.** Keyboard navigation, visible focus, meaningful alt text, reduced motion support, and a useful loading/error state remain requirements.
- **Measure outcomes, not ornament.** Every major module should have a viewer behavior hypothesis and a success measure.

## Accessibility and performance guardrails

- Meet WCAG 2.2 AA contrast and interaction requirements across light, dark, and high-contrast modes.
- Honor `prefers-reduced-motion`; thumbnails and controls must not require animation to communicate state.
- Maintain keyboard order, visible focus, descriptive link names, and meaningful image alternatives.
- Give the featured image explicit dimensions or aspect ratio to prevent layout shift.
- Lazy-load below-the-fold images, avoid autoplay, and defer third-party embeds until requested.
- Set performance budgets for mobile LCP, CLS, and JavaScript payload before introducing richer media.

## Experiment sequence

1. Measure the current archive-first baseline.
2. Ship the latest-episode feature and compare first-watch click-through.
3. Test a single “Start here” collection against the proof-point section.
4. Add related episodes and measure second-video intent.
5. Test the chosen primary destination while monitoring completed-view data from that platform.
6. Add subscription prompts only after the watch flow is working; avoid optimizing subscriptions at the expense of starts.

## Risks and mitigations

- **Unreliable metadata:** launch curated collections manually and validate aggregator fields before automating them.
- **Too many choices:** maintain one primary action and visually subordinate platform alternatives.
- **Slower pages:** avoid autoplay and third-party players; optimize image delivery before adding richer modules.
- **Archive regression:** keep search prominent, preserve deep links, and compare search usage and zero-result rates.
- **Measurement ambiguity:** define the canonical view metric and destination ownership before interpreting click-through as success.
- **Editorial maintenance:** make rails data-driven where possible and limit hand-curated modules to a sustainable number.

## Decisions to validate while iterating

- Whether the primary watch destination should be YouTube or Microsoft Learn for each episode.
- Which topic taxonomy is useful enough to justify maintaining it in the aggregator.
- Whether “most watched” data is available and trustworthy.
- Whether a newsletter, RSS, or platform subscription produces the best return-visit signal.
- Which homepage module improves starts without reducing archive search usage.
