function applySignupFunctions(scope) {
  const $scope = $(scope)
  $scope.find('#event-showAll').click(function f(e) {
    e.preventDefault()
    const element = $(this)
    const parent = element.parent()
    parent.html('<i class="fa fa-spin fa-spinner"></i>')

    $.get(element.attr('href'), (data) => {
      parent.hide()
      parent.after(data)
    })
  })

  $scope.find('.event-addMessage').click(function () {
    const element = $(this)

    if (element.hasClass('active')) {
      element.removeClass('active')
      element.siblings('.event-message').hide()
    } else {
      element.addClass('active')
      element.siblings('.event-message').show()
    }
  })

  function postSignupMessage(element, message) {
    $.post(element.data('href'), {
      message,
    }).then((response) => {
      if (response.success) {
        const successLabel = element.parent().find('.label-success')
        element
          .parent()
          .find('i.fa-spinner')
          .hide()
        successLabel.show()
        successLabel.fadeOut(1500)
      } else {
        const failLabel = element.parent().find('.label-danger')
        element
          .parent()
          .find('i.fa-spinner')
          .hide()
        failLabel.show()
        failLabel.fadeOut(3000)
      }
    })
  }

  let typingTimer

  $scope.find('.event-message').on('input', function f() {
    const element = $(this)
    element
      .parent()
      .find('i.fa-spinner')
      .show()
    const message = element.val()
    clearTimeout(typingTimer)
    typingTimer = setTimeout(() => {
      postSignupMessage(element, message)
    }, 1200)
  })
}

window.applySignupFunctions = applySignupFunctions

applySignupFunctions('body')
