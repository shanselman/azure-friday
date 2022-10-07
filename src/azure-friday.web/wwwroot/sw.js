"use strict";
//Install stage sets up the offline page in the cache and opens a new cache
self.addEventListener("install", async function(event) {
  try {
    event.waitUntil(await preLoad());
  } catch (error) {}
});

// assets to cache
var offlineCache = [
  "/css/site.css",
  "/imgs/**",
  "/js/listjs.js",
  "/js/site.js",
  "/lib/**",
  "/favicon.ico",
  "/offline",
  "https://cdn.jsdelivr.net/npm/@@ionic/core/css/ionic.bundle.css",
  "https://cdn.jsdelivr.net/npm/@@ionic/core/dist/ionic/ionic.esm.js",
  "https://cdn.jsdelivr.net/npm/@@ionic/core/dist/ionic/ionic.js"
];

async function preLoad() {
  try {
    const cache = await caches.open("azure-friday");
    // assets to cache on installation
    return await cache.addAll(offlineCache);
  } catch (error) {}
}

// on page requests
self.addEventListener("fetch", function(event) {
  event.respondWith(
    checkResponse(event.request).catch(async function() {
      try {
        return await returnFromCache(event.request);
      } catch (error) {}
    })
  );
  event.waitUntil(addToCache(event.request));
});

function checkResponse(request) {
  return new Promise(function(resolve, reject) {
    fetch(request).then(function(response) {
      if (response.status !== 404) {
        resolve(response);
      } else {
        reject();
      }
    }, reject);
  });
}

// add request to successful requests to cache
async function addToCache(request) {
  try {
    const cache = await caches.open("azure-friday");
    const response = await fetch(request);
    return await cache.put(request, response);
  } catch (error) {}
}

async function returnFromCache(request) {
  try {
    const cache = await caches.open("azure-friday");
    const matching = await cache.match(request);
    if (!matching || matching.status == 404) {
      // return offline page if user is offline, request is not in cache
      return await cache.match("/offline");
    } else {
      // if request is in cache, serve it
      return matching;
    }
  } catch (error) {}
}
