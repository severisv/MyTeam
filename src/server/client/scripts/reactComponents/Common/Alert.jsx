import React from 'react'

const getIcon = (type) => {
  switch (type) {
  case 'success':
    return 'check'
  case 'info':
    return 'check'
  case 'infosubtle':
    return 'check'
  case 'warning':
    return 'check'
  case 'danger':
    return 'check'
  default:
    return 'info'
  }
}
const Alert = ({ type, children }) => (
  <div className={`alert alert-${type}`} id={type}>
    <i className={`fa fa-${getIcon(type)}`} /> <span className="alert-content">{children}</span>
    <button type="button" className="close" data-dismiss="alert">
      ×
    </button>
  </div>
)

export default Alert
