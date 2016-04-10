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

ajax.applyLoadIcon = function ($scope) {
    $scope.find('a').each(function () {
        var element = $(this);
        if (element.data('ajax') == true) {
            console.log("ol000")
            var target = $(element.data('ajax-update'))
            element.on('click',function() {
                target.html('<i class="mt-loader fa fa-spin fa-spinner"></i>')
            })
        }
    });
}