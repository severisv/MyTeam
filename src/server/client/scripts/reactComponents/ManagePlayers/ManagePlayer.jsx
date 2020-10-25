const React = require('react')

module.exports = React.createClass({
  getInitialState() {
    return {
      player: this.props.player,
    }
  },

  setPlayerStatus(event) {
    this.props.setPlayerStatus(this.state.player, event.target.value)
  },

  togglePlayerRole(role) {
    this.props.togglePlayerRole(this.state.player, role)
  },

  renderStatusOptions() {
    const statuses = this.props.statuses
    const statusList = []
    for (const key in statuses) {
      statusList.push(<option value={statuses[key]} key={statuses[key]}>
        {statuses[key]}
                      </option>)
    }

    return (
      <select
        value={this.state.player.status}
        className="form-control"
        onChange={this.setPlayerStatus}
      >
        {statusList}
      </select>
    )
  },

  renderRoles() {
    const roles = this.props.roles
    const player = this.state.player
    const buttons = []
    for (const key in roles) {
      const role = roles[key]
      let buttonClass = 'btn btn-sm'
      if (player.roles.indexOf(role) > -1) buttonClass += ' btn-primary'
      else buttonClass += ' btn-default'
      buttons.push(<button
        onClick={this.togglePlayerRole.bind(null, role)}
        key={player.id + role}
        className={buttonClass}
      >
        {role}
      </button>)
    }
    return <span>{buttons}</span>
  },

  render() {
    const player = this.state.player
    const toggleTeam = this.props.toggleTeam
    const teamElements = this.props.teams.map((team) => {
      const checked = player.teamIds.indexOf(team.id) > -1
      return (
        <div key={team.shortName + player.id} className="col-xs-2 col-sm-1 no-padding align-center">
          <input
            onClick={toggleTeam.bind(null, team.id, player.id)}
            type="checkbox"
            defaultChecked={checked}
            className="form-control"
          />
        </div>
      )
    })
    const editPlayerHref = `/spillere/endre/${player.urlName}`
    return (
      <div className="row list-player">
        <div className="col-sm-3 mp-name">{player.fullName}</div>
        <div className="col-sm-2 col-xs-7  mp-status">{this.renderStatusOptions()}</div>
        {teamElements}
        <div className="mp-roles">
          {this.renderRoles()}
          <a className="pull-right" title="Rediger spiller" href={editPlayerHref}>
            <i className="fa fa-edit" />
          </a>
        </div>
      </div>
    )
  },
})
