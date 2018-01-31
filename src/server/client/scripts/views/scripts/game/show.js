const ReactDOM = window.global.ReactDOM
const React = window.global.React

const element = $('#game-showEvents')

if (element.length) {
  const registerEvents = React.createElement(window.ShowGame, {
    routes: {
      GET_EVENTTYPES: element.data('get-gameeventtypes-url'),
      ADD_EVENT: element.data('add-gameevent-url'),
      GET_EVENTS: element.data('get-gameevents-url'),
      DELETE_EVENT: element.data('delete-event-url'),
      SHOW_PLAYER: element.data('show-player-url'),
    },
    editMode: false,
    gameId: element.data('gameid'),
  })

  ReactDOM.render(registerEvents, document.getElementById('game-showEvents'))
}
