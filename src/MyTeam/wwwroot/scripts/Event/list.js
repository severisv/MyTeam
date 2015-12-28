

$('#event-showAll').click(function (e) {
    e.preventDefault();
    var element = $(this);
    var parent = element.parent();
    parent.html('<i class="fa fa-spin fa-spinner"></i>');

    $.get(element.attr('href'), function (data) {
        parent.hide();
        parent.after(data);
    });
});


