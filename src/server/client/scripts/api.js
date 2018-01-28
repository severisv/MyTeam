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
  }).then(result => result.json())
}

export const post = (url, payload) => call('POST', url, payload)
export const put = (url, payload) => call('PUT', url, payload)

export function get(url) {
  return fetch(url, {
    method: 'GET',
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
  }).then(result => result.json())
}
