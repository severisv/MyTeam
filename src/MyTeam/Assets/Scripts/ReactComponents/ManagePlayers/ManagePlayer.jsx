
var ManagePlayer = React.createClass({
    getInitialState: function () {
        return ({
            player: this.props.player
        })
    },

    setPlayerStatus: function(event){
        this.props.setPlayerStatus(this.state.player, event.target.value)
    },

    setRole: function(role){
        console.log(role)
    },

    renderStatusOptions: function () {

        var statuses = this.props.options.playerStatus;
        var statusList = [];
        for (var key in statuses) {
            statusList.push(<option key={statuses[key]}>{statuses[key]}</option>)
        }

        return (
            <select className="form-control" onChange={this.setPlayerStatus}>          
               {statusList}
           </select>
            
        )
    },

        renderRoles: function () {

        var roles = this.props.options.playerRoles;
        var player = this.state.player;
        var buttons = [];
        for (var key in roles) {
            console.log(player)
            var role = roles[key];
            var buttonClass = "btn";
            if (player.Roles.indexOf(role) > -1) buttonClass += " btn-primary";
            else buttonClass += " btn-default";
            buttons.push(<button onClick={this.setRole.bind(null, role)} key={player.Id+role} className={buttonClass}>{role}</button>)
        }

        return (
            <span>          
               {buttons}
           </span>
            
        )
    },

    render: function () {

        var player = this.state.player;

        return (<div className="row list-player">
               <div className="col-xs-4">{player.FullName}</div>
               <div className="col-xs-3">{this.renderStatusOptions()}</div>
               <div className="col-xs-5">{this.renderRoles()}</div>
        </div>)
    }


})