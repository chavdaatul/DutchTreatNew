var x = 0;
var s = "";

//alert("Hello Atul");

//console.log("Hello Atul");

//var theForm = document.getElementById("theForm");
//theForm.hidden = true;

var theForm = $("#theForm");
theForm.hide();

var button = document.getElementById("buyButton");
button.addEventListener("click", function () {
    console.log("Buying Item");
});

var productInfo = $(".product-props li");
productInfo.on("click", function () {
    console.log("You clicked on" + $(this).text);
});

var $loginToggle = $("#loginToggle");
var $popupForm = $(".popup-form");

$loginToggle.on("click", function () {
    //$popupForm.toggle();
    //$popupForm.toggle(1000);
    //$popupForm.slideToggle(1000);
    $popupForm.fadeToggle(1000);
});

