import 'whatwg-fetch'

function call(method, url, payload) {
  return fetch(url, {
    method,
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
    body: JSON.stringify(payload),
  }).then((response) => {
    const contentType = response.headers.get('content-type')
    if (response.status >= 200 && response.status < 300) {
      if (contentType && contentType.indexOf('application/json') !== -1) {
        return response.json()
      }
      return response.text()
    }
    return response.json().then(payload => Promise.reject({ status: response.status, payload }))
  })
}

export const post = (url, payload) => call('POST', url, payload)
