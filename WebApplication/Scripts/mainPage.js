// onload : image setting
$(document).ready(() => {
    $.getJSON("/Sources/drink.json", data => {
        console.log("[" + data + "]");
    }).success(() => {
        console.log("success");
    }).fail(() => {
        console.log("failed");
    });
});

// about slides
var slideIndex = 1;
showDivs(slideIndex);

setInterval(() => {
    plusDivs(1);
}, 3000);

function plusDivs(n) {
    showDivs(slideIndex += n);
}

function showDivs(n) {
    var i;
    var imageList = document.getElementsByClassName("slideImageContainer");

    if (n > imageList.length) {
        slideIndex = 1
    }

    if (n < 1) {
        slideIndex = imageList.length
    }

    for (i = 0; i < imageList.length; i++) {
        imageList[i].style.display = "none";
    }

    imageList[slideIndex - 1].style.display = "block";
}