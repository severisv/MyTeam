
var AddPlayers = React.createClass({

    routes: {
        ADD_PLAYER: "admin/addPlayer",
        GET_FACEBOOK_IDS: "admin/getFacebookIds",
    },

    getInitialState: function () {
        return ({
            users: [],
            currentUser: null,
            existingIds: [],
            validationMessage: "",
            addUsingFacebook: true

        })
    },

    componentDidMount() {
        var existingIds = $.getJSON(this.routes.GET_FACEBOOK_IDS).then(response => {
            this.setState({
                existingIds: response.data
            })
        }
        );
    },

    addPlayer: function (user) {
        var validationMessage = this.validateUser(user);
        console.log(user)

        if (validationMessage) this.setState({ validationMessage: validationMessage })
            
        else {
            $.post(this.routes.ADD_PLAYER, {
                firstname: user.first_name,
                lastname: user.last_name,
                facebookid: user.id,
                emailAddress: user.email,
                imageSmall: user.picture.data.url,          
                imageMedium: user.imageMedium,
                imageLarge: user.imageLarge             
            }).then(data => {
                if (data.SuccessMessage == "facebookAdd") {
                    var ids = this.state.existingIds;
                    ids.push(user.id)
                    this.setState(
                        {
                            existingIds: ids
                        })
                }
                else if (data.SuccessMessage) {
                    this.setState({ validationMessage: "" });
                    mt.alert("success", data.SuccessMessage);
                    this.clearEmailForm();
                }

                else if (data.ValidationMessage) {
                    this.setState({ validationMessage: data.ValidationMessage });
                }

            });
        }

    },

    clearEmailForm: function(){
        $('.add-players input').val('')
    },

    changeAddMethod: function (method) {
        if (method == "facebook") this.setState({ addUsingFacebook: true })
        else this.setState({ addUsingFacebook: false })
    },

    validateUser: function(user){
        if (user.id) return null;

        if (user.first_name == "" || user.last_name == "" || user.email == "") return "Alle feltene må fylles ut";

        function validateEmail(email) {
            var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
            return re.test(email);
        }
        if (!validateEmail(user.email)) return "Ugyldig e-postadresse"

    },
    render: function () {
        var addWithFacebookClass = this.state.addUsingFacebook ? "active" : "";
        var addWithEmailClass = this.state.addUsingFacebook ? "" : "active";
        return (<div className="add-players">
            <h3>Legg til spillere</h3>
                  

                    <div className="col-md-12 col-lg-8 no-padding">
            <ul className="nav nav-pills mt-justified">
                   <li className={addWithFacebookClass}><a onClick={this.changeAddMethod.bind(null, "facebook" )}><i className="fa fa-facebook"></i> Med Facebook</a></li>
                   <li className={addWithEmailClass}><a onClick={this.changeAddMethod.bind(null, "email" )}><i className="fa fa-envelope"></i>&nbsp;Med e-post</a></li>
            </ul>
            {this.renderAddModule()}
        </div>
        </div>
        )
    },
        renderAddModule: function () {
            if (this.state.addUsingFacebook)
                return (<FacebookAdd addPlayer={this.addPlayer} existingIds={this.state.existingIds} />)
            else 
            return (<EmailAdd addPlayer={this.addPlayer} validationMessage={this.state.validationMessage} />)

        }

});


