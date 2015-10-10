
var AddPlayers = React.createClass({
    
    getInitialState: function(){
        return({
            users: []
        })
    },

    requestUsers: function (q) {
        
      
        var users = mt_fb.queryUsers(q)

        console.log(users)
        return []
    },

    setUsersAsync: function (q) {

        var url = mt_fb.getSearchUrl();
        if (url) {
            $.getJSON(url.url, {
                q: q,
                type: "user",
                access_token: url.accessToken,
                limit: 5,
                fields: "picture,name"
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

    renderUsers:function(){
        return this.state.users.map(function (user) {
            return (<li key={user.id}><img src={user.picture.data.url} /> {user.name}</li>)
        })
    },

    render:function() {
        return (<div>
    <input type="text" onChange={this.handleChange} />
    <ul>{this.renderUsers()}</ul>
    </div>
        )
    }
});

