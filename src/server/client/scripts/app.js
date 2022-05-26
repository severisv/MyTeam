import 'font-awesome/css/font-awesome.css'
import 'bootstrap/dist/css/bootstrap.css'
import './lib/jquery.tablesorter.min'
import '../stylesheets/style.less'


const ANIMATION_DURATON = 300


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
  


$(document).ready(() => {
  const $scope = $(document)
  $.tablesorter.defaults.sortInitialOrder = 'desc'
  $scope.find('table.tablesorter').tablesorter()
  $scope.find('a.mt-popover').popover({ trigger: 'hover' })
  applyActiveLinkSwapper($scope)
  applySelectLinkListeners($scope)
  applySlideDownMenuListeners()
})



