
var commentsContainer = $('#article-comments');
var commentUrl = commentsContainer.data('getCommentsUrl');
$.get(commentUrl, function(data) {
    commentsContainer.html(data);
    applyCommentFormSubmitListener();
});

function applyCommentFormSubmitListener() {
    
    $('#article-commentForm form').submit(function (e) {
        e.preventDefault();
        var form = $(this);
        var button = form.find('button');
        var textarea = form.find('textarea');
        var url = form.attr('action');

        var validationMessage = form.find('#comment-validation-text');
        validationMessage.html('');

        if (textarea.val().length > 3) {

            form.find('.submitLoader').show();
            form.find('.submitText').hide();
            button.attr('disabled', 'disabled');

            $.post(url, form.serialize()).done(function (response) {
                var commentCount = $('#comments-count').html();
                $('#comments-count').html(1+parseInt(commentCount));
                form.find('textarea').val('');
                form.find('.submitLoader').hide();
                form.find('.submitText').show();
                button.attr('disabled', false);
                $('#article-commentForm').prepend(response);
            });
        }
        else {
            validationMessage.html('Kommentaren må være minst 4 tegn');
        }
       

       
    });
}