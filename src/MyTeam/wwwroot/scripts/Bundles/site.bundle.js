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


var timestamp = new Date();
var start = timestamp.getMilliseconds();

var ANIMATION_DURATON = 300; 

$('input.datepicker').datepicker();
$('table.tablesorter').tablesorter();
$('a.mt-popover').popover({ trigger: "hover" });

applySlideDownMenuListeners();
applyConfirmDialogListeners();



window.onpopstate = function () {
    location.reload();
}


console.log("global.js: " + (new Date().getMilliseconds() - start) + "ms");



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
function applyConfirmDialogListeners() {
    $('a.confirm-dialog').click(function (e) {
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


var AddPlayers = React.createClass({displayName: "AddPlayers",

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
        return (React.createElement("div", {className: "add-players"}, 
            React.createElement("h3", null, "Legg til spillere"), 
            React.createElement(FacebookAdd, {addPlayer: this.addPlayer, existingIds: this.state.existingIds})
        )
        )
    }
});


var FacebookAdd = React.createClass({displayName: "FacebookAdd",

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
        React.createElement("ul", null, this.renderUsers(this.props.addPlayer, this.props.existingIds))
        
        )
        )
    )
    }
});
