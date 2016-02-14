var ANIMATION_DURATON = 300;
var global = global || {};

global.applyScopedJsComponents = function (selector) {
    var $scope = $(selector);
    $scope.find('input.datepicker').datepicker();
    $scope.find('table.tablesorter').tablesorter();
    $scope.find('a.mt-popover').popover({ trigger: "hover" });
    applyConfirmDialogListeners($scope);
    applyActiveLinkSwapper($scope);
    applyAjaxLinkActions($scope);
    applySelectLinkListeners($scope);
    applyMtAnchorListeners($scope);
    ajax.applyFormUpdateListener($scope);
}

global.applyJsComponents = function() {
    var timestamp = new Date();
    var start = timestamp.getMilliseconds();

    this.applyScopedJsComponents($(document));
    applySlideDownMenuListeners();
    
    window.onpopstate = function () {
        //location.reload();
    }
    
    console.log("global.js: " + (new Date().getMilliseconds() - start) + "ms");
}


global.applyJsComponents();



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
function applyConfirmDialogListeners($scope) {
    $scope.find('a.confirm-dialog').click(function (e) {
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
function applyActiveLinkSwapper($scope) {
    $scope.find('ul.nav li').on('click', function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
    });


}// Active links
function applyAjaxLinkActions($scope) {
    $scope.find('a[mt-pushstate]').on('click', function () {
        var $el = $(this);
        if ($el.attr('mt-pushstate')) {
            layout.pushState($el.attr('href'), $el.attr('mt-pushstate'));
        }
    });
}

function applySelectLinkListeners($scope) {
    $scope.find('.linkSelect').on('change', function () {
        var url = $(this).val();
        window.location = url;
    });
}

function applyMtAnchorListeners($scope) {
    $scope.find('.mt-anchor').on('click', function () {
        var url = $(this).data('href');
        window.location = url;
    });
}
