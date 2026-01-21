# Prompt for Azure Function - RSS Feed Cache-Control Headers

## Task
Update the Azure Function that uploads RSS feeds to Azure Blob Storage to set proper Cache-Control headers on the uploaded blobs.

## Background
The Azure Friday RSS feeds (`azurefriday.rss` and `azurefridayaudio.rss`) are uploaded to Azure Blob Storage at `hanselstorage.blob.core.windows.net/output/`. Currently, these blobs don't have Cache-Control headers set, which can lead to indefinite caching by CDNs and clients, preventing subscribers from receiving fresh content.

## Requirements

### Cache-Control Policy
Set the following Cache-Control header on RSS feed blobs when uploading:
```
public, max-age=300, must-revalidate
```

**Why this policy?**
- `public` - Allows CDNs and proxies to cache the feed
- `max-age=300` - Caches for 5 minutes (balances freshness with performance)
- `must-revalidate` - Forces revalidation after expiration

### Implementation

When uploading RSS feed files (`.rss` extension or specifically `azurefriday.rss` and `azurefridayaudio.rss`), set the Cache-Control header.

**Example for .NET/C# Azure Function using Azure.Storage.Blobs SDK:**

```csharp
// After uploading the blob, set HTTP headers
var blobClient = containerClient.GetBlobClient("azurefriday.rss");

// Upload the content
await blobClient.UploadAsync(stream, overwrite: true);

// Set Cache-Control header
await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
{
    CacheControl = "public, max-age=300, must-revalidate",
    ContentType = "application/rss+xml" // Ensure correct content type
});
```

**Alternative - Set during upload (more efficient):**
```csharp
var uploadOptions = new BlobUploadOptions
{
    HttpHeaders = new BlobHttpHeaders
    {
        CacheControl = "public, max-age=300, must-revalidate",
        ContentType = "application/rss+xml"
    }
};

await blobClient.UploadAsync(stream, uploadOptions);
```

### Validation
After making changes:
1. Verify the RSS feeds upload successfully
2. Check the blob properties in Azure Portal or Storage Explorer to confirm Cache-Control header is set
3. Test by fetching the RSS feed URL and inspecting response headers

### Additional Considerations
- Apply this only to RSS feed files (`.rss` extension)
- Ensure this doesn't break any existing upload logic
- Consider adding logging to confirm headers are being set
- If there are other XML feeds, they may benefit from the same caching policy

## Expected Outcome
When the RSS feeds are uploaded to blob storage, they should have the `Cache-Control: public, max-age=300, must-revalidate` header set, which will be respected by CDNs and clients fetching the feed.

## References
- Azure Blob Storage Cache-Control: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-properties-metadata
- RSS Feed Caching Best Practices: https://www.ctrl.blog/entry/feed-caching.html
