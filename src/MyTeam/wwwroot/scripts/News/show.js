
var commentsContainer = $('#article-comments');
var commentUrl = commentsContainer.data('getCommentsUrl');
$.get(commentUrl, function (data) {
    commentsContainer.html(data);
    applyCommentFormSubmitListener();
    setFacebookNames($('body'));
});

function applyCommentFormSubmitListener () {
    $('#article-commentForm form').submit(function (e) {
        e.preventDefault();
        var form = $(this);
        var button = form.find('button');
        var textarea = form.find('textarea');
        var url = form.attr('action');

        var validationMessage = form.find('#comment-validation-text');
        validationMessage.html('');

        if (textarea.val().length < 4) {
            validationMessage.html('Kommentaren må være minst 4 tegn');
        }
        else if (form.find('.comment-nameInput').length > 0 && !form.find('.comment-nameInput').val()) {
            validationMessage.html('Navn må fylles ut');
        }
        else {
            form.find('.submitText').hide();
            button.attr('disabled', 'disabled');
            form.find('button').addClass('isSubmitting');

            $.post(url, form.serialize()).done(function (response) {
                var commentCount = $('#comments-count').html();
                $('#comments-count').html(1 + parseInt(commentCount));
                form.find('button').removeClass('isSubmitting');
                form.find('textarea').val('');
                form.find('.submitText').show();
                button.attr('disabled', false);
                $('#article-commentswrapper').append(response);
                setFacebookNames($('.comment-contentContainer:last-of-type'));

             });
        }
    });
}

function setFacebookNames ($scope) {
    if (!window.mt_fb.isLoaded) {
        setTimeout(function () {
            setFacebookNames($scope);
        }, 10);
    } else if (!window.mt_fb.accessToken && !window.mt_fb.userIsUnavailable) {
        window.mt_fb.aquireUserToken();
        setTimeout(function () {
            setFacebookNames($scope);
        }, 10);
    } else if (window.mt_fb.userIsUnavailable) {
        return;
    } else {
        $scope.find('.comment-facebookauthor').each(function (i, element) {
            var $element = $(element);
            var url = window.mt_fb.getUserUrl($element.data('facebookid'));
            if (url) {
                $.getJSON(url.url, {
                    access_token: url.accessToken,
                    fields: 'name'
                }).then(function (data) {
                    if (data.name) {
                        $element.html(data.name);
                    }
                });
            }
        });
    }
}
