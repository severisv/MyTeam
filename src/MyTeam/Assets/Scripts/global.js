$('input.datepicker').datepicker();
$('a.mt-popover').popover({ trigger: "hover" });
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


window.onpopstate = function () {
    location.reload();
}


