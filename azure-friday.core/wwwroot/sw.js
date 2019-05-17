//Install stage sets up the offline page in the cache and opens a new cache
self.addEventListener('install', function (event) {
    event.waitUntil(preLoad());
});


// assets to cache
var offlineCache = [
    '/css/**',
    '/images/**',
    '/js/**',
    '/lib/**',
    '/favicon.ico',
    '/offline'
];


var preLoad = async function () {
    const cache = await caches.open('azure-friday');
    // assets to cache on installation
    return cache.addAll(offlineCache);
}

// on page requests
self.addEventListener('fetch', function (event) {
    event.respondWith(checkResponse(event.request).catch(function () {
        return returnFromCache(event.request)
    }
    ));
    event.waitUntil(addToCache(event.request));
});

var checkResponse = function (request) {
    return new Promise(function (fulfill, reject) {
        fetch(request).then(function (response) {
            if (response.status !== 404) {
                fulfill(response)
            } else {
                reject()
            }
        }, reject)
    });
};

// add request to successful requests to cache
var addToCache = async function (request) {
    const cache = await caches.open('azure-friday');
    const response = await fetch(request);
    return cache.put(request, response);
};

var returnFromCache = async function (request) {
    const cache = await caches.open('azure-friday');
    const matching = await cache.match(request);
    if (!matching || matching.status == 404) {
        // return offline page if user is offline, request is not in cache
        return cache.match('/offline');
    }
    else {
        // if request is in cache, serve it
        return matching;
    }
};