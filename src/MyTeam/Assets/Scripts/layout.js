


var layout = layout || {};

layout.setPageName = function (text) {
    $('.mt-page-header').find('h1').html(text);
};

layout.pushState = function (key, value, title) {
    var uri = window.location.href;
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        uri = uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        uri = uri + separator + key + "=" + value;
    }

    history.pushState('', title, uri);
    layout.setPageName(title);
};







