var ajax = ajax || {}

ajax.applyFormUpdateListener = function ($scope) {
  let typingTimer
  const updateElements = $scope.find('.ajax-update')
  updateElements.on('input', function () {
    const element = $(this)
    const value = element.val()
    clearTimeout(typingTimer)
    typingTimer = setTimeout(() => {
      $.post(element.data('href'), { value })
    }, 500)
  })
  updateElements.on('blur', function () {
    const element = $(this)
    const value = element.val()
    clearTimeout(typingTimer)
    $.post(element.data('href'), { value })
  })
}

ajax.applyLoadListener = function ($scope) {
  $scope.find('.ajax-load').each(function () {
    const element = $(this)
    const href = element.attr('href')
    element.html('<i class="mt-loader fa fa-spin fa-spinner"></i>')
    $.get(href).then((data) => {
      element.html(data)
      window.global.applyScopedJsComponents(element)
    })
  })
}

function formIsValid($form) {
  let result = true
  $form.find('input[data-ajax-update-requiredfield]').each((i, element) => {
    if (!element.value) result = false
  })

  return result
}

ajax.applyAjaxLinkListeners = function ($scope) {
  $scope.find('a.ajax-link').each(function () {
    const element = $(this)
    const target = $(element.data('ajax-update'))
    const href = element.attr('href')

    element.click((e) => {
      e.preventDefault()
      target.addClass('ajax-replace')
      target.addClass('ajax-replace--hidden')

      setTimeout(() => {
        $.get(`${href}?ajax`, (response) => {
          window.scrollTo(0, 0)
          target.html(response)
          target.removeClass('ajax-replace--hidden')
          window.global.applyScopedJsComponents(target)
        })
      }, 50)
    })
  })

  $scope.find('a.ajax-post').each(function () {
    const element = $(this)
    const target = $(element.data('ajax-update'))
    const href = element.data('href')
    const completeFunction = element.data('ajax-complete')

    element.click((e) => {
      e.preventDefault()
      $.get(href, (response) => {
        target.html(response)
        window.global.applyScopedJsComponents(target)

        if (completeFunction) {
          eval(completeFunction)
        }
      })
    })
  })

  $scope.find('form.ajax-form').each(function () {
    const element = $(this)
    const target = $(element.data('ajax-update'))
    const appendTarget = $(element.data('ajax-append'))
    const prependTarget = $(element.data('ajax-prepend'))
    const href = element.attr('action')

    const submitButton = element.find('input[type=submit], button')

    element.submit((e) => {
      e.preventDefault()

      submitButton.addClass('isSubmitting')
      $.post(href, element.serialize(), (response) => {
        if (target.length) {
          target.html(response)
          window.global.applyScopedJsComponents(target)
        }
        if (appendTarget.length) {
          appendTarget.append(response)
          window.global.applyScopedJsComponents(appendTarget)
        }
        if (prependTarget.length) {
          prependTarget.prepend(response)
          window.global.applyScopedJsComponents(prependTarget)
        }

        element.find('input[data-ajax-update-clear], textarea[data-ajax-update-clear]').val('')
        submitButton.removeClass('isSubmitting')
        if (formIsValid(element)) {
          submitButton.prop('disabled', false)
        } else {
          submitButton.prop('disabled', true)
        }
      })
    })

    element.find('input[data-ajax-update-requiredfield]').on('input', () => {
      if (formIsValid(element)) {
        submitButton.prop('disabled', false)
      } else {
        submitButton.prop('disabled', true)
      }
    })
  })

  $scope.find('input[type=checkbox].ajax-checkbox').each(function () {
    const element = $(this)
    const href = element.data('ajax-href')
    const completeFunction = element.data('ajax-complete')

    element.change((e) => {
      const value = e.target.checked
      $.post(href, { value }).done((data) => {
        eval(completeFunction)
      })
    })
  })
}

module.exports = ajax
