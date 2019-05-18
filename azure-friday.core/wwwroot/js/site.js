// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

// api url 
let videosAPIUrl = `https://channel9.msdn.com/odata/Areas(guid'aeee37b6-ab0a-4c9f-8779-a2570148507b')/Entries`;
// wait for body to load completely then make call to API for data
document.body.onload = fetchData(videosAPIUrl);
// get user's browser locale
let userLocale = navigator.language ? navigator.language : 'en-US';


// instantiate lazy load library
var lazyLoadInstance = new LazyLoad({
    elements_selector: ".lazy"
});

// fetch data from API
function fetchData(url) {
    fetch(url)
        .then((response) => response.json())
        .then((data) => {
            renderDataInView(data);
            hideLoadingIndicator();
        })
        .catch((err) => {
            console.log(err);            
            hideLoadingIndicator();
            let errorMessage = document.getElementById('errorMessage');
            errorMessage.style.display = "block";
            errorMessage.innerHTML = "<p>Unexpected error occured, try reloading this page please</p>";
        })
}
// function to hide loader div
function hideLoadingIndicator() {
    let loader = document.getElementById('loading');
    loader.style.display = "none";
}

// function to initialize listjs
function initListJS() {
    // listJs code
    let options = {
        valueNames: ['title', 'body'],
        page: 10,
        pagination: true,
        fuzzySearch: {
            searchClass: "",
            location: 0,
            distance: 100,
            threshold: 0.4,
            multiSearch: true
        }
    };

    var videoList = new List('videos-row', options);
}
// function to render data in page body once it has been fetched successfully
function renderDataInView(data) {
    // get container element for videos
    let wrapper = document.getElementById('videos-list');
    data.value.forEach(video => {
        var videoElement = document.createElement("div");
        videoElement.setAttribute("class", "video");
        videoElement.innerHTML = `
        <img data-src="${video.LargeThumbnail}" alt="${video.Title}" class="lazy">
        <h5 class="title">${video.Title}</h5>
        <div class="meta">
            <span class="date">
                <strong>Date:</strong> ${new Date(video.PublishedDate).toLocaleString(userLocale, { timeZone: 'UTC' })}
            </span>
        </div>
        <div class="body">
            <strong>Description:</strong> ${video.NoHTMLBody}...
        </div>
        <a href="${video.Permalink}" class="button" target="_blank" rel="noopener noreferrer">View Video</a>
        `;
        // append video to container
        wrapper.append(videoElement);
    });
    // make call to initialize listjs.... data has to already be in DOM for listJs to work on it so...
    initListJS();
    // lazyLoad instance has to be updated each time the DOM is changed
    updateLazyLoadInstance();

}
// function to update instance 
function updateLazyLoadInstance() {
    if (lazyLoadInstance) {
        lazyLoadInstance.update();
    }
}


// back to top function 
// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function () { scrollFunction() };

function scrollFunction() {
    if (document.body.scrollTop > 50 || document.documentElement.scrollTop > 50) {
        document.getElementById("scrollToTop").style.display = "flex";
    } else {
        document.getElementById("scrollToTop").style.display = "none";
    }
}

// When the user clicks on the button, scroll to the top of the document
function scrollToTop() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
} 
