var timestamp = new Date();
var start = timestamp.getMilliseconds();

var ANIMATION_DURATON = 300; 

$('input.datepicker').datepicker();
$('table.tablesorter').tablesorter();
$('a.mt-popover').popover({ trigger: "hover" });

applySlideDownMenuListeners();
applyConfirmDialogListeners();
applyActiveLinkSwapper();



window.onpopstate = function () {
    location.reload();
}


console.log("global.js: " + (new Date().getMilliseconds() - start) + "ms");


// Slide down
function applySlideDownMenuListeners() {
    var element = $('.slide-down-parent');
    var subMenu = $(element.data('submenu'));

    element.click(function () {

        if (element.data('isFocused') == true) {
            element.data('isFocused', false);
            element.blur();
        } else {
            element.data('isFocused', true);
        }
    });

    element.focusin(function () {
        subMenu.slideDown(ANIMATION_DURATON);

    });

    element.focusout(function () {
        subMenu.slideUp(ANIMATION_DURATON);
        element.data('isFocused', false);
        setTimeout(function() {

        }, ANIMATION_DURATON);


    });
}

// Confirm dialog
function applyConfirmDialogListeners() {
    $('a.confirm-dialog').click(function (e) {
        e.preventDefault();
        var element = $(this);
        var message = element.attr('data-message');

        bootbox.confirm(message, function (result) {
            if (result === true) {
                window.location = element.attr('href');
            }
        });

    });
}

// Active links
function applyActiveLinkSwapper() {
    $('ul.nav li').on('click', function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
    });
}