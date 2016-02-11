
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
        form.find('.submitLoader').show();
        form.find('.submitText').hide();
        form.find('button').attr('disabled','disabled');
        var url = form.attr('action');

        $.post(url, form.serialize()).done(function (response) {
            form.find('textarea').val('');
            form.find('.submitLoader').hide();
            form.find('.submitText').show();
            form.find('button').attr('disabled', false);
            $('#article-commentForm').prepend(response);
        });
    });
}