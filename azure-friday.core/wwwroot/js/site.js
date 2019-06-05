// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

// api url
let videosAPIUrl = `/?handler=loadvideos`;
// wait for body to load completely then make call to API for data
document.body.onload = fetchData(videosAPIUrl);
// get user's browser locale
let userLocale = navigator.language ? navigator.language : "en-US";

// instantiate lazy load library
var lazyLoadInstance = new LazyLoad({
  elements_selector: ".lazy"
});

// fetch data from API
function fetchData(url) {
  fetch(url)
    .then(response => response.json())
    .then(data => {
      renderDataInView(data);
      hideLoadingIndicator();
    })
    .catch(err => {
      console.log(err);
      hideLoadingIndicator();
      let errorMessage = document.getElementById("errorMessage");
      errorMessage.style.display = "block";
      errorMessage.innerHTML =
        "<p>Unexpected error occured, try reloading this page please</p>";
    });
}
// function to hide loader div
function hideLoadingIndicator() {
  let loader = document.getElementById("loading");
  loader.style.display = "none";
}

// function to initialize listjs
function initListJS() {
  // listJs code
  let options = {
    valueNames: ["title", "author", "body"],
    page: 50,
    pagination: {
      innerWindow: 2,
      outerWindow: 1
    },
    fuzzySearch: {
      searchClass: "fuzzy-search",
      location: 0,
      distance: 100,
      threshold: 0.4,
      multiSearch: true
    }
  };

  var videoList = new List("videos-row", options);
}
// function to render data in page body once it has been fetched successfully
function renderDataInView(data) {
  // get container element for videos
  let wrapper = document.getElementById("videos-list");
  // let bodyRegex = /<p>(.*)<\/p>/gmi;
  data.items.forEach(video => {
    var videoElement = document.createElement("div");
    videoElement.setAttribute("class", "video");
    videoElement.innerHTML = `
        <img src="data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 512 288'%3E%3C/svg%3E"
          data-src="${video.largeThumbnail}" 
          alt="${ video.title} 
          thumbnail" class="lazy image">
        <h5 class="title">${video.title}</h5>
        <div class="author">
            <span> <strong>Authors:</strong> </span> ${video.authors}
        </div>
        <div class="meta">
            <span class="date">
                <span> <strong>Date:</strong> </span> ${new Date(
                  Number.parseInt(video.publishedDate.slice(6, 19), 10)
                ).toLocaleDateString(userLocale)}
            </span>
        </div>
        <div class="body">
            <span> <strong>Description</strong> </span>
                ${video.body}
        </div>
        <div class="video-link">
            <a href="${
              video.itemLink
            }" class="button" target="_blank" rel="noopener noreferrer">View Video</a>
        </div>
        `;
    // body code
    //       <div class="body">
    //     <span> <strong>Description:</strong> </span>
    //         ${video.body.match(/<p>(.*)<\/p>/gmi) != null ?
    //         video.body.match(/<p>(.*)<\/p>/gmi)[0].replace(/(<p>|<\/p>|<ul>(.*)<\/ul>|<a(.*)<\/a>)/gmi, ''): ''}

    // </div>

    // because I'm trying to normalize the body text in all individual videos
    // regex code
    // ${video.body.match(/<p>(.*)<\/p>/gmi) != null ?
    // video.body.match(/<p>(.*)<\/p>/gmi)[0].replace(/(<p>|<\/p>|<ul>(.*)<\/ul>)/gmi, '').substring(0, 200) : ''}...

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
    setTimeout(() => {
      lazyLoadInstance.update();
    }, 500);
  }
}

// back to top function
// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function() {
  scrollFunction();
};

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
