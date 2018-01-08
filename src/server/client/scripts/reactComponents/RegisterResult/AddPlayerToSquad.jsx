const React = require('react')

module.exports = React.createClass({
  render() {
    const model = this.props.model
    const actions = this.props.actions
    return (
      <div>
        <div className="col-sm-12 form-group no-padding">
          <label className="col-sm-2 control-label" />
          <div className="col-sm-9">
            <select className="form-control" onChange={actions.handleAddPlayerChange}>
              {actions.getPlayersNotInSquad().map(player => (
                <option key={player.id} value={player.id}>
                  {player.fullName}
                </option>
                ))}
            </select>
          </div>
        </div>
        <div className="col-sm-12 form-group no-padding">
          <div className="col-sm-offset-2 col-xs-12">
            <button className="btn btn-primary" onClick={actions.addPlayer}>
              Legg til
            </button>
          </div>
        </div>
      </div>
    )
  },
})
