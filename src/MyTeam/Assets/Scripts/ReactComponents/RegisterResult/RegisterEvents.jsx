var RegisterEvents = React.createClass({
    getInitialState: function () {
        return ({
            players: [],
            eventTypes: [],
            events: [],
            editMode: this.props.editMode,
            squad: []

        });
    },

    componentDidMount: function () {
        var that = this;
        $.getJSON(that.props.routes.GET_PLAYERS).then(function (response) {
            that.setState({
                squad: response
            });
        });
        $.getJSON(that.props.routes.GET_EVENTTYPES).then(function (response) {

            that.setState({
                eventTypes: response,
                Type: response[0].Value
            });
        });
        $.getJSON(that.props.routes.GET_EVENTS).then(function (response) {

            that.setState({
                events: response
            });
        });
    },

    handleEventChange: function (event) {
        var playerId = this.state.PlayerId ?
            this.state.PlayerId :
            this.getEventPlayers(parseInt(event.target.value))[0].Id

        this.setState({
            Type: parseInt(event.target.value),
            PlayerId: playerId

        })
    },
    handlePlayerChange: function (event) {
        var assistedById = this.state.AssistedById == event.target.value ?
                  null :
                  this.state.AssistedById

        var playerId = event.target.value == 'ingen' ?
                 null :
                 event.target.value

        this.setState({
            PlayerId: playerId,
            AssistedById: assistedById
        })
    },
    handleAssistChange: function (event) {
        var assistedById = event.target.value == 'ingen' ?
               null :
               event.target.value

        this.setState({ AssistedById: assistedById })
    },
    handleSubmit: function () {
        var that = this;
        $.post(that.props.routes.ADD_EVENT,
            that.state
        ).then(function (response) {

            if (response.Success != false) {
                that.setState({
                    events: that.state.events.concat([
                       response
                    ])
                })
            }

        })

    },
    deleteEvent: function (eventId) {
        var that = this;
        $.post(that.props.routes.DELETE_EVENT,
            { eventId: eventId }
        ).then(function (response) {
            if (response.Success) {
                that.setState({
                    events: that.state.events.filter(function (event) {
                        return event.Id != eventId
                    })
                })
            }
        })
    },


    getPlayerName: function (playerId) {
        var squad = this.state.squad.filter(function (player) {
            return player.Id == playerId
        });
        if (squad.length > 0) return squad[0].FullName;
        else return "Selvmål";
    },

    getEventPlayers: function (type) {
        return type == 0 ?
          [{ Id: 'ingen', FullName: "( Selvmål )" }].concat(this.state.squad) :
          this.state.squad;
    },

    render: function () {
        var actions = {
            handleEventChange: this.handleEventChange,
            handlePlayerChange: this.handlePlayerChange,
            handleAssistChange: this.handleAssistChange,
            handleSubmit: this.handleSubmit,
            getPlayerName: this.getPlayerName,
            deleteEvent: this.deleteEvent,
            getEventPlayers: this.getEventPlayers
        }


        return (
            <div className="game-showEventsWrapper">
                        <div className="row">
                    <div className="col-sm-9 col-sm-offset-2 col-xs-11 col-xs-offset-1">
                    <ListEvents model={this.state} actions={actions}></ListEvents>
                        {this.renderEditView(actions)}
                       </div>
                  </div>
                <div className="row">
                    <div className="col-sm-8 col-sm-offset-2 col-xs-10 col-xs-offset-1">
                <ListSquad model={this.state} actions={actions}></ListSquad>
                    </div>
               </div>
        </div>);
    },

    renderEditView: function (actions) {
        if (this.props.editMode != false) {
            return (<RegisterEventsView model={this.state} actions={actions }></RegisterEventsView>)
        }
    }


});
