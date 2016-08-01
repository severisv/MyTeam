

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
            if (response.success) {
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


$('#publishButton').click(function () {
    var element = $(this);
    var href = element.data('href');
    element.find('span').hide();
    element.find('i').show();
    $.post(href)
        .then(function (response) {
        if (response.success) {
            element.parent().html('<div class="disabled btn btn-success btn-lg"><i class="fa fa-check-circle"></i> Publisert</div>');

        } else {
            var failLabel = element.parent().find('.label-danger');
            element.find('span').show();
            element.find('i').hide();
            failLabel.show();
            failLabel.fadeOut(3000);
        }
    });
});


function postPublishMessage(element, message) {
        $.post(element.data('href'),
              {
                  description: message
              }).then(function (response) {
                  if (response.success) {
                      var successLabel = element.parent().find('.label-success');
                      successLabel.show();
                      successLabel.fadeOut(1500);
                  } else {
                      var failLabel = element.parent().find('.label-danger');
                      failLabel.show();
                      failLabel.fadeOut(3000);
                  }
              });

}

var typingTimer;

$('#publishMessage').on('input', (function () {
    var element = $(this);
    var message = element.val();
    clearTimeout(typingTimer);
    typingTimer = setTimeout(function () {
        postPublishMessage(element, message);
    }, 750);



}));


$.getJSON(Routes.GET_ATTENDANCE, function (data) {
    var players = data.data;
    for (var i in players) {
        var player = players[i];
        $('#playerAttendance-' + player.playerId).html(player.attendance+"%");
    }
});