import { post } from './api'

var ajax = ajax || {}

ajax.applyFormUpdateListener = function ($scope) {
  let typingTimer
  const updateElements = $scope.find('.ajax-update')
  updateElements.on('input', function () {
    const element = $(this)
    const value = element.val()
    clearTimeout(typingTimer)
    typingTimer = setTimeout(() => {
      post(element.data('href'), { value })
    }, 500)
  })
  updateElements.on('blur', function () {
    const element = $(this)
    const value = element.val()
    clearTimeout(typingTimer)
    post(element.data('href'), { value })
  })
}


ajax.applyAjaxLinkListeners = function ($scope) {

 
  $scope.find('input[type=checkbox].ajax-checkbox').each(function () {
    const element = $(this)
    const href = element.data('ajax-href')
    const completeFunction = element.data('ajax-complete')

    element.change((e) => {
      const value = e.target.checked
      element
        .parent()
        .find('i.fa-spinner')
        .show()

      post(href, { value })
        .then((data) => {
          eval(completeFunction)
          const successLabel = element.parent().find('.label-success')
          element
            .parent()
            .find('i.fa-spinner')
            .hide()
          successLabel.show()
          successLabel.fadeOut(1500)
        })
        .catch(() => {
          const failLabel = element.parent().find('.label-danger')
          element
            .parent()
            .find('i.fa-spinner')
            .hide()
          failLabel.show()
          failLabel.fadeOut(3000)
        })
    })
  })
}

export default ajax
