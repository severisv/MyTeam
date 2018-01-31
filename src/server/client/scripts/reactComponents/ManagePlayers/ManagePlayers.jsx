﻿import React from 'react'

import { put } from '../../api'

export default class ManagePlayers extends React.Component {
  constructor() {
    super()
    this.state = {
      players: [],
      teams: [],
      collapsedSections: ['Sluttet'],
    }
  }

  componentWillMount() {
    this.options = {
      playerStatus: PlayerStatus,
      playerRoles: PlayerRoles,
    }
  }

  componentDidMount() {
    const that = this
    $.getJSON('/api/members').then(response => {
      that.setState({
        players: response,
      })
    })

    $.getJSON('/api/teams').then(teams => {
      that.setState({
        teams,
      })
    })
  }

  togglePlayerRole = (player, role) => {
    const state = this.state
    put(`/api/members/${player.id}/togglerole`, {
      Role: role,
    }).then(response => {
      const players = state.players
      for (const i in state.players) {
        if (players[i].id == player.id) {
          const index = players[i].roles.indexOf(role)
          if (index > -1) {
            state.players[i].roles.splice(index, 1)
          } else {
            state.players[i].roles.push(role)
          }
        }
      }
      this.forceUpdate()
    })
  }
  setPlayerStatus = (player, status) => {
    const state = this.state
    put(`/api/members/${player.id}/status`, {
      Status: status,
    }).then(response => {
      const players = state.players
      for (const i in state.players) {
        if (players[i].id == player.id) {
          state.players[i].status = status
        }
      }
      this.forceUpdate()
    })
  }

  toggleSection = section => {
    const collapsedSections = this.state.collapsedSections
    collapsedSections.splice(this.state.collapsedSections.indexOf(section), 1)

    this.setState({
      collapsedSections,
    })
  }

  sectionIsCollapsed = section => {
    return this.state.collapsedSections.indexOf(section) > -1
  }

  toggleTeam = (teamId, playerId) => {
    const that = this
    put(`/api/members/${playerId}/toggleteam`, {
      TeamId: teamId,
    }).then(() => {
      const players = that.state.players
      for (const i in that.state.players) {
        if (players[i].id == playerId) {
          const teamIds = that.state.players[i].teamIds
          if (teamIds.indexOf(teamId) > -1) {
            that.state.players[i].teamIds = teamIds.filter(element => element != teamId)
          } else {
            that.state.players[i].teamIds.push(teamId)
          }
        }
      }
      that.forceUpdate()
    })
  }

  renderPlayers = (playerStatus, isCollapsed) => {
    if (!this.state.players) return ''

    const players = this.state.players.filter(player => {
      if (player.status == playerStatus) {
        return player
      }
    })

    if (players.length <= 0) return ''

    const options = this.options
    const routes = this.routes
    const teams = this.state.teams
    const setPlayerStatus = this.setPlayerStatus
    const togglePlayerRole = this.togglePlayerRole
    const toggleTeam = this.toggleTeam
    const playerElements = players.map((player, i) => (
      <ManagePlayer
        key={player.id}
        player={player}
        setPlayerStatus={setPlayerStatus}
        togglePlayerRole={togglePlayerRole}
        options={options}
        routes={routes}
        teams={teams}
        toggleTeam={toggleTeam}
      />
    ))

    const teamElements = teams.map((team, i) => (
      <div
        key={team.id}
        className="col-sm-1  col-xs-2 no-padding-left no-padding-right subheadline align-center"
      >
        {team.shortName}
      </div>
    ))

    return (
      <div className="manage-players">
        <div className="row">
          <div className="col-sm-3 col-xs-7 headline">
            <strong>{playerStatus}</strong>
            {this.sectionIsCollapsed(playerStatus) ? (
              <span className="subheadline smaller">
                &nbsp;({playerElements.length})&nbsp;
                <a onClick={this.toggleSection.bind(this, playerStatus)} className="anchor">
                  Vis
                </a>
              </span>
            ) : (
              ''
            )}
          </div>
          <div className="col-xs-2 subheadline hidden-xs">
            <strong>Status</strong>
          </div>
          {teamElements}
          <div className="col-xs-3 subheadline hidden-xs">
            <strong>Roller</strong>
          </div>
        </div>
        {!this.sectionIsCollapsed(playerStatus) ? <div>{playerElements}</div> : ''}
      </div>
    )
  }

  render() {
    return (
      <div>
        {this.renderPlayers(this.options.playerStatus.Active)}
        {this.renderPlayers(this.options.playerStatus.Veteran)}
        {this.renderPlayers(this.options.playerStatus.Inactive)}
        {this.renderPlayers(this.options.playerStatus.Trener)}
        {this.renderPlayers(this.options.playerStatus.Quit)}
      </div>
    )
  }
}
