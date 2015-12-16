var mt = mt || {};

mt.deleteWithAjax = function(selector) {
    var element = $(selector);
    var href = element.data('href');
    $.post(href, function (result) {
        
        $('#' + element.data('parent')).fadeOut(300, function() { $(this).remove(); });
        $('#alerts').html(result);
        $('.alert').effect("highlight", { }, 500 );
    });
}

mt.alert = function (type, message) {
    this.clearAlerts();
    $('#' + type + " .alert-content").html(message);
    $('#' + type).removeClass("hidden");
    $('.alert').effect("highlight", {}, 500);
}

mt.clearAlerts = function () {
    $('#alerts').find('.alert').addClass('hidden');
}

mt.showElement = function(selector) {
    $(selector).show();
}
mt.hideElement = function(selector) {
    $(selector).hide();
}

