"use strict";

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
