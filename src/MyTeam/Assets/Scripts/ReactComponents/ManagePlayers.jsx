
var ManagePlayers = React.createClass({

    routes: Routes,

    options: {
        playerStatus: PlayerStatus
    },


    getInitialState: function () {
        return ({
            players: []
        })
    },

    componentDidMount() {

        $.getJSON(this.routes.GET_PLAYERS).then(response => {
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

        return (<div>
    <h3>{playerStatus}</h3>
    <ul>
        {playerElements}
    </ul>
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


var ManagePlayer = React.createClass({
    getInitialState: function () {
        return ({
            player: this.props.player
        })
    },

    setPlayerStatus: function(event){
        this.props.setPlayerStatus(this.state.player, event.target.value)
    },

    renderStatusOptions: function () {

        var statuses = this.props.options.playerStatus;
        var statusList = [];
        for (var key in statuses) {
            statusList.push(<option key={statuses[key]}>{statuses[key]}</option>)
        }

        return (
            <select onChange={this.setPlayerStatus}>          
               {statusList}
           </select>
            
        )
    },

    render: function () {

        var player = this.state.player;

        return (<li>
            {player.FullName}
            {this.renderStatusOptions()}
        </li>)
    }


})