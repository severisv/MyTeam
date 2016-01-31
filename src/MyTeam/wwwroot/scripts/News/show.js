
var commentsContainer = $('#article-comments');
var commentUrl = commentsContainer.data('getCommentsUrl');
$.get(commentUrl, function(data) {
    commentsContainer.html(data);
});