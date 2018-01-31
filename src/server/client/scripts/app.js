import 'font-awesome/css/font-awesome.css'
import 'bootstrap/dist/css/bootstrap.css'
import './lib/jquery.tablesorter.min'
import '../stylesheets/style.less'

import React from 'react'
import ReactDOM from 'react-dom'

import ManagePlayers from './reactComponents/ManagePlayers/ManagePlayers.jsx'
import AddPlayers from './reactComponents/AddPlayers/AddPlayers.jsx'
import GamePlan from './reactComponents/GamePlan/GamePlan.jsx'

import ajax from './ajaxHelpers'
import global from './global'

window.ajax = ajax

window.EmailAdd = require('./reactComponents/AddPlayers/EmailAdd.jsx')
window.FacebookAdd = require('./reactComponents/AddPlayers/FacebookAdd.jsx')
window.ManagePlayer = require('./reactComponents/ManagePlayers/ManagePlayer.jsx')

window.AddPlayerToSquad = require('./reactComponents/RegisterResult/AddPlayerToSquad.jsx')
window.ListEvents = require('./reactComponents/RegisterResult/ListEvents.jsx')
window.ListSquad = require('./reactComponents/RegisterResult/ListSquad.jsx')
window.RegisterEvents = require('./reactComponents/RegisterResult/RegisterEvents.jsx')
window.AddPlayerToSquad = require('./reactComponents/RegisterResult/AddPlayerToSquad.jsx')
window.ShowGame = require('./reactComponents/RegisterResult/ShowGame.jsx')

window.checkbox = require('./checkbox')
window.mt_fb = require('./facebook')

window.global = global
window.layout = require('./layout')
window.mt = require('./myteam')

const render = (Component, target) => {
  const element = document.getElementById(target)
  if (element) {
    ReactDOM.render(<Component {...$(element).data()} />, element)
  }
}

render(ManagePlayers, 'manage-players')
render(AddPlayers, 'add-players')
render(GamePlan, 'gameplan')
