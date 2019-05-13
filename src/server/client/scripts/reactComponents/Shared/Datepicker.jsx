import moment from 'moment'
import 'react'
import ReactDatepicker from 'react-datepicker'
import 'moment/locale/nb'
import React from "react";

moment.locale('nb')

export default class Datepicker extends React.Component {
  constructor(props) {
    super(props)
    this.state = {  
      value: props.value ? moment(props.value, 'DD.MM.YYYY') : undefined
    }
  }


  render() {
    return (
      <ReactDatepicker
        name={this.props.name}
        onChange={date =>     this.setState({ value: date })}
        selected={this.state.value}
        className="form-control"
        placeholderText={moment().format('DD.MM.YYYY')}
      />
    )
  }
}

