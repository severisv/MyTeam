const React = require("react");

module.exports = React.createClass({
  getInitialState() {
    return {
      users: [],
      currentUser: null,
      existingIds: [],
      validationMessage: "",
      addUsingFacebook: true
    };
  },

  componentWillMount() {
    this.routes = {
      ADD_PLAYER: Routes.ADD_PLAYER
    };
  },

  componentDidMount() {
    const that = this;
    $.getJSON("/api/members/facebookids").then(response => {
      that.setState({
        existingIds: response
      });
    });
  },

  addPlayer(user) {
    const validationMessage = this.validateUser(user);
    if (validationMessage) this.setState({ validationMessage });
    else if (!this.state.isSubmitting) {
      const that = this;
      that.setState({ isSubmitting: true });
      $.post(that.routes.ADD_PLAYER, {
        firstname: user.first_name,
        middlename: user.middle_name,
        lastname: user.last_name,
        facebookid: user.id,
        emailAddress: user.email
      }).then(data => {
        if (data.successMessage == "facebookAdd") {
          const ids = that.state.existingIds;
          ids.push(user.id);
          that.setState({
            existingIds: ids,
            isSubmitting: false
          });
        } else if (data.successMessage) {
          that.setState({ validationMessage: "", isSubmitting: false });
          mt.alert("success", data.successMessage);
          that.clearEmailForm();
        } else if (data.validationMessage) {
          that.setState({
            validationMessage: data.validationMessage,
            isSubmitting: false
          });
        }
      });
    }
  },

  clearEmailForm() {
    $(".add-players input").val("");
  },

  changeAddMethod(method) {
    if (method == "facebook") this.setState({ addUsingFacebook: true });
    else this.setState({ addUsingFacebook: false });
  },

  validateUser(user) {
    if (user.id) return null;

    if (user.first_name == "" || user.last_name == "" || user.email == "") {
      return "Alle feltene må fylles ut";
    }

    function validateEmail(email) {
      const re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
      return re.test(email);
    }

    if (!validateEmail(user.email)) return "Ugyldig e-postadresse";
  },
  render() {
    const addWithFacebookClass = this.state.addUsingFacebook ? "active" : "";
    const addWithEmailClass = this.state.addUsingFacebook ? "" : "active";
    return (
      <div className="add-players">
        <h3>Legg til spillere</h3>

        <div className="col-md-12 col-lg-8 no-padding">
          <ul className="nav nav-pills mt-justified">
            <li className={addWithFacebookClass}>
              <a onClick={this.changeAddMethod.bind(null, "facebook")}>
                <i className="fa fa-facebook" /> Med Facebook
              </a>
            </li>
            <li className={addWithEmailClass}>
              <a onClick={this.changeAddMethod.bind(null, "email")}>
                <i className="fa fa-envelope" />&nbsp;Med e-post
              </a>
            </li>
          </ul>
          {this.renderAddModule()}
        </div>
      </div>
    );
  },
  renderAddModule() {
    if (this.state.addUsingFacebook) {
      return (
        <FacebookAdd
          addPlayer={this.addPlayer}
          existingIds={this.state.existingIds}
        />
      );
    }
    return (
      <EmailAdd
        addPlayer={this.addPlayer}
        validationMessage={this.state.validationMessage}
      />
    );
  }
});
