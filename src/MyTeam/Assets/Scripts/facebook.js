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

