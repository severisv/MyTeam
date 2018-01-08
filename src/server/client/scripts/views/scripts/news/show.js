const commentsContainer = $('#article-comments')
const commentUrl = commentsContainer.data('getCommentsUrl')
$.get(commentUrl, (data) => {
  commentsContainer.html(data)
  applyCommentFormSubmitListener()
  getFacebookName()
})

function applyCommentFormSubmitListener() {
  $('#article-commentForm form').submit(function (e) {
    e.preventDefault()
    const form = $(this)
    const button = form.find('button')
    const textarea = form.find('textarea')
    const url = form.attr('action')

    const validationMessage = form.find('#comment-validation-text')
    validationMessage.html('')

    if (textarea.val().length < 4) {
      validationMessage.html('Kommentaren må være minst 4 tegn')
    } else if (
      form.find('.comment-nameInput').length > 0 &&
      !form.find('.comment-nameInput').val()
    ) {
      validationMessage.html('Navn må fylles ut')
    } else {
      form.find('.submitText').hide()
      button.attr('disabled', 'disabled')
      form.find('button').addClass('isSubmitting')

      $.post(url, form.serialize()).done((response) => {
        const commentCount = $('#comments-count').html()
        $('#comments-count').html(1 + parseInt(commentCount))
        form.find('button').removeClass('isSubmitting')
        form.find('textarea').val('')
        form.find('.submitText').show()
        button.attr('disabled', false)
        $('#article-commentswrapper').append(response)
      })
    }
  })
}

function getFacebookName() {
  if (!$('.comment-facebookUserName').length > 0) {
    return
  }

  if (!window.mt_fb.isLoaded) {
    setTimeout(() => {
      getFacebookName()
    }, 10)
  } else if (!window.mt_fb.accessToken && !window.mt_fb.userIsUnavailable) {
    window.mt_fb.aquireUserToken()
    setTimeout(() => {
      getFacebookName()
    }, 10)
  } else if (window.mt_fb.userIsUnavailable) {
  } else {
    const $element = $('.comment-facebookUserName')
    const url = window.mt_fb.getUserUrl($element.data('facebookid'))
    if (url) {
      $.getJSON(url.url, {
        access_token: url.accessToken,
        fields: 'name',
      }).then((data) => {
        $element.val(data.name)
      })
    }
  }
}
