import 'font-awesome/css/font-awesome.css'
import 'bootstrap/dist/css/bootstrap.css'
import './lib/jquery.tablesorter.min'
import '../stylesheets/style.less'

import ajax from './ajaxHelpers'
import global from './global'

window.ajax = ajax
window.checkbox = require('./checkbox')

window.global = global
