const moment = require('moment')
const React = require('react')
const ReactDatepicker = require('react-datepicker')
const react = require('react')
require('moment/locale/nb')

moment.locale('nb')

const Datepicker = react.createClass({
  getInitialState() {
    const date = this.props.value ? moment(this.props.value, 'DD.MM.YYYY') : undefined
    return { value: date }
  },

  handleChange(date) {
    this.setState({ value: date })
  },

  render() {
    return (
      <ReactDatepicker
        name={this.props.name}
        onChange={this.handleChange}
        selected={this.state.value}
        className="form-control"
        placeholderText={moment().format('DD.MM.YYYY')}
      />
    )
  },
})

module.exports = Datepicker
