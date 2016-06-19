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
};


ajax.applyLoadListener = function ($scope) {
    $scope.find('.ajax-load').each(function () {
        var element = $(this);
        var href = element.attr('href');
        element.html('<i class="mt-loader fa fa-spin fa-spinner"></i>');
        $.get(href).then(function (data) {
            element.html((data));
        });
    });
};


function formIsValid ($form) {
    var result = true;
    $form.find('input[data-ajax-update-requiredfield]')
        .each(function (i, element) {
            if (!element.value) result = false;
        });

    return result;
}

ajax.applyAjaxLinkListeners = function ($scope) {
    $scope.find('a.ajax-link').each(function () {
        var element = $(this);
        var target = $(element.data('ajax-update'));
        var href = element.attr('href');

        element.click(function (e) {
            e.preventDefault();
            target.addClass('ajax-replace');
            target.addClass('ajax-replace--hidden');

            setTimeout(function () {
                $.get(href, function (response) {
                    window.scrollTo(0, 0);
                    target.html(response);
                    target.removeClass('ajax-replace--hidden');
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
        var appendTarget = $(element.data('ajax-append'));
        var href = element.attr('action');

        var submitButton = element.find('input[type=submit], button');

        element.submit(function (e) {
            e.preventDefault();

            submitButton.addClass('isSubmitting');
            $.post(href, element.serialize(), function (response) {
                target.html(response);
                appendTarget.append(response);
                window.global.applyScopedJsComponents(target);
                element.find('input[type=text], textarea').val('');
                submitButton.removeClass('isSubmitting');
                if (formIsValid(element)) {
                    submitButton.prop('disabled', false);
                } else {
                    submitButton.prop('disabled', true);
                }
            });
        });

        element.find('input[data-ajax-update-requiredfield]').on('input', function () {
            if (formIsValid(element)) {
                submitButton.prop('disabled', false);
            } else {
                submitButton.prop('disabled', true);
            }
        });
    });
};

module.exports = ajax;
