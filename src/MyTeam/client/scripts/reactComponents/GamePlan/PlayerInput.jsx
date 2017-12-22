const Autosuggest = require('react-autosuggest')
const React = require('react')

function getSuggestions(players, lineup, input) {
  const ln = []

  for (const key in lineup) {
    ln.push(lineup[key])
  }

  return players.filter(player => ln.indexOf(player.Name) == -1)
}

function getSuggestionValue(suggestion) {
  return suggestion.Name
}

function renderSuggestion(suggestion) {
  return (
    <span>
      {suggestion.ImageUrl ? <img src={suggestion.ImageUrl} /> : ''}
      {suggestion.Name}
    </span>
  )
}

function shouldRenderSuggestions() {
  return true
}

function handleFocus(event) {
  event.target.select()
}

module.exports = React.createClass({
  getInitialState() {
    return { suggestions: [] }
  },

  onSuggestionsFetchRequested(value) {
    this.setState({
      suggestions: getSuggestions(this.props.players, this.props.lineup, value),
    })
  },

  onSuggestionsClearRequested(value) {
    this.setState({ suggestions: [] })
  },

  render() {
    const value = this.props.value
    const suggestions = this.state.suggestions

    const inputProps = {
      placeholder: 'Ingen',
      value,
      onChange: this.props.onChange,
      onBlur: this.props.onBlur,
      onFocus: handleFocus,
    }

    return (
      <Autosuggest
        suggestions={suggestions}
        onSuggestionsFetchRequested={this.onSuggestionsFetchRequested}
        onSuggestionsClearRequested={this.onSuggestionsClearRequested}
        getSuggestionValue={getSuggestionValue}
        renderSuggestion={renderSuggestion}
        inputProps={inputProps}
        shouldRenderSuggestions={shouldRenderSuggestions}
      />
    )
  },
})
