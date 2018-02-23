import { waitForDom } from '../../../util/dom.js'

waitForDom.then(() => {
  $('.register-attendance-input').click(function f() {
    const element = $(this)
    const eventId = element.data('eventId')
    const playerId = element.data('playerId')
    const value = element.is(':checked')
    element
      .parent()
      .find('i.fa-spinner')
      .show()
    $.post('/intern/oppmote/bekreft', {
      playerId,
      eventId,
      didAttend: value,
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
  })
})
