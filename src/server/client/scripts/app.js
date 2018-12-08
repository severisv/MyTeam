import 'font-awesome/css/font-awesome.css'
import 'bootstrap/dist/css/bootstrap.css'
import './lib/jquery.tablesorter.min'
import '../stylesheets/style.less'

import React from 'react'
import ReactDOM from 'react-dom'
import ManagePlayers from './reactComponents/ManagePlayers/ManagePlayers'
import AddPlayers from './reactComponents/AddPlayers/AddPlayers'
import ShowGame from './reactComponents/RegisterResult/ShowGame'

import ajax from './ajaxHelpers'
import global from './global'

window.ajax = ajax

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
render(ShowGame, 'game-showEvents')
render(ShowGame, 'registerResult-addEvent')
