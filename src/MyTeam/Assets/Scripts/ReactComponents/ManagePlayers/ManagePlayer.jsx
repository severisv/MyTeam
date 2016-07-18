
module.exports = React.createClass({
    getInitialState: function () {
        return ({
            player: this.props.player
        });
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
            statusList.push(<option value={statuses[key]} key={statuses[key]}>{statuses[key]}</option>)
        }

        return (
            <select value={this.state.player.Status} className="form-control" onChange={this.setPlayerStatus}>
               {statusList}
           </select>
        );
    },

        renderRoles: function () {

        var roles = this.props.options.playerRoles;
        var player = this.state.player;
        var buttons = [];
        for (var key in roles) {
            var role = roles[key];
            var buttonClass = "btn btn-sm";
            if (player.roles.indexOf(role) > -1) buttonClass += " btn-primary";
            else buttonClass += " btn-default";
            buttons.push(<button onClick={this.togglePlayerRole.bind(null, role)} key={player.id+role} className={buttonClass}>{role}</button>)
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
        var teamElements = this.props.teams.map(function (team) {
            var checked = player.teamIds.indexOf(team.id) > -1;
            return (<div key={team.shortName + player.id} className="col-xs-2 col-sm-1 no-padding align-center">
                    <input onClick={toggleTeam.bind(null, team.id, player.id)} type="checkbox" defaultChecked={checked} className="form-control" />
                    </div>)
        });
        var editPlayerHref = this.props.routes.EDIT_PLAYER + "?playerId=" + player.id;
        return (<div className="row list-player">
               <div className="col-sm-3 mp-name">{player.fullName}</div>
               <div className="col-sm-2 col-xs-7  mp-status">{this.renderStatusOptions()}</div>
                {teamElements}
               <div className="col-sm-5 col-xs-12 mp-roles">{this.renderRoles()}
                    <a className="pull-right" title="Rediger spiller" href={editPlayerHref}><i className="fa fa-edit"></i></a>
                </div>
        </div>)
    }


})