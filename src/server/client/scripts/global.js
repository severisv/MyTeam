import ajax from './ajaxHelpers'
import Datepicker from './reactComponents/Shared/Datepicker.jsx'


import ReactDOM from 'react-dom'
import React from 'react'

const ANIMATION_DURATON = 300
const global = global || {}

global.React = React
global.ReactDOM = ReactDOM

global.applyScopedJsComponents = function (selector) {
  const $scope = $(selector)
  applyDatepickers($scope)
  $.tablesorter.defaults.sortInitialOrder = 'desc'
  $scope.find('table.tablesorter').tablesorter()
  $scope.find('a.mt-popover').popover({ trigger: 'hover' })
  applyActiveLinkSwapper($scope)
  applyAjaxLinkActions($scope)
  applySelectLinkListeners($scope)
  applyMtAnchorListeners($scope)
  ajax.applyFormUpdateListener($scope)
  ajax.applyLoadListener($scope)
  ajax.applyAjaxLinkListeners($scope)
}

global.applyJsComponents = function () {
  const timestamp = new Date()
  const start = timestamp.getMilliseconds()

  this.applyScopedJsComponents($(document))
  applySlideDownMenuListeners()

  $(window).on('popstate', (e) => {
    if (e.originalEvent.state == 'ajax-action') {
      location.reload()
    }
  })

  applyBrowserCheck()

  console.log(`global.js: ${new Date().getMilliseconds() - start}ms`)
}

$(document).ready(() => {
  global.applyJsComponents()
})

// Slide down
function applySlideDownMenuListeners() {
  const element = $('.slide-down-parent')
  const subMenu = $(element.data('submenu'))
  element.click(() => {
    if (element.data('isFocused') == true) {
      element.data('isFocused', false)
      element.blur()
    } else {
      element.data('isFocused', true)
      subMenu.slideDown(ANIMATION_DURATON)
    }
  })

  element.focusout(() => {
    subMenu.slideUp(ANIMATION_DURATON)
    element.data('isFocused', false)
    setTimeout(() => {}, ANIMATION_DURATON)
  })
}

// Active links
function applyActiveLinkSwapper($scope) {
  $scope.find('ul.nav li').on('click', function () {
    $(this)
      .siblings()
      .removeClass('active')
    $(this).addClass('active')
  })
}

// Active links
function applyAjaxLinkActions($scope) {
  $scope.find('a[mt-pushstate]').on('click', function () {
    const $el = $(this)
    if ($el.attr('mt-pushstate')) {
      layout.pushState($el.attr('href'), $el.attr('mt-pushstate'))
    }
  })
}

function applySelectLinkListeners($scope) {
  $scope.find('.linkSelect').on('change', function () {
    const url = $(this).val()
    window.location = url
  })
}

function applyMtAnchorListeners($scope) {
  $scope.find('.mt-anchor').on('click', function () {
    const url = $(this).data('href')
    window.location = url
  })
}

function applyDatepickers($scope) {
  $scope.find('.datepicker').each((i, element) => {
    const $el = $(element)
    const datepickerElement = React.createElement(Datepicker, {
      value: $el.data('value'),
      name: $el.attr('id'),
    })
    ReactDOM.render(datepickerElement, element)
  })
}

function applyBrowserCheck() {
  if (isIEOrEdge()) {
    $('html').addClass('ie')
  }
}

function isIEOrEdge() {
  const ua = window.navigator.userAgent

  const msie = ua.indexOf('MSIE ')
  if (msie > 0) {
    return true
  }

  const trident = ua.indexOf('Trident/')
  if (trident > 0) {
    return true
  }

  const edge = ua.indexOf('Edge/')
  if (edge > 0) {
    return true
  }

  return false
}

export default global
