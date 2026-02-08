// Theme initialization - runs before body renders to prevent flash of unstyled content
(function() {
    if (localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
    if (localStorage.getItem('geocities') === 'true') {
        document.documentElement.classList.add('geocities');
    }
})();
