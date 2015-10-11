
var AddPlayers = React.createClass({

    views: {
        FACEBOOK_ADD: "FACEBOOK_ADD",
        EMAIL_ADD: "EMAIL_ADD",
        CONFIRM: "CONFIRM",
    },

    routes : {
        ADD_PLAYER: "admin/addPlayer"
    },

    getInitialState: function () {
        return ({
            users: [],
            currentUser: null,
            view: this.views.FACEBOOK_ADD,
            existingIds: []
        })
    },

    componentDidMount(){
        //var existingIds = $.getJSON("getFacebookIds").then(response => {
        //    this.setState({
        //        existingIds: response.data
        //    })
        //}
        //    );
    },
    
    addPlayer: function (user) {
        $.post(this.routes.ADD_PLAYER, {
            firstname: user.first_name,
            lastname: user.last_name,
            facebookid: user.id
        }).then(data => {
            if (data.success) {
                var ids = this.state.existingIds;
                ids.push(user.id)
                console.log(ids)
                this.setState(
                    {
                        existingIds: ids
                    })
            }
        });      

    },
    
    render: function () {
        return (<div className="add-players">
            <h3>Legg til spillere</h3>
            <FacebookAdd addPlayer={this.addPlayer} existingIds={this.state.existingIds} />
        </div>
        )
    }
});


var FacebookAdd = React.createClass({

    getInitialState: function () {
        return ({
            users: [],
        })
    },


    setUsersAsync: function (q) {

        var url = mt_fb.getSearchUrl();
        if (url) {
            $.getJSON(url.url, {
                q: q,
                type: "user",
                access_token: url.accessToken,
                limit: 6,
                fields: "picture,name,first_name,last_name"
            }).then(data => {
                this.setState({
                    users: data.data
                });
            });
        }
    },

    handleChange: function (event) {
        var q = event.target.value;
        if (q) {
            this.setUsersAsync(q);
        }
    },

    renderUsers: function (addPlayer, existingIds) {
        return this.state.users.map(function (user) {

            var disabled = false;
            var icon = "";
            var buttonClass = "pull-right btn";
            var buttonText = ""
            if (existingIds.indexOf(user.id) > -1) {
                buttonClass += " btn-success"
                icon = "fa fa-check";
                disabled = true;
                buttonText = "Lagt til"
            }
            else {
                buttonClass += " btn-primary"
                icon = "fa fa-plus"
                buttonText = "Legg til"
            }


            return (<li key={user.id}>
            <img src={user.picture.data.url} /> {user.name}
                    <button onClick={addPlayer.bind(null, user)} className={buttonClass} disabled={disabled}><i className={icon}></i> {buttonText}</button>
            </li>)
        })
    },

    render: function () {
        return (
        <div>
             <div className="col-xs-12 no-padding">
        <input className="form-control search" placeholder="Søk etter personer" type="text" onChange={this.handleChange} />
        <i className="fa fa-search search-icon"></i>

        </div>
        <div className="clearfix"></div>
        <div className="list-users">
        <ul>{this.renderUsers(this.props.addPlayer, this.props.existingIds)}</ul>
        
        </div>
        </div>
    )
    }
});
