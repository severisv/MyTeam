


var layout = layout || {};

layout.setPageName = function (text) {
    $('.mt-page-header').find('h1').html(text);
};

layout.pushState = function (href, title) {
    history.pushState('', title, href);
    layout.setPageName(title);
};







