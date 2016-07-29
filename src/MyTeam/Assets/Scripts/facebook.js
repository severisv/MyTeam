var mt_fb = mt_fb || {};


mt_fb.aquireUserToken = function () {

    function saveToken () {
        var accessToken = FB.getAccessToken();
        mt_fb.accessToken = accessToken;
    }

    function aquireToken () {
        if (!mt_fb.isLoaded === true) {
            console.log("Facebook SDK not yet loaded .");
        }
        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                saveToken();
            }
            else {
                mt_fb.userIsUnavailable = true;
            }
        });
    }

    if (!mt_fb.accessToken) {
        aquireToken();
    }

    if (mt_fb.userIsUnavailable) {
        return null;
    }

    return mt_fb.accessToken;

};

function login () {
    FB.getLoginStatus(function (response) {
        if (response.status === 'connected') {
        }
        else {
            FB.login();
        }
    });
}

mt_fb.login = function () {
    if (!window.mt_fb.isLoaded) {
        setTimeout(function () {
            mt_fb.login();
        }, 50);
    } else {
        login();
    }
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

mt_fb.getUserUrl = function (id) {
    var accessToken = mt_fb.aquireUserToken();


    if (accessToken) {

        var url = "https://graph.facebook.com/v2.5/" + id;
        return {
            url: url,
            accessToken: accessToken
        };
    }
    return null;
}

mt_fb.getUserImageUrl = function (id) {
    var accessToken = mt_fb.aquireUserToken();
    if (accessToken) {
        var url = "https://graph.facebook.com/v2.5/" + id + "/picture";
        return {
            url: url,
            accessToken: accessToken
        };
    }
    return null;
};

module.exports = mt_fb;
