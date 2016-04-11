var ajax = ajax || {};


ajax.applyFormUpdateListener = function ($scope) {
    var typingTimer;
    $scope.find('.ajax-update').on('input', function () {

        var element = $(this);
        var value = element.val();
        clearTimeout(typingTimer);
        typingTimer = setTimeout(function () {
            $.post(element.data('href'), { value: value });
        }, 500);
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
                        target.html(response);
                        target.removeClass("ajax-replace--hidden");
                        global.applyScopedJsComponents(target);
                    });
                }, 80);
            });
    });

    $scope.find('a.ajax-post').each(function () {
        var element = $(this);
        var target = $(element.data('ajax-update'));
        var href = element.attr('href');

        element.click(function (e) {
            e.preventDefault();
            $.get(href, function (response) {
                target.html(response);
                global.applyScopedJsComponents(target);
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
                global.applyScopedJsComponents(target);
            });
        });
    });
}