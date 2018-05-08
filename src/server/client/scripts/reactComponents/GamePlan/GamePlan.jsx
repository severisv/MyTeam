import React from 'react'

import { get, post } from '../../api'

import PlayerInput from './PlayerInput.jsx'

export default class GamePlan extends React.Component {
  constructor(props) {
    super(props)
    this.state = props.gameplan || {
      rows: [
        {
          id: 0,
          time: 0,
          lw: 'LW',
          s: 'S',
          rw: 'RW',
          lcm: 'LCM',
          rcm: 'RCM',
          dm: 'DM',
          lb: 'LB',
          lcb: 'LCB',
          rcb: 'RCB',
          rb: 'RB',
          gk: 'GK',
        },
      ],
      errorMessage: undefined,
    }
  }

  setPlayer = (i, key, event, input) => {
    const value = input ? input.newValue : event.target.value

    const rows = this.state.rows
    rows[i][key] = value
    this.setState({
      rows,
    })
  }

  setTime = (i, input) => {
    const value = parseInt(input.target.value)
    this.setState(state => ({
      rows: state.rows.map(row => row === state.rows[i]
        ? { ...row, time: isNaN(value) ? '' : value }
        : row)
    }))
  }

  duplicateRow = i => {
    this.setState(state => ({
      rows: state.rows.concat([{ ...state.rows[i], id: Date.now() }]).sort((a, b) => a.time - b.time),
    }), this.save)
  }

  removeRow = i => {
    this.setState(state => ({
      rows: state.rows.filter(row => row !== state.rows[i]),
    }), this.save)
  }

  save = () => {
    if (this.props.iscoach == 'True') {
      post(`/api/games/${this.props.gameid}/gameplan`, {
        gamePlan: JSON.stringify(this.state),
      })
        .then(response => {
          this.setState({ errorMessage: undefined })
        })
        .catch(() => {
          this.setState({ errorMessage: 'Feil ved lagring' })
        })
    }
  }

  publish = () => {
    this.setState({ isPublishing: true })
    post(`/api/games/${this.props.gameid}/gameplan/publish`)
      .then(() => {
        this.setState({ errorMessage: undefined, isPublishing: undefined, isPublished: true })
      })
      .catch(() => {
        this.setState({ errorMessage: 'Feil ved publisering' })
      })
  }

  renderPlayerInput = (lineup, i, position) => {
    const player = this.props.players.filter(p => p.Name == lineup[position])[0]
    return (
      <div className="gp-square">
        {player && player.ImageUrl ? (
          <img className="gameplan-playerImage" src={player.ImageUrl} />
        ) : (
            ''
          )}
        {this.props.iscoach == 'True' ? (
          <PlayerInput
            onBlur={this.save}
            onChange={this.setPlayer.bind(null, i, position)}
            value={lineup[position]}
            lineup={lineup}
            players={this.props.players}
          />
        ) : (
            <input readOnly value={lineup[position]} />
          )}
      </div>
    )
  }

  render() {
    const props = this.props
    const state = this.state

    return (
      <div className="gameplan">
        <div className="mt-main">
          <div className="mt-container clearfix">
            <h2 className="text-center">
              {props.team} vs {props.opponent}
            </h2>
            <div className={this.state.errorMessage ? 'alert alert-danger' : 'hidden'}>
              <i className="fa fa-exclamation-triangle" /> {this.state.errorMessage}
            </div>
            <br />
            <br />
            {this.state.rows.map((lineup, i) => (
              <div key={lineup.id}>
                <div className="text-center">
                  <input
                    readOnly={props.iscoach == 'False'}
                    className="gp-time"
                    onBlur={this.save}
                    onChange={this.setTime.bind(null, i)}
                    placeholder="tid"
                    value={lineup.time}
                  />min
                </div>
                <button
                  className={
                    props.iscoach == 'True' && state.rows.length > 1
                      ? 'pull-right hidden-print'
                      : 'hidden'
                  }
                  onBlur={this.save}
                  onClick={this.removeRow.bind(null, i)}
                >
                  <i className="fa fa-times" />
                </button>
                <button
                  className={props.iscoach == 'True' ? 'pull-right hidden-print' : 'hidden'}
                  onClick={() => this.duplicateRow(i)}
                >
                  <i className="fa fa-plus" />
                </button>
                <br />
                <div className="gp-row">{this.renderDiff(i)}</div>
                <br />
                <div className="gameplan-field">
                  <div className="gp-row">
                    {this.renderPlayerInput(lineup, i, 'lw')}
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 's')}
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 'rw')}
                  </div>
                  <div className="gp-row">
                    <div className="gp-square" />
                    <div className="gp-square" />
                    <div className="gp-square" />
                    <div className="gp-square" />
                    <div className="gp-square" />
                  </div>
                  <div className="gp-row">
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 'lcm')}
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 'rcm')}
                    <div className="gp-square" />
                  </div>
                  <div className="gp-row">
                    <div className="gp-square" />
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 'dm')}
                    <div className="gp-square" />
                    <div className="gp-square" />
                  </div>
                  <div className="gp-row">
                    {this.renderPlayerInput(lineup, i, 'lb')}
                    {this.renderPlayerInput(lineup, i, 'lcb')}
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 'rcb')}
                    {this.renderPlayerInput(lineup, i, 'rb')}
                  </div>
                  <div className="gp-row">
                    <div className="gp-square" />
                    <div className="gp-square" />
                    {this.renderPlayerInput(lineup, i, 'gk')}
                    <div className="gp-square" />
                    <div className="gp-square" />
                  </div>
                </div>
                <hr />
              </div>
            ))}
            <div className="text-center">
              <button
                className={props.iscoach == 'True' ? 'btn btn-primary hidden-print' : 'hidden'}
                onClick={() => this.duplicateRow(state.rows.length - 1)}
              >
                <i className="fa fa-plus" />
              </button>
            </div>
            <div className="clearfix">
              <br />&nbsp;
            </div>
            {this.renderPublishButton()}
          </div>
        </div>
        <div className="mt-sidebar">
          <div className="mt-container">{this.renderGameTime()}</div>
        </div>
      </div>
    )
  }

  renderDiff = i => {
    if (i < 1) return <div />
    const previous = this.state.rows[i - 1]
    const current = this.state.rows[i]

    function getPlayers(row) {
      const result = []
      for (const key in row) {
        if (key != 'time') {
          result.push(row[key])
        }
      }
      return result
    }

    function isInLineup(lineup, player) {
      const ln = []
      for (const key in lineup) {
        ln.push(lineup[key])
      }
      return ln.indexOf(player) != -1
    }

    function getSubs() {
      const result = []
      for (const key in current) {
        if (key != 'time' && key != 'id') {
          if (previous[key] != current[key]) {
            const substitution = {
              in: !isInLineup(previous, current[key]) ? current[key] : undefined,
              out: !isInLineup(current, previous[key]) ? previous[key] : undefined,
            }
            if (substitution.in || substitution.out) {
              result.push(substitution)
            }
          }
        }
      }
      return result
    }

    const subs = getSubs()

    const subsIn = subs.filter(sub => !sub.out)
    const subsOut = subs.filter(sub => !sub.in)
    const pairs = subs.filter(sub => sub.in && sub.out)

    const result = pairs.concat(
      subsIn.map((sub, index) => {
        const subOut = subsOut[index]
        return { in: sub.in, out: subOut ? subOut.out : undefined, positionChange: true }
      }),
    )

    return (
      <div>
        {result.map(sub => (
          <div className="text-center gp-subs" key={sub.in + sub.out}>
            <span className="gameplan-sub-in">{sub.in}</span>
            &nbsp;=&gt;&nbsp;
            <span className="gameplan-sub-out">{sub.out}</span>
            {sub.positionChange ? '*' : ''}
          </div>
        ))}
      </div>
    )
  }

  renderGameTime = () => {
    const gameTime = []
    const rows = this.state.rows
    for (const index in rows) {
      var i = parseInt(index)
      if (i == rows.length - 1) {
        for (var j in rows[i]) {
          gameTime.push({
            player: rows[i][j],
            minutes: 90 - rows[i].time,
          })
        }
      } else {
        for (var j in rows[i]) {
          gameTime.push({
            player: rows[i][j],
            minutes: rows[i + 1].time - rows[i].time,
          })
        }
      }
    }

    const total = {}
    for (const k in gameTime) {
      const element = gameTime[k]
      if (isNaN(element.player)) {
        if (element.player in total) {
          total[element.player] += element.minutes
        } else {
          total[element.player] = element.minutes
        }
      }
    }

    function getPlayerTime(player, value) {
      return (
        <div key={player.Id}>
          {player.Name}:&nbsp;<b>{value || 0}</b>
        </div>
      )
    }

    const result = []
    for (var i in this.props.players) {
      const player = this.props.players[i]
      result.push(getPlayerTime(player, total[player.Name]))
    }
    return <div>{result}</div>
  }
  renderPublishButton = () => {
    if (this.props.iscoach == 'False') return ''
    if (this.state.isPublished || this.props.ispublished == 'True') {
      return (
        <div className="text-center">
          <div className="disabled btn btn-lg btn-success">
            <i className="fa fa-check-circle" /> Publisert
          </div>
        </div>
      )
    }
    return (
      <div className="text-center hidden-print">
        <button onClick={this.publish} className="btn btn-lg btn-success">
          <span className={this.state.isPublishing ? 'hidden' : ''}>Publiser bytteplan</span>
          <i className={this.state.isPublishing ? 'fa fa-spinner fa-spin' : 'hidden'} />
        </button>
      </div>
    )
  }
}
