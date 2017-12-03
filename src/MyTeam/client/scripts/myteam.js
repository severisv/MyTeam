const mt = mt || {}

mt.deleteWithAjax = function (selector) {
  const element = $(selector)
  const href = element.data('href')
  $.post(href, (result) => {
    $(`#${element.data('parent')}`).fadeOut(300, function () {
      $(this).remove()
    })
    $('#alerts').html(result)
  })
}

mt.alert = function (type, message) {
  this.clearAlerts()
  $(`#${type} .alert-content`).html(message)
  $(`#${type}`).removeClass('hidden')
}

mt.clearAlerts = function () {
  $('#alerts')
    .find('.alert')
    .addClass('hidden')
}

mt.showElement = function (selector) {
  $(selector).show()
}
mt.hideElement = function (selector) {
  $(selector).hide()
}

mt.refreshNotificationButton = function () {
  $.get('/notifications', (result) => {
    $('#notification-button').html(result)
  })
}

module.exports = mt
