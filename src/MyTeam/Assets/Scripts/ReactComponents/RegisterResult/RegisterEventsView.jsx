var RegisterEventsView = React.createClass({


    render: function () {
        var model = this.props.model;
        return (<div>
        <h4>Kamphendelser</h4>
        <select className="form-control">
            {model.eventTypes.map(function (type, i) {
                return (<option key={i} value={type.Value}>{type.Name}</option>);
            })}
        </select>
            <select className="form-control">
                {model.players.map(function (player, i) {
                return (
                <option key={i} value={player.Id}>{player.FullName}</option>);
                })}
            </select>
        </div>);
    }
});
