﻿import React from 'react'

import { get, post } from '../../api'

export default class AddPlayers extends React.Component {
  constructor() {
    super()
    this.state = {
      users: [],
      currentUser: null,
      existingIds: [],
      validationMessage: '',
      addUsingFacebook: true,
    }
  }

  componentDidMount() {
    const that = this
    get('/api/members/facebookids').then(response => {
      that.setState({
        existingIds: response,
      })
    })
  }

  addPlayer = user => {
    const state = this.state
    const validationMessage = this.validateUser(user)
    if (validationMessage) this.setState({ validationMessage })
    else if (!state.isSubmitting) {
      this.setState({ isSubmitting: true })
      post('/admin/spillerinvitasjon', {
        FirstName: user.first_name,
        MiddleName: user.middle_name,
        LastName: user.last_name,
        FacebookId: user.id,
        EmailAddress: user.email,
      }).then(data => {
        if (data.validationMessage) {
          this.setState({
            validationMessage: data.validationMessage,
            isSubmitting: false,
          })
        } else if (user.id) {
          this.setState({
            existingIds: state.existingIds.concat([user.id]),
            isSubmitting: false,
          })
        } else {
          this.setState({ validationMessage: '', isSubmitting: false })
          mt.alert('success', data.successMessage)
          this.clearEmailForm()
        }
      })
    }
  }

  clearEmailForm() {
    $('.add-players input').val('')
  }

  changeAddMethod = method => {
    if (method == 'facebook') this.setState({ addUsingFacebook: true })
    else this.setState({ addUsingFacebook: false })
  }

  validateUser = user => {
    if (user.id) return null

    if (user.first_name == '' || user.last_name == '' || user.email == '') {
      return 'Alle feltene må fylles ut'
    }

    function validateEmail(email) {
      const re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i
      return re.test(email)
    }

    if (!validateEmail(user.email)) return 'Ugyldig e-postadresse'
  }

  render() {
    const addWithFacebookClass = this.state.addUsingFacebook ? 'active' : ''
    const addWithEmailClass = this.state.addUsingFacebook ? '' : 'active'
    return (
      <div className="add-players">
        <h3>Legg til spillere</h3>

        <div className="col-md-12 col-lg-8 no-padding">
          <ul className="nav nav-pills mt-justified">
            <li className={addWithFacebookClass}>
              <a onClick={this.changeAddMethod.bind(null, 'facebook')}>
                <i className="fa fa-facebook" /> Med Facebook
              </a>
            </li>
            <li className={addWithEmailClass}>
              <a onClick={this.changeAddMethod.bind(null, 'email')}>
                <i className="fa fa-envelope" />&nbsp;Med e-post
              </a>
            </li>
          </ul>
          {this.renderAddModule()}
        </div>
      </div>
    )
  }
  renderAddModule = () => {
    if (this.state.addUsingFacebook) {
      return <FacebookAdd addPlayer={this.addPlayer} existingIds={this.state.existingIds} />
    }
    return <EmailAdd addPlayer={this.addPlayer} validationMessage={this.state.validationMessage} />
  }
}
