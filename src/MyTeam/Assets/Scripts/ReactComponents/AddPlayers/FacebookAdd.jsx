var FacebookAdd = React.createClass({

    getInitialState: function () {
        return ({
            users: []
        });
    },


    setUsersAsync: function (q) {

        var url = mt_fb.getSearchUrl();
        if (url) {
            var that = this;
            $.getJSON(url.url, {
                q: q,
                type: "user",
                access_token: url.accessToken,
                limit: 10,
                fields: "picture,name,first_name,last_name,middle_name"
            }).then(function (data) {
                that.setState({
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

    addPlayer: function(user){
                
        function getImageUrl(url, size) {
            var imageUrl;
            $.ajax({
                url: url.url,
                async: false,
                data: {
                    access_token: url.accessToken,
                    height: size,
                    width: size,
                    redirect: 0
                },
                success: function (response) {
                    imageUrl = response.data.url;
                }

            });
            return imageUrl;
        }                  

        var url = mt_fb.getUserImageUrl(user.id);
        
        user.imageMedium = getImageUrl(url, 400);
        user.imageLarge = getImageUrl(url, 800);

        this.props.addPlayer(user);
    },

    renderUsers: function (addPlayer, existingIds) {
        return this.state.users.map(function(user) {

            var disabled = false;
            var icon = "";
            var buttonClass = "pull-right btn";
            var buttonText = ""
            if (existingIds.indexOf(user.id) > -1) {
                buttonClass += " btn-success"
                icon = "fa fa-check";
                disabled = true;
                buttonText = "Lagt til"
            } else {
                buttonClass += " btn-primary"
                icon = "fa fa-plus"
                buttonText = "Legg til"
            }


            return (<li key={user.id}>
            <img src={user.picture.data.url} /> {user.name}
                    <button onClick={addPlayer.bind(null, user)} className={buttonClass} disabled={disabled}><i className={icon}></i> {buttonText}</button>
            </li>);
        });
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
          <ul>{this.renderUsers(this.addPlayer, this.props.existingIds)}</ul>
          </div>
        </div>
        );
    }

});
