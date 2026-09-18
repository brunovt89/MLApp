self.addEventListener('install', (e) => {
  self.skipWaiting();
});

self.addEventListener('fetch', (e) => {
  // Passthrough directo a la red para datos clínicos en tiempo real
  e.respondWith(fetch(e.request).catch(() => caches.match(e.request)));
});
