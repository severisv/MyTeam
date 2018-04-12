let mt_fb = mt_fb || {}

mt_fb.aquireUserToken = function () {
  function saveToken() {
    const accessToken = FB.getAccessToken()
    mt_fb.accessToken = accessToken
  }

  function aquireToken() {
    if (!mt_fb.isLoaded === true) {
      console.log('Facebook SDK not yet loaded .')
    }
    FB.getLoginStatus((response) => {
      if (response.status === 'connected') {
        saveToken()
      } else {
        mt_fb.userIsUnavailable = true
      }
    })
  }

  if (!mt_fb.accessToken) {
    aquireToken()
  }

  if (mt_fb.userIsUnavailable) {
    return null
  }

  return mt_fb.accessToken
}

function login() {
  FB.getLoginStatus((response) => {
    if (response.status === 'connected') {
    } else {
      FB.login()
    }
  })
}

mt_fb.login = function () {
  if (!window.mt_fb.isLoaded) {
    setTimeout(() => {
      mt_fb.login()
    }, 50)
  } else {
    login()
  }
}

mt_fb.getSearchUrl = function () {
  const accessToken = mt_fb.aquireUserToken()
  if (accessToken) {
    const url = 'https://graph.facebook.com/v2.11/search'
    return {
      url,
      accessToken,
    }
  }
  return null
}

mt_fb.getUserUrl = function (id) {
  const accessToken = mt_fb.aquireUserToken()

  if (accessToken) {
    const url = `https://graph.facebook.com/v2.11/${id}`
    return {
      url,
      accessToken,
    }
  }
  return null
}

mt_fb.getUserImageUrl = function (id) {
  const accessToken = mt_fb.aquireUserToken()
  if (accessToken) {
    const url = `https://graph.facebook.com/v2.11/${id}/picture`
    return {
      url,
      accessToken,
    }
  }
  return null
}

module.exports = mt_fb
