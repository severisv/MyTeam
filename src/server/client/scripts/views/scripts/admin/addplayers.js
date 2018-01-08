const ReactDOM = window.global.ReactDOM
const React = window.global.React

const addPlayers = React.createElement(window.AddPlayers)

ReactDOM.render(addPlayers, document.getElementById('add-players'))

window.mt_fb.login()
