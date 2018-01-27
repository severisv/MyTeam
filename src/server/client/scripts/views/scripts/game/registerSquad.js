import { put } from '../../../api'

$('.register-attendance-input').click(function f() {
  const element = $(this)
  const eventId = element.data('eventId')
  const playerId = element.data('playerId')
  const value = element.is(':checked')
  element
    .parent()
    .find('i.fa-spinner')
    .show()
  $.post(Routes.SELECT_PLAYER, {
    playerId,
    eventId,
    isSelected: value,
  }).then((response) => {
    if (response.success) {
      const playerId = element.data('playerId')

      const squadCount = parseInt($('#squadCount').text())

      if (value == true) {
        const playerName = element.data('playerName')
        $('#squad').append(`<li id="${playerId}"><i class="flaticon-soccer18"></i>&nbsp;${playerName}</li>`)
        $('#squadCount').html(squadCount + 1)
      } else {
        $(`#${playerId}`).remove()
        $('#squadCount').html(squadCount - 1)
      }
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

$('#publishButton').click(function f() {
  const element = $(this)
  const href = element.data('href')
  element.find('span').hide()
  element.find('i').show()
  $.post(href).then((response) => {
    if (response.success) {
      element
        .parent()
        .html('<div class="disabled btn btn-success btn-lg"><i class="fa fa-check-circle"></i> Publisert</div>')
    } else {
      const failLabel = element.parent().find('.label-danger')
      element.find('span').show()
      element.find('i').hide()
      failLabel.show()
      failLabel.fadeOut(3000)
    }
  })
})

function postPublishMessage(element, message) {
  put(element.data('href'), {
    description: message,
  })
    .then((response) => {
      const successLabel = element.parent().find('.label-success')
      successLabel.show()
      successLabel.fadeOut(1500)
    })
    .catch(() => {
      const failLabel = element.parent().find('.label-danger')
      failLabel.show()
      failLabel.fadeOut(5000)
    })
}

let typingTimer

$('#publishMessage').on('input', function f() {
  const element = $(this)
  const message = element.val()
  clearTimeout(typingTimer)
  typingTimer = setTimeout(() => {
    postPublishMessage(element, message)
  }, 750)
})

$.getJSON(Routes.GET_ATTENDANCE, (data) => {
  const players = data.data
  for (const i in players) {
    const player = players[i]
    $(`#playerAttendance-${player.playerId}`).html(`${player.attendance}%`)
  }
})
