function applySignupFunctions(scope) {
  const $scope = $(scope)
  $scope.find('#event-showAll').click(function f(e) {
    e.preventDefault()
    const element = $(this)
    const parent = element.parent()
    parent.html('<i class="fa fa-spin fa-spinner"></i>')

    $.get(element.attr('href'), (data) => {
      parent.hide()
      parent.after(data)
    })
  })
  
}

window.applySignupFunctions = applySignupFunctions

applySignupFunctions('body')
