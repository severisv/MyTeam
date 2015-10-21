
var ManagePlayers = React.createClass({

    routes: {
        GET_PLAYERS: Routes.GET_PLAYERS
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

    renderPlayers: function (playerStatus) {
        if (!this.state.players) return "";

        var players = this.state.players.filter(function (player) {
            if (player.Status == playerStatus) {
                return (player)
            }
        })
        var playerElements = players.map(function (player) {
            return (<li>{player.FullName}</li>)
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
            {this.renderPlayers(Constants.Active)}
            {this.renderPlayers(Constants.Veterans)}
            {this.renderPlayers(Constants.Inactive)}
        </div>)
    }

});


