
var element = $('#game-showEvents');


var registerEvents = React.createElement(RegisterEvents, { routes: {
    GET_PLAYERS : element.data('get-players-url'),
    GET_EVENTTYPES: element.data('get-gameeventtypes-url'),
    ADD_EVENT: element.data('add-gameevent-url'),
    GET_EVENTS: element.data('get-gameevents-url'),
    DELETE_EVENT: element.data('delete-event-url')
},
 editMode: false});


ReactDOM.render(registerEvents, document.getElementById("game-showEvents"));