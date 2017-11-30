var ReactDOM = window.global.ReactDOM;
var React = window.global.React;

var element = $('#game-showEvents');

if (element.length) {
    var registerEvents = React.createElement(window.ShowGame, {
        routes: {
            GET_SQUAD: element.data('get-squad-url'),
            GET_EVENTTYPES: element.data('get-gameeventtypes-url'),
            ADD_EVENT: element.data('add-gameevent-url'),
            GET_EVENTS: element.data('get-gameevents-url'),
            DELETE_EVENT: element.data('delete-event-url'),
            SHOW_PLAYER: element.data('show-player-url'),
            GET_PLAYERS: element.data('get-players-url')
        },
        editMode: false,
        gameId: element.data('gameid')

    });


    ReactDOM.render(registerEvents, document.getElementById("game-showEvents"));
}

