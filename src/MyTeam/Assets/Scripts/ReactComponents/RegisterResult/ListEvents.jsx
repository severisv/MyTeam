var ListEvents = React.createClass({

    getIconClassName: function (type) {
      if(type == 0) return "fa fa-soccer-ball-o"  
      if(type == 1) return "icon icon-card-yellow"  
      if(type == 2) return "icon icon-card-red"  ;
    },

    render: function () {
        var model = this.props.model;
        var actions = this.props.actions;
        var that = this;
        var iconClassName = this.getIconClassName


        return (
            <div className="col-sm-offset-1 gameEvents">
            {model.events.map(function(event, i)
            {
                return(<div key={i} className="gameEvent">
                          <span className="no-wrap"><i className={iconClassName(event.Type)}></i>&nbsp;&nbsp;
                             {that.renderPlayerLink(actions, event, model)}
                           </span><span>&nbsp; </span>
                           {that.renderAssist(event, actions, model)}
                           {that.renderDeleteButton(event, actions, model)}
                </div>)
            
            })}</div>
            )
    },

    renderPlayerLink: function (actions, event, model) {
        if (event.PlayerId) {
            var showPlayerUrl = model.showPlayerUrl + '/' + event.PlayerId
            return (<a className="underline" href={showPlayerUrl}>{actions.getPlayerShortName(event.PlayerId)}</a>)
        } else {
            return (<span>{actions.getPlayerName(event.PlayerId)}</span>)
        }
    },

    renderAssist: function (event, actions, model) {
        if (event.AssistedById) {
            var showPlayerUrl = model.showPlayerUrl + '/' + event.AssistedById
            return (<span className="no-wrap">( <i className="flaticon-football119"></i> <a className="underline" href={showPlayerUrl }>{actions.getPlayerShortName(event.AssistedById)}</a> )</span>)
        }
    },
    renderDeleteButton: function (event, actions, model) {
        if (model.editMode != false) {
            return(<a className="pull-right" onClick={actions.deleteEvent.bind(null, event.Id)}><i className="text-danger fa fa-times"></i></a>)

        }
    }
    
});

