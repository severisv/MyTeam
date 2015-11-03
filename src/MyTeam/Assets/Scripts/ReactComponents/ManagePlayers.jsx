
var ManagePlayers = React.createClass({
         
    getInitialState: function () {
        return ({
            players: []
        })
    },

    componentWillMount: function(){
      this.routes = Routes,
      this.options = {
          playerStatus: PlayerStatus,
          playerRoles: PlayerRoles
      }
    },

    componentDidMount() {

        $.getJSON(this.routes.GET_PLAYERS).then(response => {
            this.setState({
                players: response
            })
        }
        );
    },

    togglePlayerRole: function(player, role){
        $.post(this.routes.TOGGLE_PLAYER_ROLE, { Id: player.Id, Role: role }).then(response => {
            if (response.Success) {
                var players = this.state.players;
                for (var i in this.state.players) {
                    if (players[i].Id == player.Id) {
                        var index = players[i].Roles.indexOf(role);
                        if (index > -1) {
                            this.state.players[i].Roles.splice(index, 1);
                        }
                        else {
                            this.state.players[i].Roles.push(role);
                        }
                    }
                }
            }
            this.forceUpdate();
        }
    );
    },
    setPlayerStatus: function (player, status) {
   
        $.post(this.routes.SET_PLAYER_STATUS, { Id: player.Id, Status: status}).then(response => {
            if (response.Success) {
                var players = this.state.players;
                for (var i in this.state.players) {                    
                    if (players[i].Id == player.Id) {
                        this.state.players[i].Status = status;
                    }
                }
            }
            this.forceUpdate();
        }
      );

    },

    renderPlayers: function (playerStatus) {
        if (!this.state.players) return "";

        var players = this.state.players.filter(function (player) {
            if (player.Status == playerStatus) {
                return (player)
            }
        })

        var options = this.options;
        var routes = this.routes;
        var setPlayerStatus = this.setPlayerStatus;
        var togglePlayerRole = this.togglePlayerRole;
        var playerElements = players.map(function (player, i) {
            return (<ManagePlayer key={player.Id} player={player} setPlayerStatus={setPlayerStatus} togglePlayerRole={togglePlayerRole} options={options} routes={routes} />)
        })

        return (<div className="manage-players">
    <div className="row">
        <div className="col-xs-4 headline"><strong>{playerStatus}</strong></div>
        <div className="col-xs-3 subheadline hidden-xs"><strong>Status</strong></div>
        <div className="col-xs-5 subheadline hidden-xs"><strong>Roller</strong></div>
    </div>
    <div>
        {playerElements}
    </div>
        </div>)
    },

    render: function () {
        return (<div>
            {this.renderPlayers(this.options.playerStatus.Active)}
            {this.renderPlayers(this.options.playerStatus.Veteran)}
            {this.renderPlayers(this.options.playerStatus.Inactive)}
        </div>)
    }

});