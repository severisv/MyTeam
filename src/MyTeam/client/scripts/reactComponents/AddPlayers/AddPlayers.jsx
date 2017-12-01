var React = require('react');

module.exports = React.createClass({

    getInitialState: function () {
        return ({
            users: [],
            currentUser: null,
            existingIds: [],
            validationMessage: "",
            addUsingFacebook: true

        });
    },

    componentWillMount: function () {
        this.routes = {
            ADD_PLAYER: Routes.ADD_PLAYER,
            GET_FACEBOOK_IDS: Routes.GET_FACEBOOK_IDS,
        }
    },

    componentDidMount: function () {
        var that = this;
        $.getJSON(that.routes.GET_FACEBOOK_IDS).then(function (response) {
            that.setState({
                existingIds: response.data
            });
        });
    },

    addPlayer: function (user) {
        var validationMessage = this.validateUser(user);
        if (validationMessage) this.setState({ validationMessage: validationMessage });

        else if (!this.state.isSubmitting) {
            var that = this;
            that.setState( { isSubmitting: true } );
            $.post(that.routes.ADD_PLAYER, {
                firstname: user.first_name,
                middlename: user.middle_name,
                lastname: user.last_name,
                facebookid: user.id,
                emailAddress: user.email
            }).then(function (data) {
                if (data.successMessage == "facebookAdd") {
                    var ids = that.state.existingIds;
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
                    that.setState({ validationMessage: data.validationMessage, isSubmitting: false });
                }
            });
        }

    },

    clearEmailForm: function () {
        $('.add-players input').val('');
    },

    changeAddMethod: function (method) {
        if (method == "facebook") this.setState({ addUsingFacebook: true });
        else this.setState({ addUsingFacebook: false });
    },

    validateUser: function (user) {
        if (user.id) return null;

        if (user.first_name == "" || user.last_name == "" || user.email == "") return "Alle feltene må fylles ut";

        function validateEmail(email) {
            var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
            return re.test(email);
        }

        if (!validateEmail(user.email)) return "Ugyldig e-postadresse";

    },
    render: function () {
        var addWithFacebookClass = this.state.addUsingFacebook ? "active" : "";
        var addWithEmailClass = this.state.addUsingFacebook ? "" : "active";
        return (<div className="add-players">
            <h3>Legg til spillere</h3>


                    <div className="col-md-12 col-lg-8 no-padding">
            <ul className="nav nav-pills mt-justified">
                   <li className={addWithFacebookClass}><a onClick={this.changeAddMethod.bind(null, "facebook")}><i className="fa fa-facebook"></i> Med Facebook</a></li>
                   <li className={addWithEmailClass}><a onClick={this.changeAddMethod.bind(null, "email")}><i className="fa fa-envelope"></i>&nbsp;Med e-post</a></li>
            </ul>
                        {this.renderAddModule()}
                    </div>
        </div>
        );
    },
    renderAddModule: function () {
        if (this.state.addUsingFacebook)
            return (<FacebookAdd addPlayer={this.addPlayer} existingIds={this.state.existingIds } />);
        else
            return (<EmailAdd addPlayer={this.addPlayer} validationMessage={this.state.validationMessage } />);

    }

});
