# Azure Friday redesign plan

## Product goal

Turn Azure Friday from an episode index into the fastest, most inviting way to discover and watch practical Azure engineering conversations. Every visit should answer three questions quickly: **what should I watch now, why is it worth my time, and where can I keep watching?**

## What success looks like

- More homepage visitors start a video in their first session.
- More visitors watch a second episode rather than leaving after one click.
- Search and topic discovery help people find older evergreen episodes.
- YouTube, Microsoft Learn, and podcast destinations become intentional continuation paths instead of a row of miscellaneous links.
- The experience stays fast, accessible, crawlable, and useful without JavaScript.

## Information architecture

1. **Editorial hero** — a clear promise, one primary action, and a secondary archive action.
2. **Featured/latest episode** — a large visual treatment that makes the newest episode feel like an event.
3. **Choose your path** — short routes for beginners, current Azure practitioners, and people looking for a specific topic.
4. **Browse the archive** — search, filters, and pagination for the complete catalog.
5. **Keep watching** — related episodes, curated playlists, and persistent subscription destinations.

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

## Design principles

- **One primary action per viewport.** Avoid making every destination look equally important.
- **Editorial before inventory.** Lead with a recommendation, then expose the catalog.
- **Progressive disclosure.** Show the useful summary first; keep the full archive available.
- **Trust the content.** Use real episode imagery and plain language instead of decorative UI.
- **Fast by default.** Keep the current cached JSON architecture and avoid shipping a video player or analytics-heavy framework prematurely.
- **Accessible and resilient.** Keyboard navigation, visible focus, meaningful alt text, reduced motion support, and a useful loading/error state remain requirements.

## Decisions to validate while iterating

- Whether the primary watch destination should be YouTube or Microsoft Learn for each episode.
- Which topic taxonomy is useful enough to justify maintaining it in the aggregator.
- Whether “most watched” data is available and trustworthy.
- Whether a newsletter, RSS, or platform subscription produces the best return-visit signal.
- Which homepage module improves starts without reducing archive search usage.
