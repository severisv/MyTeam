var timestamp = new Date();
var start = timestamp.getMilliseconds();
$('input.datepicker').datepicker();
$('table.tablesorter').tablesorter();
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


console.log(new Date().getMilliseconds() - start);