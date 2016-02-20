var RegisterEventsView = React.createClass({

    render: function () {
        var model = this.props.model;
        var actions = this.props.actions;
        var assistPlayers = [{ Id: 'ingen', FullName: "( Ingen )" }].concat(model.players.filter(function(element){return element.Id != model.PlayerId}));
        return (<div>
                   <br />

        <div className="form-horizontal">
            <div className="row">
                <div className="col-xs-12">
        <div className="col-sm-12 form-group no-padding">
                <label className="col-sm-3 control-label">Hendelse</label>
                <div className="col-sm-9">
                    <select className="form-control" onChange={actions.handleEventChange}>
                        {model.eventTypes.map(function (type) {
                        return (
                    <option key={type.Value} value={type.Value}>{type.Name}</option>);
                    })}
                    </select>
                    <span className="text-danger"></span>
            </div>
            </div>
            <div className="col-sm-12 form-group no-padding">
            <label className="col-sm-3 control-label">Hvem</label>
            <div className="col-sm-9">
                <select className="form-control" onChange={actions.handlePlayerChange}>
                    {actions.getEventPlayers(model.Type).map(function (player) {
                        return (
                            <option key={player.Id} value={player.Id}>{player.FullName}</option>);
                          })}
                </select>
                <span className="text-danger"></span>
            </div>


            </div>

                        {this.renderAssistForm(model, actions,assistPlayers)}
                    </div>

                </div>
                     <div className="col-sm-12 form-group no-padding">
                         <div className="col-sm-offset-3 col-xs-12">
                        <button className="btn btn-primary" onClick={actions.handleSubmit}>Legg til</button>
                         </div>
                     </div>
            </div>
        </div>
    );
    },
    renderAssistForm: function (model, actions, assistPlayers) {
        if (model.Type == 0) {
            return ( <div className="col-sm-12 no-padding form-group">
                 <label className="col-sm-3 control-label">Assist</label>
                <div className="col-sm-9">
                    <select className="form-control" onChange={actions.handleAssistChange}>
                        {assistPlayers.map(function (player) {
                            return (
           <option key={player.Id} value={player.Id}>{player.FullName}</option>);
                        })}
                    </select>
            <span className="text-danger"></span>
                </div>
            </div>)
        }

    }
});
