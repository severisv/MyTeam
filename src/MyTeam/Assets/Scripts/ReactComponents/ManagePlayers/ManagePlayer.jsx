
var ManagePlayer = React.createClass({
    getInitialState: function () {
        return ({
            player: this.props.player
        })
    },

    setPlayerStatus: function(event){
        this.props.setPlayerStatus(this.state.player, event.target.value)
    },

    togglePlayerRole: function(role){
        this.props.togglePlayerRole(this.state.player, role)
    },

    renderStatusOptions: function () {

        var statuses = this.props.options.playerStatus;
        var statusList = [];
        for (var key in statuses) {
            statusList.push(<option  key={statuses[key]}>{statuses[key]}</option>)
        }

        return (
          <select value={this.state.player.Status} className="form-control" onChange={this.setPlayerStatus}>          
               {statusList}
           </select>
            
        )
    },

        renderRoles: function () {

        var roles = this.props.options.playerRoles;
        var player = this.state.player;
        var buttons = [];
        for (var key in roles) {
            var role = roles[key];
            var buttonClass = "btn btn-sm";
            if (player.Roles.indexOf(role) > -1) buttonClass += " btn-primary";
            else buttonClass += " btn-default";
            buttons.push(<button onClick={this.togglePlayerRole.bind(null, role)} key={player.Id+role} className={buttonClass}>{role}</button>)
        }
        return (
            <span>          
               {buttons}
           </span>
            
        )
    },

    render: function () {

        var player = this.state.player;
        var toggleTeam = this.props.toggleTeam;
        var teamElements = this.props.teams.map(function (team, i) {
            var checked = player.TeamIds.indexOf(team.Id) > -1;
            return (<div key={team.ShortName + player.Id} className="col-xs-1 no-padding align-center"><input onClick={toggleTeam.bind(null, team.Id, player.Id)} type="checkbox" checked={checked} className="form-control" /></div>)
        });
        var editPlayerHref = this.props.routes.EDIT_PLAYER + "?playerId=" + player.Id;
        return (<div className="row list-player">
               <div className="col-sm-3 mp-name">{player.FullName}</div>
               <div className="col-sm-2 mp-status">{this.renderStatusOptions()}</div>
                {teamElements}
               <div className="col-sm-5 mp-roles">{this.renderRoles()}
                    <a className="pull-right" title="Rediger spiller" href={editPlayerHref}><i className="fa fa-edit"></i></a>
                </div>
        </div>)
    }


})