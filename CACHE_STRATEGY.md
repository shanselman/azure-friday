# RSS Feed Cache Strategy

## Overview
This document describes the caching strategy implemented for Azure Friday's RSS feeds.

## Implementation

### Cache-Control Headers
RSS feed endpoints (`/rss` and `/rssaudio`) are configured with the following cache headers:

```
Cache-Control: public, max-age=300, must-revalidate
Vary: Accept-Encoding
```

### Header Explanation

- **`public`**: Allows the response to be cached by any cache (CDNs, proxies, browsers)
- **`max-age=300`**: Marks the feed as "fresh" for 5 minutes (300 seconds)
- **`must-revalidate`**: Instructs caches to revalidate with the origin server once the feed becomes stale
- **`Vary: Accept-Encoding`**: Ensures proper handling of compressed content

### Why 5 Minutes?

The 5-minute TTL (Time To Live) balances several factors:

1. **Freshness**: Ensures subscribers receive new episodes within a reasonable timeframe
2. **Server Load**: Reduces unnecessary requests to the origin server
3. **Bandwidth**: Minimizes bandwidth usage while maintaining good user experience
4. **Standard Practice**: Aligns with RSS feed caching best practices

### Technical Details

The cache headers are applied via middleware in `Startup.cs` before the URL rewrite/redirect occurs. This ensures:

- Headers are set before the redirect to blob storage
- The Cache-Control policy is enforced at the application level
- Future changes to the RSS feed URLs won't bypass the cache policy

### RSS Feed Endpoints

- `/rss` - Main Azure Friday video RSS feed
- `/rssaudio` - Audio-only RSS feed

Both endpoints redirect to Azure Blob Storage:
- `https://hanselstorage.blob.core.windows.net/output/azurefriday.rss`
- `https://hanselstorage.blob.core.windows.net/output/azurefridayaudio.rss`

## Best Practices Followed

This implementation follows industry best practices for RSS feed caching:

1. ✅ Public caching enabled for non-personalized content
2. ✅ Reasonable TTL set (5 minutes)
3. ✅ Revalidation required after expiration
4. ✅ Vary header for compression support
5. ✅ Applied before redirects for proper enforcement

## Future Enhancements

Potential improvements could include:

- **ETag support**: Add entity tags for conditional requests (requires blob storage configuration)
- **Last-Modified headers**: Support `If-Modified-Since` headers (requires blob storage metadata)
- **Dynamic TTL**: Adjust cache duration based on publishing schedule
- **Compression**: Ensure gzip compression is enabled (typically handled by Azure infrastructure)

## References

- [Best practices for syndication feed caching](https://www.ctrl.blog/entry/feed-caching.html)
- [Cache-Control MDN Documentation](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control)
- [HTTP Caching Headers Guide](https://accreditly.io/articles/the-practical-guide-to-http-caching-headers-cache-control-etag-and-304s)
