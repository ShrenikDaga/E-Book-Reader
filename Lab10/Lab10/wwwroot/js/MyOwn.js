

var myIndex = 0;
automatic();
//For the slider on the home/index
function automatic() {
    let i;
    let x = document.getElementsByClassName("slider-block");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    myIndex++;
    if (myIndex > x.length) { myIndex = 1 }
    x[myIndex - 1].style.display = "flex";
    setTimeout(automatic, 2500);
}

//For rendering epub into a div
function renderEpub(name) {
    var book = ePub("/File Storage/"+name);
    var rendition = book.renderTo("area", {
        flow:"paginated",
        height: 600,
        width: "60%"
    });

    var next = document.getElementById("next");

    next.addEventListener("click", function (e) {
        book.package.metadata.direction === "rtl" ? rendition.prev() : rendition.next();
        e.preventDefault();
    }, false);

    var prev = document.getElementById("prev");
    prev.addEventListener("click", function (e) {
        book.package.metadata.direction === "rtl" ? rendition.next() : rendition.prev();
        e.preventDefault();
    }, false);

    rendition.display();
    
}

function plus()      //function to increase size the iframe
{
    var x = document.getElementById("area").width;
    if(x<1100)
        document.getElementById("area").width = 1.2 * x;

    var y = document.getElementById("area").height;
    document.getElementById("area").height = 1.2 * y;
}

function minus() //function to reduce size of the iframe
{
    var x = document.getElementById("area").width;
    var y = document.getElementById("area").height;

    if(y<500)
        document.getElementById("area").width = x / 1.2;
    document.getElementById("area").height = y / 1.2;

}