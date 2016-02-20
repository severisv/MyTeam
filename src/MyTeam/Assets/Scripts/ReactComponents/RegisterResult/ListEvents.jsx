var ListEvents = React.createClass({

    getIconClassName: function (type) {
      if(type == 0) return "fa fa-soccer-ball-o"  
      if(type == 1) return "icon icon-card-yellow"  
      if(type == 2) return "icon icon-card-red"  
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
                          <i className={iconClassName(event.Type)} ></i><span> {actions.getPlayerName(event.PlayerId)}</span>
                           {that.renderAssist(event, actions)}
                           {that.renderDeleteButton(event, actions, model)}
                </div>)
            
            })}</div>
            )

       
   
    },
    renderAssist: function (event, actions) {
        if (event.AssistedById) {
            return(<span className="no-wrap"> ( <i className="flaticon-football119"></i> {actions.getPlayerName(event.AssistedById)} )</span>)
        }
    },
    renderDeleteButton: function (event, actions, model) {
        if (model.editMode != false) {
            return(<a className="pull-right" onClick={actions.deleteEvent.bind(null, event.Id)}><i className="text-danger fa fa-times"></i></a>)

        }
    }
    
});

