

$('.register-attendance-input').click(function () {
    var element = $(this);
    var eventId = element.data('eventId');
    var playerId = element.data('playerId');
    var value = element.is(":checked");
    element.parent().find('i.fa-spinner').show();
    $.post(Routes.CONFIRM_ATTENDANCE,
    {
        playerId: playerId,
        eventId: eventId,
        isSelected: value
    }).then(function (response) {
        if (response.Success) {
            var successLabel = element.parent().find('.label-success');
            element.parent().find('i.fa-spinner').hide();
            successLabel.show();
            successLabel.fadeOut(1500);
        } else {
            var failLabel = element.parent().find('.label-danger');
            element.parent().find('i.fa-spinner').hide();
            failLabel.show();
            failLabel.fadeOut(3000);
        }
    });
});


