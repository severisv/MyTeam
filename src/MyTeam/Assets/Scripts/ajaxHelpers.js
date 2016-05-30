var ajax = ajax || {};


ajax.applyFormUpdateListener = function ($scope) {
    var typingTimer;
    var updateElements = $scope.find('.ajax-update');
    updateElements.on('input', function () {
        var element = $(this);
        var value = element.val();
        clearTimeout(typingTimer);
        typingTimer = setTimeout(function () {
            $.post(element.data('href'), { value: value });
        }, 500);
    });
    updateElements.on('blur', function () {
        var element = $(this);
        var value = element.val();
        clearTimeout(typingTimer);
        $.post(element.data('href'), { value: value });
    });
}


ajax.applyLoadListener = function($scope)
{
    $scope.find('.ajax-load').each(function() {
        var element = $(this);
        var href = element.attr('href');
        element.html('<i class="mt-loader fa fa-spin fa-spinner"></i>')
        $.get(href).then(function(data) {
            element.html((data));
        })
    });
}

ajax.applyAjaxLinkListeners = function ($scope) {
    $scope.find('a.ajax-link').each(function () {
        var element = $(this);
            var target = $(element.data('ajax-update'));
            var href = element.attr('href');

            element.click(function (e) {
                e.preventDefault();
                target.addClass("ajax-replace");
                target.addClass("ajax-replace--hidden");

                setTimeout(function () { 
                    $.get(href, function (response) {
                        window.scrollTo(0, 0);
                        target.html(response);
                        target.removeClass("ajax-replace--hidden");
                        window.global.applyScopedJsComponents(target);
                    });
                }, 50);
            });
    });

    $scope.find('a.ajax-post').each(function () {
        var element = $(this);
        var target = $(element.data('ajax-update'));
        var href = element.attr('href');
        var completeFunction = element.data('ajax-complete');

        element.click(function (e) {
            e.preventDefault();
            $.get(href, function (response) {
                target.html(response);
                window.global.applyScopedJsComponents(target);

                if (completeFunction) {
                    eval(completeFunction);
                }
                
            });
        });
    });

    $scope.find('form.ajax-form').each(function () {
        var element = $(this);
        var target = $(element.data('ajax-update'));
        var href = element.attr('action');

        element.submit(function (e) {
            e.preventDefault();
            $.post(href, element.serialize(), function (response) {
                target.html(response);
                window.global.applyScopedJsComponents(target);
            });
        });
    });
}

module.exports = ajax;