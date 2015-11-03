var checkbox = checkbox || {};

checkbox.showHideAssociatedElement = function (element, associatedSelector, reverse) {
    var show = element.checked;
    if (reverse) {
        show = !element.checked;
    }

    if (show) {
        $(associatedSelector).show();
    } else {
        $(associatedSelector).hide();
    }
};








/* Norwegian initialisation for the jQuery UI date picker plugin. */
/* Written by Naimdjon Takhirov (naimdjon@gmail.com). */

(function (factory) {
    if (typeof define === "function" && define.amd) {

        // AMD. Register as an anonymous module.
        define(["../datepicker"], factory);
    } else {

        // Browser globals
        factory(jQuery.datepicker);
    }
}(function (datepicker) {

    datepicker.regional['no'] = {
        closeText: 'Lukk',
        prevText: '&#xAB;Forrige',
        nextText: 'Neste&#xBB;',
        currentText: 'I dag',
        monthNames: ['januar', 'februar', 'mars', 'april', 'mai', 'juni', 'juli', 'august', 'september', 'oktober', 'november', 'desember'],
        monthNamesShort: ['jan', 'feb', 'mar', 'apr', 'mai', 'jun', 'jul', 'aug', 'sep', 'okt', 'nov', 'des'],
        dayNamesShort: ['søn', 'man', 'tir', 'ons', 'tor', 'fre', 'lør'],
        dayNames: ['søndag', 'mandag', 'tirsdag', 'onsdag', 'torsdag', 'fredag', 'lørdag'],
        dayNamesMin: ['sø', 'ma', 'ti', 'on', 'to', 'fr', 'lø'],
        weekHeader: 'Uke',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    datepicker.setDefaults(datepicker.regional['no']);

    return datepicker.regional['no'];

}));


var mt_fb = mt_fb || {};


mt_fb.aquireUserToken = function () {
    
    function saveToken() {
        var accessToken = FB.getAccessToken();
        mt_fb.accessToken = accessToken;
    }

    function aquireToken() {
        if (!mt_fb.isLoaded === true) {
            console.log("Facebook SDK not yet loaded .");
        }
        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                saveToken();
            } else {
                FB.login();
                saveToken();
            }
        });
    }

    if (!mt_fb.accessToken) {
        aquireToken();
    } 
    return mt_fb.accessToken;

};


mt_fb.getSearchUrl = function () {

    var accessToken = mt_fb.aquireUserToken();
    if (accessToken) {
        var url = "https://graph.facebook.com/v2.5/search" ;
        return {
            url: url,
            accessToken: accessToken
    };
    }
    return null;
};

mt_fb.getUserImageUrl = function(id) {
    var accessToken = mt_fb.aquireUserToken();
    if (accessToken) {
        var url = "https://graph.facebook.com/v2.5/"+id+"/picture";
        return {
            url: url,
            accessToken: accessToken
        };
    }
    return null;
}
var ANIMATION_DURATON = 300;
var global = global || {};

global.applyScopedJsComponents = function ($scope) {
    $scope.find('input.datepicker').datepicker();
    $scope.find('table.tablesorter').tablesorter();
    $scope.find('a.mt-popover').popover({ trigger: "hover" });
    applyConfirmDialogListeners($scope);
    applyActiveLinkSwapper($scope);
}

global.applyJsComponents = function() {
    var timestamp = new Date();
    var start = timestamp.getMilliseconds();

    this.applyScopedJsComponents($(document));
    applySlideDownMenuListeners();
    

    window.onpopstate = function () {
        location.reload();
    }
    console.log("global.js: " + (new Date().getMilliseconds() - start) + "ms");
}


global.applyJsComponents();



// Slide down
function applySlideDownMenuListeners() {
    var element = $('.slide-down-parent');
    var subMenu = $(element.data('submenu'));

    element.click(function () {

        if (element.data('isFocused') == true) {
            element.data('isFocused', false);
            element.blur();
        } else {
            element.data('isFocused', true);
        }
    });

    element.focusin(function () {
        subMenu.slideDown(ANIMATION_DURATON);

    });

    element.focusout(function () {
        subMenu.slideUp(ANIMATION_DURATON);
        element.data('isFocused', false);
        setTimeout(function() {

        }, ANIMATION_DURATON);


    });
}

// Confirm dialog
function applyConfirmDialogListeners($scope) {
    $scope.find('a.confirm-dialog').click(function (e) {
        e.preventDefault();
        var element = $(this);
        var message = element.attr('data-message');

        bootbox.confirm(message, function (result) {
            if (result === true) {
                window.location = element.attr('href');
            }
        });

    });
}

// Active links
function applyActiveLinkSwapper($scope) {
    $scope.find('ul.nav li').on('click', function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
    });
}



var layout = layout || {};

layout.setPageName = function (text) {
    $('.mt-page-header').find('h1').html(text);
};

layout.pushState = function (key, value, title) {
    var uri = window.location.href;
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        uri = uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        uri = uri + separator + key + "=" + value;
    }

    history.pushState('', title, uri);
    layout.setPageName(title);
};








var mt = mt || {};

mt.deleteWithAjax = function(selector) {
    var element = $(selector);
    var href = element.data('href');
    $.post(href, function (result) {
        
        $('#' + element.data('parent')).fadeOut(300, function() { $(this).remove(); });
        $('#alerts').html(result);
        $('.alert').effect("highlight", { }, 500 );
    });
}


mt.alert = function(type, message) {
    $('#' + type).removeClass("hidden");
    $('#' + type + " .alert-content").html(message);
    $('.alert').effect("highlight", {}, 500);
}

mt.showElement = function(selector) {
    $(selector).show();
}
mt.hideElement = function(selector) {
    $(selector).hide();
}

var AddPlayers = React.createClass({displayName: "AddPlayers",

   

    getInitialState: function () {
        return ({
            users: [],
            currentUser: null,
            existingIds: [],
            validationMessage: "",
            addUsingFacebook: true

        })
    },

    componentWillMount(){
        this.routes = {
                ADD_PLAYER: Routes.ADD_PLAYER,
                GET_FACEBOOK_IDS: Routes.GET_FACEBOOK_IDS,
                }
    },

    componentDidMount() {
        $.getJSON(this.routes.GET_FACEBOOK_IDS).then(response => {
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
        return (React.createElement("div", {className: "add-players"}, 
            React.createElement("h3", null, "Legg til spillere"), 
                  

                    React.createElement("div", {className: "col-md-12 col-lg-8 no-padding"}, 
            React.createElement("ul", {className: "nav nav-pills mt-justified"}, 
                   React.createElement("li", {className: addWithFacebookClass}, React.createElement("a", {onClick: this.changeAddMethod.bind(null, "facebook")}, React.createElement("i", {className: "fa fa-facebook"}), " Med Facebook")), 
                   React.createElement("li", {className: addWithEmailClass}, React.createElement("a", {onClick: this.changeAddMethod.bind(null, "email")}, React.createElement("i", {className: "fa fa-envelope"}), " Med e-post"))
            ), 
            this.renderAddModule()
        )
        )
        )
    },
        renderAddModule: function () {
            if (this.state.addUsingFacebook)
                return (React.createElement(FacebookAdd, {addPlayer: this.addPlayer, existingIds: this.state.existingIds}))
            else 
            return (React.createElement(EmailAdd, {addPlayer: this.addPlayer, validationMessage: this.state.validationMessage}))

        }

});




var ManagePlayers = React.createClass({displayName: "ManagePlayers",
         
    getInitialState: function () {
        return ({
            players: []
        })
    },

    componentWillMount: function(){
      this.routes = Routes,
      this.options = {
          playerStatus: PlayerStatus,
          playerRoles: PlayerRoles
      }
    },

    componentDidMount() {

        $.getJSON(this.routes.GET_PLAYERS).then(response => {
            this.setState({
                players: response
            })
        }
        );
    },

    togglePlayerRole: function(player, role){
        $.post(this.routes.TOGGLE_PLAYER_ROLE, { Id: player.Id, Role: role }).then(response => {
            if (response.Success) {
                var players = this.state.players;
                for (var i in this.state.players) {
                    if (players[i].Id == player.Id) {
                        var index = players[i].Roles.indexOf(role);
                        if (index > -1) {
                            this.state.players[i].Roles.splice(index, 1);
                        }
                        else {
                            this.state.players[i].Roles.push(role);
                        }
                    }
                }
            }
            this.forceUpdate();
        }
    );
    },
    setPlayerStatus: function (player, status) {
   
        $.post(this.routes.SET_PLAYER_STATUS, { Id: player.Id, Status: status}).then(response => {
            if (response.Success) {
                var players = this.state.players;
                for (var i in this.state.players) {                    
                    if (players[i].Id == player.Id) {
                        this.state.players[i].Status = status;
                    }
                }
            }
            this.forceUpdate();
        }
      );

    },

    renderPlayers: function (playerStatus) {
        if (!this.state.players) return "";

        var players = this.state.players.filter(function (player) {
            if (player.Status == playerStatus) {
                return (player)
            }
        })

        var options = this.options;
        var routes = this.routes;
        var setPlayerStatus = this.setPlayerStatus;
        var togglePlayerRole = this.togglePlayerRole;
        var playerElements = players.map(function (player, i) {
            return (React.createElement(ManagePlayer, {key: player.Id, player: player, setPlayerStatus: setPlayerStatus, togglePlayerRole: togglePlayerRole, options: options, routes: routes}))
        })

        return (React.createElement("div", {className: "manage-players"}, 
    React.createElement("div", {className: "row"}, 
        React.createElement("div", {className: "col-xs-4 headline"}, React.createElement("strong", null, playerStatus)), 
        React.createElement("div", {className: "col-xs-3 subheadline hidden-xs"}, React.createElement("strong", null, "Status")), 
        React.createElement("div", {className: "col-xs-5 subheadline hidden-xs"}, React.createElement("strong", null, "Roller"))
    ), 
    React.createElement("div", null, 
        playerElements
    )
        ))
    },

    render: function () {
        return (React.createElement("div", null, 
            this.renderPlayers(this.options.playerStatus.Active), 
            this.renderPlayers(this.options.playerStatus.Veteran), 
            this.renderPlayers(this.options.playerStatus.Inactive)
        ))
    }

});

var EmailAdd = React.createClass({displayName: "EmailAdd",
   
    getInitialState: function () {
        return { firstname: '', lastname: '', email: '' };
    },
    
    submit: function(){
        var user = {        
            first_name: this.state.firstname,
            last_name: this.state.lastname,
            email: this.state.email            
        }
        this.props.addPlayer(user);
    },

    handleFirstnameChange : function(){ this.setState({firstname: event.target.value}) },
    handleLastnameChange : function(){ this.setState({lastname: event.target.value}) },
    handleEmailChange : function(){ this.setState({email: event.target.value}) },

    render: function () {
        return (
                  React.createElement("div", null, 
                       React.createElement("span", {className: "text-danger"}, this.props.validationMessage), 
                      React.createElement("input", {onChange: this.handleEmailChange, className: "form-control", placeholder: "E-postadresse"}), 
                      React.createElement("input", {onChange: this.handleFirstnameChange, className: "form-control", placeholder: "Fornavn"}), 
                      React.createElement("input", {onChange: this.handleLastnameChange, className: "form-control", placeholder: "Etternavn"}), 
                      React.createElement("button", {onClick: this.submit, className: "btn btn-primary"}, React.createElement("i", {className: "fa fa-plus"}), " Legg til")
                  ))
    }
   
        

});



var FacebookAdd = React.createClass({displayName: "FacebookAdd",

    getInitialState: function () {
        return ({
            users: []
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


            return (React.createElement("li", {key: user.id}, 
            React.createElement("img", {src: user.picture.data.url}), " ", user.name, 
                    React.createElement("button", {onClick: addPlayer.bind(null, user), className: buttonClass, disabled: disabled}, React.createElement("i", {className: icon}), " ", buttonText)
            ))
        })
    },


    render: function () {
        return (
        React.createElement("div", null, 
            React.createElement("div", {className: "col-xs-12 no-padding"}, 
             React.createElement("input", {className: "form-control search", placeholder: "Søk etter personer", type: "text", onChange: this.handleChange}), 
          React.createElement("i", {className: "fa fa-search search-icon"})
               ), 
          React.createElement("div", {className: "clearfix"}), 
          React.createElement("div", {className: "list-users"}, 
          React.createElement("ul", null, this.renderUsers(this.addPlayer, this.props.existingIds))
          )
        )
    )
    },

});


var ManagePlayer = React.createClass({displayName: "ManagePlayer",
    getInitialState: function () {
        return ({
            player: this.props.player
        })
    },

    setPlayerStatus: function(event){
        this.props.setPlayerStatus(this.state.player, event.target.value)
    },

    togglePlayerRole: function(role){
        this.props.togglePlayerRole(this.state.player, role)
    },

    renderStatusOptions: function () {

        var statuses = this.props.options.playerStatus;
        var statusList = [];
        for (var key in statuses) {
            statusList.push(React.createElement("option", {key: statuses[key]}, statuses[key]))
        }

        return (
            React.createElement("select", {value: this.state.player.Status, className: "form-control", onChange: this.setPlayerStatus}, 
               statusList
           )
            
        )
    },

        renderRoles: function () {

        var roles = this.props.options.playerRoles;
        var player = this.state.player;
        var buttons = [];
        for (var key in roles) {
            var role = roles[key];
            var buttonClass = "btn";
            if (player.Roles.indexOf(role) > -1) buttonClass += " btn-primary";
            else buttonClass += " btn-default";
            buttons.push(React.createElement("button", {onClick: this.togglePlayerRole.bind(null, role), key: player.Id+role, className: buttonClass}, role))
        }

        return (
            React.createElement("span", null, 
               buttons
           )
            
        )
    },

    render: function () {

        var player = this.state.player;
        var editPlayerHref = this.props.routes.EDIT_PLAYER + "&playerId=" + player.Id;
        return (React.createElement("div", {className: "row list-player"}, 
               React.createElement("div", {className: " col-sm-4 mp-name"}, player.FullName), 
               React.createElement("div", {className: "col-sm-3 mp-status"}, this.renderStatusOptions()), 
               React.createElement("div", {className: "col-sm-5"}, this.renderRoles(), 
                    React.createElement("a", {className: "btn btn-default pull-right", title: "Rediger spiller", href: editPlayerHref}, React.createElement("i", {className: "fa fa-edit"}))
                )
        ))
    }


})