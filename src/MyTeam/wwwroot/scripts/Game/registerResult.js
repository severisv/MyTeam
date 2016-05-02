
var element = $('#registerResult-addEvent');


var registerEvents = React.createElement(window.ShowGame, {
    routes: {
        GET_PLAYERS: element.data('get-players-url'),
        GET_SQUAD: element.data('get-squad-url'),
        GET_EVENTTYPES: element.data('get-gameeventtypes-url'),
        ADD_EVENT: element.data('add-gameevent-url'),
        GET_EVENTS: element.data('get-gameevents-url'),
        DELETE_EVENT: element.data('delete-event-url'),
        SHOW_PLAYER: element.data('show-player-url'),
        SELECT_PLAYER: element.data('select-player-url')
    },
    gameId: element.data('gameid')
});


ReactDOM.render(registerEvents, document.getElementById("registerResult-addEvent"));