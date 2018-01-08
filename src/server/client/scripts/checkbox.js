let checkbox = checkbox || {}

checkbox.showHideAssociatedElement = function (element, associatedSelector, reverse) {
  let show = element.checked
  if (reverse) {
    show = !element.checked
  }

  if (show) {
    $(associatedSelector).show()
  } else {
    $(associatedSelector).hide()
  }
}

module.exports = checkbox
