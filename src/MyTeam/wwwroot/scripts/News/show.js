
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
        var url = form.attr('action');

        $.post(url, form.serialize()).done(function (response) {
            $('#article-commentForm').html(response);
            applyCommentFormSubmitListener();
        });
    });
}