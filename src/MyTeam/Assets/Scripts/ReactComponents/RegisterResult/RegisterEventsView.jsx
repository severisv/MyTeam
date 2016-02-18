var RegisterEventsView = React.createClass({


    render: function () {
        var model = this.props.model;
        return (<div>
        <h4>Kamphendelser</h4>
    <div className="form-horizontal">
        <div className="row">
            <div className="col-xs-12">
    <div className="col-sm-6 form-group no-padding">
            <label className="col-sm-3 control-label">Hendelse</label>
            <div className="col-sm-9">
                <select className="form-control">
                    {model.eventTypes.map(function (type, i) {
                        return (
                <option key={i} value={type.Value }>{type.Name}</option>);
                    })}
                </select>
                <span className="text-danger"></span>
            </div>
    </div>
    <div className="col-sm-6 form-group no-padding">
    <label className="col-sm-3 control-label">Hvem</label>
    <div className="col-sm-9">
        <select className="form-control">
            {model.players.map(function (type, i) {
                return (
        <option key={i} value={type.Value }>{type.FullName}</option>);
            })}
        </select>
        <span className="text-danger"></span>
    </div>


    </div>

                {this.renderAssistForm(model)}
            </div>

        </div>

                     <div className="col-sm-6 form-group no-padding">
                         <div className="col-sm-offset-3 col-xs-12">
                        <button className="btn btn-primary">Legg til</button>
                         </div>
                     </div>
    </div>
        </div>
    );
    },
    renderAssistForm: function (model) {
        if (model.eventType == model.constants.GOAL) {
            return ( <div className="col-sm-6 form-group">
                 <label className="col-sm-3 control-label">Assist</label>
                <div className="col-sm-9">
                    <select className="form-control">
                        {model.players.map(function (type, i) {
                            return (
           <option key={i} value={type.Value }>{type.FullName}</option>);
                        })}
                    </select>
            <span className="text-danger"></span>
                </div>
            </div>)
        }

    }
});

