var moment = require('moment');
var React = require('react');
var ReactDatepicker = require('react-datepicker');
var react = require('react');
require('moment/locale/nb');
moment.locale('nb');




var Datepicker = react.createClass({

    getInitialState() {
        var date = this.props.value ? moment(this.props.value, 'DD.MM.YYYY') : moment();
        return { value: date };
    },

    handleChange(date) {
        this.setState({value: date})
    },

    render() {
        return (
            <ReactDatepicker name={this.props.name} onChange={this.handleChange} selected={this.state.value} className="form-control"  placeholderText={moment().format('DD.MM.YYYY')} />
    );
    }
})
    
module.exports = Datepicker;