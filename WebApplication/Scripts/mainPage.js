// about slides
var slideIndex = 1;

// onload : image setting
$(document).ready(() => {
    // get slider object
    let imageSlide = $(".slideContainer");

    $.getJSON("/Sources/drink.json", drinkList => {

        for (var i = 0; i < drinkList.length; i++)
        {
            let drinkName = drinkList[i].drinkName;

            // <img class="slideImageContainer w3-animate-opacity" src="@Url.Content("~/Sources/Images/mainPagePic_1.png")" style="width:100%">
            // 새로운 img 태그 생성
            let newImage = document.createElement("img");
            newImage.classList.add("slideImageContainer");
            newImage.classList.add("w3-animate-opacity");

            newImage.style.width = imageSlide.width() + "px";
            newImage.style.height = imageSlide.height() + "px";
            newImage.style.display = "none";

            // src append
            $.get(`/Sources/Images/${drinkName}.png`)
                .done(() => {
                    newImage.src = `/Sources/Images/${drinkName}.png`;
                })
                .fail(() => {
                    $.get(`/Sources/Images/${drinkName}.jpg`)
                        .done(() => {
                            newImage.src = `/Sources/Images/${drinkName}.jpg`;
                        })
                        .fail(() => {
                            newImage.src = '/Sources/Images/NoImage.png';
                        })
                })

            newImage.addEventListener("click", () => {
                window.location.href = `/Monitoring?target=${drinkName}`;
            });

            imageSlide.append(newImage);
        }
    }).then(() => {
        // 좌측 버튼
        //<button class="w3-button w3-black w3-display-left" onclick="plusDivs(-1)">&#10094;</button>
        let prevButton = document.createElement("button");
        prevButton.classList.add("w3-button");
        prevButton.classList.add("w3-black");
        prevButton.classList.add("w3-display-left");
        prevButton.innerHTML = "&#10094;";
        prevButton.addEventListener("click", () => { plusDivs(-1) });
        imageSlide.append(prevButton);

        // 우측 버튼
        //<button class="w3-button w3-black w3-display-right" onclick="plusDivs(1)">&#10095;</button>
        let nextButton = document.createElement("button");
        nextButton.classList.add("w3-button");
        nextButton.classList.add("w3-black");
        nextButton.classList.add("w3-display-right");
        nextButton.innerHTML = "&#10095;";
        nextButton.addEventListener("click", () => { plusDivs(1) });
        imageSlide.append(nextButton);

        showDivs(slideIndex);

        setInterval(() => {
            plusDivs(1);
        }, 2000);

        console.log("success");
    }).fail(() => {
        console.log("failed");
    });
});

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