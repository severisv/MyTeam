const React = require('react')

module.exports = React.createClass({
  getInitialState() {
    return {
      players: [],
      eventTypes: [],
      events: [],
      editMode: this.props.editMode,
      squad: [],
      showPlayerUrl: this.props.routes.SHOW_PLAYER,
      loadingPlayers: true,
      loadingEvents: true,
      isAddingEvent: false,
      isRemovingEvent: false,
    }
  },

  componentDidMount() {
    const that = this
    const { props, state } = this
    $.getJSON(that.props.routes.GET_EVENTS).then((response) => {
      that.setState({
        events: response,
        loadingEvents: false,
      })
    })
    $.getJSON('/api/players').then((players) => {
      that.setState({
        players,
      })
      that.setState({
        addPlayerId: that.getPlayersNotInSquad()[0].id,
      })
    })
    $.getJSON(that.props.routes.GET_EVENTTYPES).then((response) => {
      that.setState({
        eventTypes: response,
        type: response[0].value,
      })
    })

    $.getJSON(`/api/games/${props.gameId}/squad`).then((response) => {
      that.setState({
        squad: response,
        loadingPlayers: false,
      })
    })
  },

  handleEventChange(event) {
    const playerId = this.state.playerId
      ? this.state.playerId
      : this.getEventPlayers(parseInt(event.target.value))[0].id

    this.setState({
      type: parseInt(event.target.value),
      playerId,
    })
  },
  handlePlayerChange(event) {
    const assistedById =
      this.state.assistedById == event.target.value ? null : this.state.assistedById

    const playerId = event.target.value == 'ingen' ? null : event.target.value

    this.setState({
      playerId,
      assistedById,
    })
  },
  handleAssistChange(event) {
    const assistedById = event.target.value == 'ingen' ? null : event.target.value

    this.setState({ assistedById })
  },
  handleSubmit() {
    const that = this
    const state = that.state
    const form = {
      Type: state.type,
      PlayerId: state.playerId,
      PlayerName: state.playerName,
      GameId: state.gameId,
      AssistedById: state.assistedById,
      AssistedByName: state.assistedByName,
      Id: state.Id,
    }
    this.setState({ isAddingEvent: true })
    $.post(that.props.routes.ADD_EVENT, form).then((response) => {
      if (response.success != false) {
        that.setState({
          events: that.state.events.concat([response]),
          isAddingEvent: false,
        })
      }
    })
  },
  deleteEvent(eventId) {
    const that = this
    that.setState({ isRemovingEvent: eventId })
    $.post(that.props.routes.DELETE_EVENT, { eventId }).then((response) => {
      if (response.success) {
        that.setState({
          events: that.state.events.filter(event => event.id != eventId),
          isRemovingEvent: undefined,
        })
      }
    })
  },

  handleAddPlayerChange(event) {
    this.setState({ addPlayerId: event.target.value })
  },

  addPlayer() {
    const that = this
    that.setState({ isAddingPlayer: true })
    $.post(that.props.routes.SELECT_PLAYER, {
      eventId: that.props.gameId,
      playerId: that.state.addPlayerId,
      isSelected: true,
    }).then((response) => {
      if (response.success) {
        const player = that.state.players.filter(player => player.id == that.state.addPlayerId)
        that.setState({
          squad: player
            .concat(that.state.squad)
            .sort((a, b) => a.firstName.localeCompare(b.firstName)),
        })
        that.setState({
          addPlayerId: that.getPlayersNotInSquad()[0].id,
          isAddingPlayer: false,
        })
      }
    })
  },

  removePlayerFromSquad(playerId) {
    const that = this
    that.setState({ isRemovingPlayer: playerId })
    $.post(that.props.routes.SELECT_PLAYER, {
      eventId: that.props.gameId,
      playerId,
      isSelected: false,
    }).then((response) => {
      if (response.success) {
        that.setState({
          squad: that.state.squad.filter(player => player.id != playerId),
          isRemovingPlayer: undefined,
        })
      }
    })
  },

  getPlayerName(playerId) {
    const squad = this.state.players.filter(player => player.id == playerId)
    if (squad.length > 0) return squad[0].firstName
    return 'Selvmål'
  },
  getPlayerShortName(playerId) {
    const squad = this.state.players.filter(player => player.id == playerId)
    if (squad.length > 0) return `${squad[0].firstName} ${squad[0].lastName}`
    return 'Selvmål'
  },
  getPlayerUrlName(playerId) {
    const squad = this.state.players.filter(player => player.id == playerId)

    if (squad.length > 0) return squad[0].urlName
    return ''
  },

  getEventPlayers(type) {
    return type == 0
      ? [{ id: 'ingen', fullName: '( Selvmål )' }].concat(this.state.squad)
      : this.state.squad
  },

  getPlayersNotInSquad() {
    return this.state.players.filter(isNotInList(this.state.squad))
  },

  render() {
    const actions = {
      handleEventChange: this.handleEventChange,
      handlePlayerChange: this.handlePlayerChange,
      handleAssistChange: this.handleAssistChange,
      handleSubmit: this.handleSubmit,
      getPlayerName: this.getPlayerName,
      getPlayerShortName: this.getPlayerShortName,
      getPlayerUrlName: this.getPlayerUrlName,
      deleteEvent: this.deleteEvent,
      getEventPlayers: this.getEventPlayers,
      removePlayerFromSquad: this.removePlayerFromSquad,
      getPlayersNotInSquad: this.getPlayersNotInSquad,
      addPlayer: this.addPlayer,
      handleAddPlayerChange: this.handleAddPlayerChange,
    }

    let eventsClassName = 'col-sm-9 col-sm-offset-2 col-xs-11 col-xs-offset-1 u-fade-in'
    if (this.state.loadingEvents) eventsClassName += ' u-fade-in--hidden'
    let playersClassName = 'col-sm-8 col-sm-offset-2 col-xs-10 col-xs-offset-1 u-fade-in'
    if (this.state.loadingPlayers) playersClassName += ' u-fade-in--hidden'
    return (
      <div className="game-showEventsWrapper">
        <div className="row">
          <div className={this.state.loadingEvents ? 'text-center' : 'hidden'}>
            <i className="fa fa-spinner fa-spin" />
          </div>
          <div className={eventsClassName}>
            <ListEvents model={this.state} actions={actions} />
            {this.renderEditView(actions)}
          </div>
        </div>
        <div className="row">
          <div className={this.state.loadingPlayers ? 'text-center' : 'hidden'}>
            <i className="fa fa-spinner fa-spin" />
          </div>
          <div className={playersClassName}>
            <ListSquad model={this.state} actions={actions} />
            {this.renderAddView(actions)}
          </div>
        </div>
      </div>
    )
  },

  renderEditView(actions) {
    if (this.props.editMode != false) {
      return <RegisterEvents model={this.state} actions={actions} />
    }
  },

  renderAddView(actions) {
    if (this.props.editMode != false) {
      return <AddPlayerToSquad model={this.state} actions={actions} />
    }
  },
})

function isNotInList(list) {
  return function (player) {
    for (let i = 0, len = list.length; i < len; i++) {
      if (list[i].id === player.id) {
        return false
      }
    }
    return true
  }
}
