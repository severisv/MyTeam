module.exports = React.createClass({
    getInitialState: function () {
        return ({
            players: [],
            eventTypes: [],
            events: [],
            editMode: this.props.editMode,
            squad: [],
            showPlayerUrl: this.props.routes.SHOW_PLAYER,
            loadingPlayers: true,
            loadingEvents: true

        });
    },

    componentDidMount: function () {
        var that = this;
         $.getJSON(that.props.routes.GET_EVENTS).then(function (response) {

            that.setState({
                events: response,
                loadingEvents: false
            });
        });
        $.getJSON(that.props.routes.GET_PLAYERS).then(function (response) {
            that.setState({
                players: response
            });
            that.setState({
                addPlayerId: that.getPlayersNotInSquad()[0].id
            })

        });
        $.getJSON(that.props.routes.GET_EVENTTYPES).then(function (response) {

            that.setState({
                eventTypes: response,
                type: response[0].value
            });
        });

            $.getJSON(that.props.routes.GET_SQUAD).then(function (response) {
            that.setState({
                squad: response,
                loadingPlayers: false
            });
        });
    },

    handleEventChange: function (event) {
        var playerId = this.state.playerId ?
            this.state.playerId :
            this.getEventPlayers(parseInt(event.target.value))[0].id

        this.setState({
            type: parseInt(event.target.value),
            playerId: playerId

        })
    },
    handlePlayerChange: function (event) {
        var assistedById = this.state.assistedById == event.target.value ?
                  null :
                  this.state.assistedById

        var playerId = event.target.value == 'ingen' ?
                 null :
                 event.target.value

        this.setState({
            playerId: playerId,
            assistedById: assistedById
        })
    },
    handleAssistChange: function (event) {
        var assistedById = event.target.value == 'ingen' ?
               null :
               event.target.value

        this.setState({ assistedById: assistedById })
    },
    handleSubmit: function () {
        var that = this;
        $.post(that.props.routes.ADD_EVENT,
            that.state
        ).then(function (response) {

            if (response.success != false) {
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
            if (response.success) {
                that.setState({
                    events: that.state.events.filter(function (event) {
                        return event.id != eventId
                    })
                })
            }
        })
    },

    handleAddPlayerChange: function (event) {
        this.setState({ addPlayerId: event.target.value })
    },

    addPlayer: function () {
        var that = this;
        $.post(that.props.routes.SELECT_PLAYER,
            { eventId: that.props.gameId, playerId: that.state.addPlayerId, isSelected: true }
        ).then(function (response) {
            if (response.success) {
                var player = that.state.players.filter(function (player) { return player.id == that.state.addPlayerId })
                that.setState({ squad: player.concat(that.state.squad).sort(function (a, b) { return a.firstName.localeCompare(b.firstName) }) })
                that.setState({
                    addPlayerId: that.getPlayersNotInSquad()[0].id
                })
            }
        })
    },


    removePlayerFromSquad: function (playerId) {
        var that = this;
        $.post(that.props.routes.SELECT_PLAYER,
            { eventId: that.props.gameId, playerId: playerId, isSelected: false }
        ).then(function (response) {
            if (response.success) {
                that.setState({
                    squad: that.state.squad.filter(function (player) {
                        return player.id != playerId
                    })
                })
            }
        })
    },


    getPlayerName: function (playerId) {
        var squad = this.state.squad.filter(function (player) {
            return player.id == playerId
        });
        if (squad.length > 0) return squad[0].firstName;
        else return "Selvmål";
    },
    getPlayerShortName: function (playerId) {
        var squad = this.state.squad.filter(function (player) {
            return player.id == playerId
        });
        if (squad.length > 0) return squad[0].firstName + " " + squad[0].lastName;
        else return "Selvmål";
    },

    getEventPlayers: function (type) {
        return type == 0 ?
          [{ id: 'ingen', fullName: "( Selvmål )" }].concat(this.state.squad) :
          this.state.squad;
    },

    getPlayersNotInSquad: function () {
        return this.state.players.filter(isNotInList(this.state.squad));
    },

    render: function () {
        var actions = {
            handleEventChange: this.handleEventChange,
            handlePlayerChange: this.handlePlayerChange,
            handleAssistChange: this.handleAssistChange,
            handleSubmit: this.handleSubmit,
            getPlayerName: this.getPlayerName,
            getPlayerShortName: this.getPlayerShortName,
            deleteEvent: this.deleteEvent,
            getEventPlayers: this.getEventPlayers,
            removePlayerFromSquad: this.removePlayerFromSquad,
            getPlayersNotInSquad: this.getPlayersNotInSquad,
            addPlayer: this.addPlayer,
            handleAddPlayerChange: this.handleAddPlayerChange
        }

        var eventsClassName = "col-sm-9 col-sm-offset-2 col-xs-11 col-xs-offset-1 u-fade-in";
        if(this.state.loadingEvents) eventsClassName += " u-fade-in--hidden"
        var playersClassName = "col-sm-8 col-sm-offset-2 col-xs-10 col-xs-offset-1 u-fade-in";
        if(this.state.loadingPlayers) playersClassName += " u-fade-in--hidden"
        return (
            <div className="game-showEventsWrapper">
                 <div className="row">
                    <div className={this.state.loadingEvents ? "text-center" : "hidden" }><i className="fa fa-spinner fa-spin"></i></div>
                    <div className={eventsClassName}>
                    <ListEvents model={this.state} actions={actions}></ListEvents>
                        {this.renderEditView(actions)}
                       </div>
                  </div>
                <div className="row">
                    <div className={this.state.loadingPlayers ? "text-center" : "hidden" }><i className="fa fa-spinner fa-spin"></i></div>
                    <div className={playersClassName}>
                <ListSquad model={this.state} actions={actions}></ListSquad>
                        {this.renderAddView(actions)}


                    </div>
               </div>
        </div>);
    },

    renderEditView: function (actions) {
        if (this.props.editMode != false) {
            return (<RegisterEvents model={this.state} actions={actions }></RegisterEvents>)
        }
    },

    renderAddView: function (actions) {
        if (this.props.editMode != false) {
            return (<AddPlayerToSquad model={this.state} actions={actions}></AddPlayerToSquad>
)
        }
    }


});


function isNotInList(list) {
    return function(player) {
        for (var i = 0, len = list.length; i < len; i++) {
            if (list[i].id === player.id) {
                return false;
            }
        }
        return true;
    }
}