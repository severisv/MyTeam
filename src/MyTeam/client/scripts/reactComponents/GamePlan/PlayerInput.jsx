var Autosuggest = require('react-autosuggest');
var React = require('react');

function getSuggestions(players, lineup, input) {

    var ln = [];

     for(var key in lineup){
            ln.push(lineup[key]);
     }

    return players.filter(function(player) {
       return ln.indexOf(player.Name) == -1;
    });
}

function getSuggestionValue(suggestion) {
    return suggestion.Name;
}

function renderSuggestion(suggestion) {
    return <span>
            {suggestion.ImageUrl ? 
                    <img src={suggestion.ImageUrl} /> : 
                    '' 
                }
            {suggestion.Name}
        </span>;
}

function shouldRenderSuggestions() {
    return true;
}

function handleFocus(event) {
  event.target.select();
}

module.exports = React.createClass({
    getInitialState: function () {
        return ({suggestions: []});
    },

    onSuggestionsFetchRequested: function (value) {
        this.setState({
            suggestions: getSuggestions(this.props.players, this.props.lineup, value)
        });
    },

    onSuggestionsClearRequested: function (value) {
        this.setState({suggestions: []});
    },

    render() {

        var value = this.props.value;
        var suggestions = this.state.suggestions;

        var inputProps = {
            placeholder: 'Ingen',
            value,
            onChange: this.props.onChange,
            onBlur: this.props.onBlur,
            onFocus: handleFocus

        };

        return (<Autosuggest
            suggestions={suggestions}
            onSuggestionsFetchRequested={this.onSuggestionsFetchRequested}
            onSuggestionsClearRequested={this.onSuggestionsClearRequested}
            getSuggestionValue={getSuggestionValue}
            renderSuggestion={renderSuggestion}
            inputProps={inputProps}
            shouldRenderSuggestions={shouldRenderSuggestions}/>);
    }
});
