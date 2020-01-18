(function () {

    "use strict";




    FB.init({
        appId: fbId,
        cookie: true,
        status: true,
        xfbml: true
    });

    fbFunc = function fbLeagueInvite(leagueId, leagueName, passCode) {
        FB.ui({
            method: 'share',
            quote: "Please join " + leagueName + ". The Pass code for this league is: " + passCode,
            href: window.location.protocol + "//" + window.location.hostname + "/League/LeagueJoin/" + leagueId
        },
            function (response) {
                if (response) {
                    alert('Successfully shared');
                } else {
                    alert('Failed To share');
                }
            });
    };

    fbFuncPlain = function fbLeagueInvitePlain() { 
        FB.ui({
            method: 'share',
            quote: "Please join and play Super 11!",
            href: window.location.protocol + "//" + window.location.hostname +"/Home"
        },
            function (response) {
                if (response) {
                    alert('Successfully shared');
                } else {
                    alert('Failed To share');
                }
            });
    };

})();
