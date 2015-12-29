

function applySignupFunctions(scope) {
    var $scope = $(scope);
    $scope.find('#event-showAll').click(function (e) {
        e.preventDefault();
        var element = $(this);
        var parent = element.parent();
        parent.html('<i class="fa fa-spin fa-spinner"></i>');

        $.get(element.attr('href'), function (data) {
            parent.hide();
            parent.after(data);
        });
    });


    $scope.find('.event-addMessage').click(function () {
        var element = $(this);

        if (element.hasClass('active')) {
            element.removeClass('active');
            element.siblings('.event-message').hide();
        } else {
            element.addClass('active');
            element.siblings('.event-message').show();
        }
    });

    function postSignupMessage(element, message) {
        $.post(element.data('href'),
              {
                  message: message
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
    }

    var typingTimer;

    $scope.find('.event-message').on('input', (function () {
        var element = $(this);
        element.parent().find('i.fa-spinner').show();
        var message = element.val();
        clearTimeout(typingTimer);
        typingTimer = setTimeout(function () {
            postSignupMessage(element, message);
        }, 750);



    }));


}

applySignupFunctions('body');