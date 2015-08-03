var checkbox = checkbox || {};

checkbox.showHideAssociatedElement = function (element, associatedSelector, reverse) {
    var show = element.checked;
    if (reverse) {
        show = !element.checked;
    }

    if (show) {
        $(associatedSelector).show();
    } else {
        $(associatedSelector).hide();
    }
};




