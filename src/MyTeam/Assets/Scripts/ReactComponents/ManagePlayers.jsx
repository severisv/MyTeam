
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
        console.log("hei")

        $.getJSON(this.routes.GET_PLAYERS).then(response => {
            console.log(response)

            this.setState({
                players: response
            })
        }
        );
    },

    setPlayerStatus: function (player, status) {
   
        $.post(this.routes.SET_PLAYER_STATUS).then(response => {
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
        var setPlayerStatus = this.setPlayerStatus;
        var playerElements = players.map(function (player, i) {
            return (<ManagePlayer key={i} player={player} setPlayerStatus={setPlayerStatus} options={options} />)
        })

        return (<div className="manage-players">
    <div className="row">
        <div className="col-xs-4 headline"><strong>{playerStatus}</strong></div>
        <div className="col-xs-3 subheadline"><strong>Status</strong></div>
        <div className="col-xs-5 subheadline"><strong>Roller</strong></div>
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