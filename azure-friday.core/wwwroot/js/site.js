"use strict";

// api url
const videosAPIUrl = `/?handler=loadvideos`;
// wait for body to load completely then make call to API for data
document.body.onload = fetchData(videosAPIUrl);
// get user's browser locale
let userLocale = navigator.language ? navigator.language : "en-US";
// will store the listjs object from the function below here once I initialize it after loading the videos
var listJsObj;

// this event runs when the clear icon in the searchbar is clicked
document.body.addEventListener("ionClear", handleSearchInput);

// back to top feature
// When the user scrolls down 50px from the top of the document, show the button
var ionContent = document.querySelector("ion-content");
ionContent.scrollEvents = true;
ionContent.addEventListener("ionScroll", scrollFunction);

// fetch data from API
function fetchData(url) {
  fetch(url)
    .then(response => response.json())
    .then(async data => {
      renderDataInView(data);
      dismissSkeleton();
    })
    .catch(async err => {
      dismissSkeleton();
      let errorMessage = document.getElementById("errorMessage");
      if (errorMessage && errorMessage.style) {
        errorMessage.style.display = "block";
      }
    });
}

function dismissSkeleton() {
  const skeletonEl = document.getElementById("skeleton");
  if (skeletonEl && skeletonEl.style) {
    skeletonEl.style.display = "none";
  }
}

// function to initialize listjs
function initListJS() {
  // listJs code
  let options = {
    valueNames: ["title", "author", "body"],
    page: 20,
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
  return new List("videos-row", options);
}
// function to render data in page body once it has been fetched successfully
function renderDataInView(data) {
  // get container element for videos
  let wrapper = document.getElementById("videos-list");
  // let bodyRegex = /<p>(.*)<\/p>/gmi;
  data.items.forEach(video => {
    wrapper.insertAdjacentHTML(
      "beforeend",
      `
    <ion-col size="12" size-md="5">
        <ion-card href="${video.itemLink}">
          <ion-img src="${video.largeThumbnail}" 
          alt="${video.title} thumbnail" class="lazy image">
          </ion-img>  
          <ion-card-header>
              <ion-card-subtitle>
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
              </ion-card-subtitle>
              <ion-card-title>${video.title}</ion-card-title>
          </ion-card-header>

          <ion-card-content>
            <div class="body">
                <span> <strong>Description</strong> </span>   ${video.body}
            </div>
            <div class="video-link">
                <ion-button expand="full" href="${
                  video.itemLink
                }" class="button" target="_blank" rel="noopener noreferrer">
                  View Video
                </ion-button>
            </div>
          </ion-card-content>
      </ion-card>
    </ion-col>   
    `
    );
  });
  // make call to initialize listjs.... data has to already be in DOM for listJs to work on it so...
  listJsObj = initListJS();
}

function handleSearchInput(event) {
  listJsObj.search();
}

function scrollFunction(event) {
  let scrollToTopDisplay = document.getElementById("scrollToTop");
  if (event.detail.scrollTop > 50) {
    scrollToTopDisplay.style.display = "block";
  } else {
    scrollToTopDisplay.style.display = "none";
  }
}

// When the user clicks on the button, scroll to the top of the document
function scrollToTop() {
  ionContent.scrollToTop(1000);
}
