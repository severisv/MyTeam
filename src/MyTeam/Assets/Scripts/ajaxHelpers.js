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
        $.get(href).then(function(data) {
            element.html((data));
        })
    });
}
