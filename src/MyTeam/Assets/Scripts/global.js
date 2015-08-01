$('input.datepicker').datepicker();
$('a.mt-popover').popover({ trigger: "hover" });



window.onpopstate = function (event) {

    location.reload();
}


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