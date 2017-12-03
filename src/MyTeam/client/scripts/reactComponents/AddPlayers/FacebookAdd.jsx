const facebook = require('../../facebook.js')
const React = require('react')

module.exports = React.createClass({
  getInitialState() {
    return {
      users: [],
    }
  },

  setUsersAsync(q) {
    const url = facebook.getSearchUrl()
    if (url) {
      const that = this
      $.getJSON(url.url, {
        q,
        type: 'user',
        access_token: url.accessToken,
        limit: 10,
        fields: 'picture,name,first_name,last_name,middle_name',
      }).then((data) => {
        that.setState({
          users: data.data,
        })
      })
    }
  },

  handleChange(event) {
    const q = event.target.value
    if (q) {
      this.setUsersAsync(q)
    }
  },

  addPlayer(user) {
    function getImageUrl(url, size) {
      let imageUrl
      $.ajax({
        url: url.url,
        async: false,
        data: {
          access_token: url.accessToken,
          height: size,
          width: size,
          redirect: 0,
        },
        success(response) {
          imageUrl = response.data.url
        },
      })
      return imageUrl
    }

    const url = facebook.getUserImageUrl(user.id)

    user.imageMedium = getImageUrl(url, 400)
    user.imageLarge = getImageUrl(url, 800)

    this.props.addPlayer(user)
  },

  renderUsers(addPlayer, existingIds) {
    return this.state.users.map((user) => {
      let disabled = false
      let icon = ''
      let buttonClass = 'pull-right btn'
      let buttonText = ''
      if (existingIds.indexOf(user.id) > -1) {
        buttonClass += ' btn-success'
        icon = 'fa fa-check'
        disabled = true
        buttonText = 'Lagt til'
      } else {
        buttonClass += ' btn-primary'
        icon = 'fa fa-plus'
        buttonText = 'Legg til'
      }

      return (
        <li key={user.id}>
          <img src={user.picture.data.url} /> {user.name}
          <button onClick={addPlayer.bind(null, user)} className={buttonClass} disabled={disabled}>
            <i className={icon} /> {buttonText}
          </button>
        </li>
      )
    })
  },

  render() {
    return (
      <div>
        <div className="col-xs-12 no-padding">
          <input
            className="form-control search"
            placeholder="Søk etter personer"
            type="text"
            onChange={this.handleChange}
          />
          <i className="fa fa-search search-icon" />
        </div>
        <div className="clearfix" />
        <div className="list-users">
          <ul>{this.renderUsers(this.addPlayer, this.props.existingIds)}</ul>
        </div>
      </div>
    )
  },
})
