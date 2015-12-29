

$('.register-attendance-input').click(function () {
    var element = $(this);
    var eventId = element.data('eventId');
    var playerId = element.data('playerId');
    var value = element.is(":checked");
    element.parent().find('i.fa-spinner').show();
    $.post(Routes.SELECT_PLAYER,
    {
        playerId: playerId,
        eventId: eventId,
        isSelected: value
    }).then(function (response) {
        if (response.Success) {
            var playerId = element.data('playerId');

            var squadCount = parseInt($('#squadCount').text());

            if (value == true) {
                var playerName = element.data('playerName');
                $('#squad').append('<li id="' + playerId + '"><i class="flaticon-soccer18"></i>&nbsp;' + playerName + '</li>');
                $('#squadCount').html(squadCount + 1);
            } else {
                $('#' + playerId).remove();
                $('#squadCount').html(squadCount -1);
            }


        } else {
            var failLabel = element.parent().find('.label-danger');
            element.parent().find('i.fa-spinner').hide();
            failLabel.show();
            failLabel.fadeOut(3000);
        }
    });
});


