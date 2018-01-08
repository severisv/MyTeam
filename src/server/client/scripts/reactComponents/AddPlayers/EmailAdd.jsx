const React = require('react')

module.exports = React.createClass({
  getInitialState() {
    return { firstname: '', lastname: '', email: '' }
  },

  submit() {
    const user = {
      first_name: this.state.firstname,
      last_name: this.state.lastname,
      email: this.state.email,
    }
    this.props.addPlayer(user)
  },

  handleFirstnameChange(event) {
    this.setState({ firstname: event.target.value })
  },
  handleLastnameChange(event) {
    this.setState({ lastname: event.target.value })
  },
  handleEmailChange(event) {
    this.setState({ email: event.target.value })
  },

  render() {
    return (
      <div>
        <span className="text-danger">{this.props.validationMessage}</span>
        <input
          onChange={this.handleEmailChange}
          className="form-control"
          placeholder="E-postadresse"
        />
        <input
          onChange={this.handleFirstnameChange}
          className="form-control"
          placeholder="Fornavn"
        />
        <input
          onChange={this.handleLastnameChange}
          className="form-control"
          placeholder="Etternavn"
        />
        <button onClick={this.submit} className="btn btn-primary">
          <i className="fa fa-plus" />&nbsp;Legg til
        </button>
      </div>
    )
  },
})
