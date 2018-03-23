export const waitForDom = new Promise(((resolve, reject) => {
  document.addEventListener('DOMContentLoaded', resolve)
}))
