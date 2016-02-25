"use strict";

var ManagePlayers = React.createClass({
         
    getInitialState: function () {
        return ({
            players: [],
            teams: []
        });
    },

    componentWillMount: function(){
      this.routes = Routes,
      this.options = {
          playerStatus: PlayerStatus,
          playerRoles: PlayerRoles
      }
    },

    componentDidMount: function() {
        var that = this;
        $.getJSON(that.routes.GET_PLAYERS).then(function (response) {
            that.setState({
                players: response
            });
        });

        $.getJSON(that.routes.GET_TEAMS).then(function (response) {
            that.setState({
                teams: response.data
            });
        });
    },

    togglePlayerRole: function (player, role) {
        var that = this;
        $.post(that.routes.TOGGLE_PLAYER_ROLE, { Id: player.Id, Role: role }).then(function (response) {
            if (response.Success) {
                var players = that.state.players;
                for (var i in that.state.players) {
                    if (players[i].Id == player.Id) {
                        var index = players[i].Roles.indexOf(role);
                        if (index > -1) {
                            that.state.players[i].Roles.splice(index, 1);
                        } else {
                            that.state.players[i].Roles.push(role);
                        }
                    }
                }
            }
            that.forceUpdate();
        });
    },
    setPlayerStatus: function (player, status) {
        var that = this;
        $.post(that.routes.SET_PLAYER_STATUS, { Id: player.Id, Status: status }).then(function (response) {
            if (response.Success) {
                var players = that.state.players;
                for (var i in that.state.players) {
                    if (players[i].Id == player.Id) {
                        that.state.players[i].Status = status;
                    }
                }
            }
            that.forceUpdate();
        });

    },
    
    toggleTeam: function (teamId, playerId) {

        var that = this;
        $.post(that.routes.TOGGLE_PLAYER_TEAM, { PlayerId: playerId, TeamId: teamId }).then(function (response) {
            if (response.Success) {
                var players = that.state.players;
                for (var i in that.state.players) {
                    if (players[i].Id == playerId) {
                        var teamIds = that.state.players[i].TeamIds;
                        if (teamIds.indexOf(teamId) > -1) {
                            that.state.players[i].TeamIds = teamIds.filter(function (element) { return element != teamId; });
                        }
                        else {
                            that.state.players[i].TeamIds.push(teamId);
                        }
                    }
                }
            }
            that.forceUpdate();
        });

    },
    
    renderPlayers: function (playerStatus) {
        if (!this.state.players) return "";

        var players = this.state.players.filter(function(player) {
            if (player.Status == playerStatus) {
                return (player)
            }
        });

        var options = this.options;
        var routes = this.routes;
        var teams = this.state.teams;
        var setPlayerStatus = this.setPlayerStatus;
        var togglePlayerRole = this.togglePlayerRole;
        var toggleTeam = this.toggleTeam;
        var playerElements = players.map(function(player, i) {
            return (<ManagePlayer key={player.Id} player={player} setPlayerStatus={setPlayerStatus} togglePlayerRole={togglePlayerRole} options={options} routes={routes} teams={teams} toggleTeam={toggleTeam} />)
        });

        var teamElements = teams.map(function (team, i) {
            return (<div key={team.Id} className="col-sm-1  col-xs-2 no-padding subheadline align-center">{team.ShortName}</div>)
        });

        return (<div className="manage-players">
    <div className="row">
        <div className="col-sm-3 col-xs-7 headline"><strong>{playerStatus}</strong></div>
        <div className="col-xs-2 subheadline hidden-xs"><strong>Status</strong></div>
        {teamElements}
        <div className="col-xs-3 subheadline hidden-xs"><strong>Roller</strong></div>
    </div>
    <div>
        {playerElements}
    </div>
        </div>);
    },

    render: function () {
        return (<div>
            {this.renderPlayers(this.options.playerStatus.Active)}
            {this.renderPlayers(this.options.playerStatus.Veteran)}
            {this.renderPlayers(this.options.playerStatus.Inactive)}
        </div>);
    }

});