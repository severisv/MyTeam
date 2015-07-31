$('.datepicker').datepicker();
$('.mt-popover').popover({ trigger: "hover" });

window.onpopstate = function (event) {

    location.reload();
}
