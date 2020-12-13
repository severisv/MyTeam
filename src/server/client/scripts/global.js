import ajax from './ajaxHelpers'


import ReactDOM from 'react-dom'
import React from 'react'

const ANIMATION_DURATON = 300
const global = global || {}

global.React = React
global.ReactDOM = ReactDOM

global.applyScopedJsComponents = function (selector) {
  const $scope = $(selector)
  $.tablesorter.defaults.sortInitialOrder = 'desc'
  $scope.find('table.tablesorter').tablesorter()
  $scope.find('a.mt-popover').popover({ trigger: 'hover' })
  applyActiveLinkSwapper($scope)
  applySelectLinkListeners($scope)
  ajax.applyFormUpdateListener($scope)
  ajax.applyAjaxLinkListeners($scope)
}

global.applyJsComponents = function () {
  const timestamp = new Date()
  const start = timestamp.getMilliseconds()
  this.applyScopedJsComponents($(document))
  applySlideDownMenuListeners()
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

function applySelectLinkListeners($scope) {
  $scope.find('.linkSelect').on('change', function () {
    const url = $(this).val()
    window.location = url
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
