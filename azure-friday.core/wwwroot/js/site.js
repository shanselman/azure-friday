"use strict";

// ============================================
// GeoCities Mode Toggle
// ============================================
const geocitiesToggle = document.getElementById('geocities-toggle');

function setGeoCitiesMode(enabled) {
    if (enabled) {
        document.documentElement.classList.add('geocities');
        localStorage.setItem('geocities', 'true');
        // Add some classic 90s flair
        addGeoCitiesExtras();
    } else {
        document.documentElement.classList.remove('geocities');
        localStorage.removeItem('geocities');
        removeGeoCitiesExtras();
    }
}

function addGeoCitiesExtras() {
    // Add flames bar at top
    if (!document.getElementById('geocities-flames-top')) {
        const header = document.querySelector('header');
        if (header) {
            const flames = document.createElement('div');
            flames.id = 'geocities-flames-top';
            flames.className = 'geocities-only geocities-flames';
            header.insertAdjacentElement('afterend', flames);
        }
    }

    // Add marquee to hero if not already there
    if (!document.getElementById('geocities-marquee')) {
        const heroSection = document.querySelector('section.bg-gradient-to-br');
        if (heroSection) {
            const marqueeDiv = document.createElement('div');
            marqueeDiv.id = 'geocities-marquee';
            marqueeDiv.className = 'geocities-only geocities-ants';
            marqueeDiv.innerHTML = `
                <marquee behavior="alternate" scrollamount="5" class="geocities-marquee geocities-sparkle">
                    ★☆★ WELCOME TO AZURE FRIDAY!!! ★☆★ BEST VIEWED WITH NETSCAPE NAVIGATOR 4.0 ★☆★ 
                    <span class="geocities-new">NEW!</span> 
                    YOU ARE VISITOR #<span class="geocities-hits">${Math.floor(Math.random() * 900000 + 100000).toLocaleString()}</span> ★☆★
                    <span class="geocities-hot">HOT!</span> ★☆★
                </marquee>
            `;
            heroSection.insertBefore(marqueeDiv, heroSection.firstChild);
        }
    }

    // Add under construction banner with flames
    if (!document.getElementById('geocities-construction-banner')) {
        const main = document.querySelector('main');
        if (main) {
            const banner = document.createElement('div');
            banner.id = 'geocities-construction-banner';
            banner.className = 'geocities-only';
            banner.innerHTML = `
                <div class="geocities-tape"></div>
                <div class="geocities-construction">
                    <img src="/imgs/construction-worker.gif" alt="" style="height:32px;vertical-align:middle" class="geocities-wobble">
                    <span class="geocities-blink">🚧</span> THIS PAGE IS UNDER CONSTRUCTION!!! <span class="geocities-blink">🚧</span>
                    <img src="/imgs/construction-worker.gif" alt="" style="height:32px;vertical-align:middle" class="geocities-wobble">
                </div>
                <div class="geocities-tape"></div>
            `;
            main.insertBefore(banner, main.firstChild);
        }
    }

    // Add circle ants around the hero title
    const heroTitle = document.querySelector('.bg-gradient-to-br h1');
    if (heroTitle && !heroTitle.querySelector('.geocities-circle-ants')) {
        heroTitle.classList.add('geocities-circle-container');
        const ants = document.createElement('div');
        ants.className = 'geocities-circle-ants geocities-only';
        heroTitle.appendChild(ants);
    }

    // Add guestbook and email links to footer
    if (!document.getElementById('geocities-footer-extras')) {
        const footer = document.querySelector('footer .flex');
        if (footer) {
            const extras = document.createElement('div');
            extras.id = 'geocities-footer-extras';
            extras.className = 'geocities-only text-center w-full mt-4';
            extras.innerHTML = `
                <div class="geocities-flames" style="height:30px;margin-bottom:10px"></div>
                <p class="geocities-neon">
                    <span class="geocities-spin">📧</span>
                    <a href="mailto:webmaster@azurefriday.com" class="geocities-guestbook">EMAIL THE WEBMASTER</a>
                    &nbsp;|&nbsp;
                    <a href="#" class="geocities-guestbook" onclick="alert('Thanks for signing my guestbook! This feature coming soon! (since 1998)'); return false;">📖 SIGN MY GUESTBOOK 📖</a>
                    &nbsp;|&nbsp;
                    <span class="geocities-new">NEW!</span> <a href="#" class="geocities-guestbook" onclick="alert('You have been added to my webring!'); return false;">🔗 JOIN MY WEBRING 🔗</a>
                </p>
                <p style="margin-top:8px">
                    <img src="https://web.archive.org/web/20091026213553/http://geocities.com/ResearchTriangle/Thinktank/4203/netscape.gif" alt="Netscape Now!" style="height:32px" onerror="this.alt='[Netscape Now!]'">
                    <img src="https://web.archive.org/web/20091027030818/http://www.geocities.com/TheTropics/Cabana/6780/ieani.gif" alt="IE" style="height:32px" onerror="this.style.display='none'">
                </p>
                <p class="geocities-blink" style="margin-top:8px;font-size:10px">
                    ♪ MIDI would be playing if this were really 1997 ♪
                </p>
            `;
            footer.appendChild(extras);
        }
    }

    // Play MIDI... just kidding, but show a console message
    console.log('%c♪ 🎵 [MIDI MUSIC WOULD BE PLAYING] 🎵 ♪', 'color: magenta; font-size: 20px; font-family: Comic Sans MS');
    console.log('%cWelcome to 1997! Best viewed at 800x600 resolution.', 'color: lime; background: navy; font-family: Comic Sans MS; padding: 10px;');
}

function removeGeoCitiesExtras() {
    // Remove added elements
    document.getElementById('geocities-marquee')?.remove();
    document.getElementById('geocities-construction-banner')?.remove();
    document.getElementById('geocities-footer-extras')?.remove();
}

// Initialize GeoCities mode from localStorage
if (localStorage.getItem('geocities') === 'true') {
    document.documentElement.classList.add('geocities');
    // Defer extras until DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', addGeoCitiesExtras);
    } else {
        addGeoCitiesExtras();
    }
}

geocitiesToggle?.addEventListener('click', () => {
    const isGeoCities = document.documentElement.classList.contains('geocities');
    setGeoCitiesMode(!isGeoCities);
});

// ============================================
// Theme Toggle
// ============================================
const themeToggle = document.getElementById('theme-toggle');

function setTheme(theme) {
    if (theme === 'dark') {
        document.documentElement.classList.add('dark');
        localStorage.theme = 'dark';
    } else {
        document.documentElement.classList.remove('dark');
        localStorage.theme = 'light';
    }
}

themeToggle?.addEventListener('click', () => {
    const isDark = document.documentElement.classList.contains('dark');
    setTheme(isDark ? 'light' : 'dark');
});

// ============================================
// Scroll to Top Button
// ============================================
const scrollToTopBtn = document.getElementById('scroll-to-top');

window.addEventListener('scroll', () => {
    if (window.scrollY > 300) {
        scrollToTopBtn?.classList.remove('opacity-0', 'invisible');
        scrollToTopBtn?.classList.add('opacity-100', 'visible');
    } else {
        scrollToTopBtn?.classList.add('opacity-0', 'invisible');
        scrollToTopBtn?.classList.remove('opacity-100', 'visible');
    }
});

scrollToTopBtn?.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
});

// ============================================
// Video Data & Filtering
// ============================================
const videosAPIUrl = '/?handler=loadvideos';
let allVideos = [];
let filteredVideos = [];
let currentPage = 1;
const videosPerPage = 20;
const userLocale = navigator.language || 'en-US';

// DOM Elements
const videosGrid = document.getElementById('videos-grid');
const skeletonGrid = document.getElementById('skeleton-grid');
const searchInput = document.getElementById('search-input');
const resultsCount = document.getElementById('results-count');
const errorMessage = document.getElementById('error-message');
const pagination = document.getElementById('pagination');

// Fetch videos on page load
document.addEventListener('DOMContentLoaded', () => {
    fetchVideos();
});

async function fetchVideos() {
    try {
        const response = await fetch(videosAPIUrl);
        if (!response.ok) throw new Error('Failed to fetch videos');
        
        allVideos = await response.json();
        filteredVideos = [...allVideos];
        
        renderVideos();
        showVideosGrid();
    } catch (err) {
        console.error('Error fetching videos:', err);
        showError();
    }
}

function showVideosGrid() {
    skeletonGrid?.classList.add('hidden');
    videosGrid?.classList.remove('hidden');
    pagination?.classList.remove('hidden');
}

function showError() {
    skeletonGrid?.classList.add('hidden');
    errorMessage?.classList.remove('hidden');
}

// ============================================
// Render Videos
// ============================================
function renderVideos() {
    if (!videosGrid) return;
    
    // Calculate pagination
    const startIndex = (currentPage - 1) * videosPerPage;
    const endIndex = startIndex + videosPerPage;
    const paginatedVideos = filteredVideos.slice(startIndex, endIndex);
    
    // Update results count
    if (resultsCount) {
        const total = filteredVideos.length;
        const showing = Math.min(endIndex, total);
        resultsCount.textContent = `Showing ${startIndex + 1}-${showing} of ${total} videos`;
    }
    
    // Render video cards
    videosGrid.innerHTML = paginatedVideos.map(video => createVideoCard(video)).join('');
    
    // Render pagination
    renderPagination();
}

function createVideoCard(video) {
    const date = new Date(video.uploadDate).toLocaleDateString(userLocale, {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
    
    // Strip HTML from description for plain text preview
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = video.descriptionAsHtml || '';
    const plainDescription = tempDiv.textContent || tempDiv.innerText || '';
    const truncatedDescription = plainDescription.length > 150 
        ? plainDescription.substring(0, 150) + '...' 
        : plainDescription;
    
    return `
        <a href="${video.url}" target="_blank" rel="noopener noreferrer" 
           class="group block bg-white dark:bg-gray-800 rounded-xl shadow-sm hover:shadow-lg transition-all duration-300 overflow-hidden border border-gray-200 dark:border-gray-700 hover:border-azure-300 dark:hover:border-azure-600">
            <div class="relative aspect-video overflow-hidden">
                <img src="${video.thumbnailUrl}" 
                     alt="${video.title}" 
                     class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                     loading="lazy">
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity">
                    <div class="absolute bottom-3 left-3 right-3">
                        <span class="inline-flex items-center gap-1 text-white text-sm font-medium">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd"/>
                            </svg>
                            Watch Now
                        </span>
                    </div>
                </div>
            </div>
            <div class="p-4">
                <p class="text-xs text-gray-500 dark:text-gray-400 mb-1">${date}</p>
                <h3 class="font-semibold text-gray-900 dark:text-gray-100 line-clamp-2 group-hover:text-azure-600 dark:group-hover:text-azure-400 transition-colors">
                    ${video.title}
                </h3>
                <p class="mt-2 text-sm text-gray-600 dark:text-gray-400 line-clamp-3">
                    ${truncatedDescription}
                </p>
            </div>
        </a>
    `;
}

// ============================================
// Pagination
// ============================================
function renderPagination() {
    if (!pagination) return;
    
    const totalPages = Math.ceil(filteredVideos.length / videosPerPage);
    
    if (totalPages <= 1) {
        pagination.classList.add('hidden');
        return;
    }
    
    pagination.classList.remove('hidden');
    
    let html = '';
    
    // Previous button
    html += `
        <button onclick="goToPage(${currentPage - 1})" 
                ${currentPage === 1 ? 'disabled' : ''} 
                class="px-3 py-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/>
            </svg>
        </button>
    `;
    
    // Page numbers
    const pages = getPaginationRange(currentPage, totalPages);
    pages.forEach(page => {
        if (page === '...') {
            html += `<span class="px-3 py-2 text-gray-500 dark:text-gray-400">...</span>`;
        } else {
            const isActive = page === currentPage;
            html += `
                <button onclick="goToPage(${page})" 
                        class="px-4 py-2 rounded-lg border ${isActive 
                            ? 'border-azure-500 bg-azure-500 text-white' 
                            : 'border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'} transition-colors">
                    ${page}
                </button>
            `;
        }
    });
    
    // Next button
    html += `
        <button onclick="goToPage(${currentPage + 1})" 
                ${currentPage === totalPages ? 'disabled' : ''} 
                class="px-3 py-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
            </svg>
        </button>
    `;
    
    pagination.innerHTML = html;
}

function getPaginationRange(current, total) {
    const delta = 2;
    const range = [];
    const rangeWithDots = [];
    let l;
    
    for (let i = 1; i <= total; i++) {
        if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
            range.push(i);
        }
    }
    
    for (let i of range) {
        if (l) {
            if (i - l === 2) {
                rangeWithDots.push(l + 1);
            } else if (i - l !== 1) {
                rangeWithDots.push('...');
            }
        }
        rangeWithDots.push(i);
        l = i;
    }
    
    return rangeWithDots;
}

// Global function for pagination buttons
window.goToPage = function(page) {
    const totalPages = Math.ceil(filteredVideos.length / videosPerPage);
    if (page < 1 || page > totalPages) return;
    
    currentPage = page;
    renderVideos();
    
    // Scroll to videos section
    document.getElementById('videos-section')?.scrollIntoView({ behavior: 'smooth' });
};

// ============================================
// Search / Filter
// ============================================
let searchTimeout;

searchInput?.addEventListener('input', (e) => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        filterVideos(e.target.value);
    }, 300); // 300ms debounce
});

function filterVideos(query) {
    const searchTerms = query.toLowerCase().trim().split(/\s+/).filter(Boolean);
    
    if (searchTerms.length === 0) {
        filteredVideos = [...allVideos];
    } else {
        filteredVideos = allVideos.filter(video => {
            const searchableText = [
                video.title || '',
                video.descriptionAsHtml || '',
                video.description || ''
            ].join(' ').toLowerCase();
            
            return searchTerms.every(term => searchableText.includes(term));
        });
    }
    
    currentPage = 1;
    renderVideos();
}
