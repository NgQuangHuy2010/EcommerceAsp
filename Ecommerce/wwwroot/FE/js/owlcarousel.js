document.addEventListener("DOMContentLoaded", function(){
    // Owl Carousel cho carousel chính
    $(".owl-carousel.banner-carousel").owlCarousel({
        items: 1,
        loop: true,
        dots: false,
        autoplayHoverPause: true,
        margin: 10,
        autoplay: true,
        autoplayTimeout: 3000,
        responsive: {
            678: {
                items: 1,

            },
            1000: {
                items: 1,
            }
        }
    });

    // Owl Carousel cho carousel khác
    $(".owl-carousel.other-carousel").owlCarousel({
        margin: 10,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1,
                nav: false,
            },
            1000: {
                items: 5, // Đổi số items từ 1 thành 5 cho carousel khác ở mốc 1000 pixels
                nav: false,
            },
        },
    });
});



AutoHeight.Defaults = {
    autoHeight: false,
    autoHeightClass: 'owl-height'
};


