var ReactDOM = window.global.ReactDOM;
var React = window.global.React;
    
var gamePlan = React.createElement(window.GamePlan, $('#gameplan').data());
    
ReactDOM.render(gamePlan, document.getElementById("gameplan"));
    
