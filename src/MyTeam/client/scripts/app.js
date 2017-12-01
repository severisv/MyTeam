import './lib/jquery.tablesorter.min'
import '../stylesheets/style.less'

window.AddPlayers = require('./reactComponents/AddPlayers/AddPlayers.jsx')
window.EmailAdd = require('./reactComponents/AddPlayers/EmailAdd.jsx')
window.FacebookAdd = require('./reactComponents/AddPlayers/FacebookAdd.jsx')
window.ManagePlayer = require('./reactComponents/ManagePlayers/ManagePlayer.jsx')
window.ManagePlayers = require('./reactComponents/ManagePlayers/ManagePlayers.jsx')
window.AddPlayerToSquad = require('./reactComponents/RegisterResult/AddPlayerToSquad.jsx')
window.ListEvents = require('./reactComponents/RegisterResult/ListEvents.jsx')
window.ListSquad = require('./reactComponents/RegisterResult/ListSquad.jsx')
window.RegisterEvents = require('./reactComponents/RegisterResult/RegisterEvents.jsx')
window.AddPlayerToSquad = require('./reactComponents/RegisterResult/AddPlayerToSquad.jsx')
window.ShowGame = require('./reactComponents/RegisterResult/ShowGame.jsx')
window.GamePlan = require('./reactComponents/GamePlan/GamePlan.jsx')

require('./ajaxHelpers')
window.checkbox = require('./checkbox')
window.mt_fb = require('./facebook')
window.global = require('./global')
window.layout = require('./layout')
window.mt = require('./myteam')
