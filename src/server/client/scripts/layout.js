﻿const layout = layout || {}

layout.setPageName = function (text) {
  $('.mt-page-header')
    .find('h1')
    .html(text)
}

layout.pushState = function f(href, title) {
  const titlePrefix = document.title.split(' - ')[0]
  document.title = `${titlePrefix} - ${title}`
  history.pushState('ajax-action', title, href)
  layout.setPageName(title)
}

module.exports = layout