const ReactDOM = window.global.ReactDOM
const React = window.global.React

const gamePlan = React.createElement(window.GamePlan, $('#gameplan').data())

ReactDOM.render(gamePlan, document.getElementById('gameplan'))
