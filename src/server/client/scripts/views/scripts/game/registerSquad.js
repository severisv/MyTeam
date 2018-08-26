import { put, post } from '../../../api'

$('.register-attendance-input').click(function f() {
  const element = $(this)
  const eventId = element.data('eventId')
  const playerId = element.data('playerId')
  const value = element.is(':checked')
  element
    .parent()
    .find('i.fa-spinner')
    .show()
  post(`/api/games/${eventId}/squad/select/${playerId}`, {
    value,
  })
    .then(() => {
      const playerId = element.data('playerId')

      const squadCount = parseInt($('#squadCount').text())

      if (value == true) {
        const playerName = element.data('playerName')
        $('#squad').append(`<li id="${playerId}"><i class="flaticon-soccer18"></i>&nbsp;${playerName}</li>`)
        $('#squad')
          .children()
          .sort((a, b) => a.innerText.localeCompare(b.innerText, 'nb-NO'))
          .detach()
          .appendTo('#squad')
        $('#squadCount').html(squadCount + 1)
      } else {
        $(`#${playerId}`).remove()
        $('#squadCount').html(squadCount - 1)
      }
    })
    .catch(() => {
      const failLabel = element.parent().find('.label-danger')
      element
        .parent()
        .find('i.fa-spinner')
        .hide()
      failLabel.show()
      failLabel.fadeOut(3000)
    })
})

$('#publishButton').click(function f() {
  const element = $(this)
  const eventId = element.data('eventId')
  element.find('span').hide()
  element.find('i').show()

  $.post(`/api/games/${eventId}/squad/publish`)
    .then(() => {
      element
        .parent()
        .html('<div class="disabled btn btn-success btn-lg"><i class="fa fa-check-circle"></i> Publisert</div>')
    })
    .catch(() => {
      const failLabel = element.parent().find('.label-danger')
      element.find('span').show()
      element.find('i').hide()
      failLabel.show()
      failLabel.fadeOut(3000)
    })
})

function postPublishMessage(element, message) {
  const eventId = element.data('eventId')
  put(`/api/events/${eventId}/description`, {
    description: message,
  })
    .then(() => {
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

$.get(`/api/attendance/${registerSquad.teamId}/recent`).then((players) => {
  players.forEach(player =>
    $(`#playerAttendance-${player.memberId}`).html(`${player.attendancePercentage}%`))
})
