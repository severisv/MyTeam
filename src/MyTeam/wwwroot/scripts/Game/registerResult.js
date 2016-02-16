
var element = $('#registerResult-addEvent');


var registerEvents = React.createElement(RegisterEvents, { routes: {
    GET_PLAYERS : element.data('get-players-url'),
    GET_EVENTTYPES: element.data('get-gameeventtypes-url')
}});


ReactDOM.render(registerEvents, document.getElementById("registerResult-addEvent"));