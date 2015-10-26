
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

    changePlayerStatus: function (player, event,asd,asd2) {
        console.log(player)
        console.log(event.target)
        console.log(asd)
        console.log(asd2)
        $.post(this.routes.SET_PLAYER_STATUS).then(response => {
            if (response.Success && event.target.value) {
                for (var i in this.state.players) {
                    this.state.players[i].Status = event.target.value;
                }
            }
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
        var changePlayerStatus = this.changePlayerStatus;
        var playerElements = players.map(function (player, i) {
            return (<ManagePlayer key={i} player={player} changePlayerStatus={changePlayerStatus} options={options} />)
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

    renderStatusOptions: function () {

        var statuses = this.props.options.playerStatus;
        var statusList = [];
        for (var key in statuses) {
            statusList.push(<option key={statuses[key]}>{statuses[key]}</option>)
        }

        return (
            <select onChange={this.props.changePlayerStatus.bind(null, this.state.player)}>          
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