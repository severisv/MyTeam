var moment = require('moment');
require('moment/locale/nb');
moment.locale('nb');

var ReactDatepicker = require('react-datepicker');
var React = require('react');


const Datepicker = ({ className, selected, onChange, onBlur }) => {
    if (selected) {
        selected = moment(selected);
    }
    return (
        <ReactDatepicker className={className} selected={selected} onChange={onChange} onBlur={onBlur} placeholderText={moment().format('DD.MM.YYYY')} />
    );
};

module.exports = Datepicker;