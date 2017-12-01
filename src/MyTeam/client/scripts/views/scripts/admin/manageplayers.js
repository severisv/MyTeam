const ReactDOM = window.global.ReactDOM
const React = window.global.React

const managePlayers = React.createElement(window.ManagePlayers)

ReactDOM.render(managePlayers, document.getElementById('manage-players'))
