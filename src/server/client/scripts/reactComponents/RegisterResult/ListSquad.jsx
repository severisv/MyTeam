const React = require('react')

module.exports = React.createClass({
  render() {
    const model = this.props.model
    const actions = this.props.actions
    const that = this
    return (
      <div>
        <br />
        <h5>{model.squad.length > 0 ? 'Tropp' : ''}</h5>
        <div className="flex flex-justify-center game-squadList">
          <ul className="list-unstyled">
            {model.squad.map((player) => {
              const showPlayerUrl = `${model.showPlayerUrl}/${player.urlName}`
              return (
                <li key={player.id}>
                  <i className="flaticon-soccer18" />&nbsp;<a
                    href={showPlayerUrl}
                    className="underline"
                  >
                    {player.name}
                  </a>
                  {model.isRemovingPlayer == player.id ? (
                    <i className="fa fa-spinner fa-spin removePlayer" />
                  ) : (
                    that.renderDeleteButton(player, actions, model)
                  )}
                </li>
              )
            })}
            <li key="loader">
              {model.isAddingPlayer ? <i className="fa fa-spinner fa-spin" /> : ''}
            </li>
          </ul>
        </div>
      </div>
    )
  },
  renderDeleteButton(player, actions, model) {
    if (model.editMode != false) {
      return (
        <a className="pull-right" onClick={actions.removePlayerFromSquad.bind(null, player.id)}>
          &nbsp;&nbsp;<i className="text-danger fa fa-times" />
        </a>
      )
    }
  },
})
