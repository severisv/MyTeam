
var element = $('#registerResult-addEvent');


var registerEvents = React.createElement(ShowGame, { routes: {
    GET_PLAYERS : element.data('get-players-url'),
    GET_EVENTTYPES: element.data('get-gameeventtypes-url'),
    ADD_EVENT: element.data('add-gameevent-url'),
    GET_EVENTS: element.data('get-gameevents-url'),
    DELETE_EVENT: element.data('delete-event-url')
}});


ReactDOM.render(registerEvents, document.getElementById("registerResult-addEvent"));