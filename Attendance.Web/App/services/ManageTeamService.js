app.filter('playerListFilter',
    function () {
        return function(items, playerType, team) {

            if (playerType) {
                if (playerType === 1) {
                    items = items.filter(function(element) {
                        return element.PlayerTypeId === playerType || element.PlayerTypeId === 8;
                    });
                } else if (playerType === 4) {
                    items = items.filter(function(element) {
                        return element.PlayerTypeId === playerType ||
                            element.PlayerTypeId === 6 ||
                            element.PlayerTypeId === 7;
                    });
                } else {
                    items = items.filter(function(element) {
                        return element.PlayerTypeId === playerType;
                    });
                }
            }

            if (team) {
                items = items.filter(function(element) {
                    return element.TeamId === team;
                });
            }
            return items;
        };

    });

app.factory('focus',
    function ($rootScope, $timeout) {
        return function (name) {
            $timeout(function () {
                $rootScope.$broadcast('focusOn', name);
            });
        };
    });
app.factory('manageTeamService',
    [
        '$http', function ($http) {

            return {
                saveMyTeam: function (myTeam) {
                    return $http({
                        url: '/Team/CreateUserTeam',
                        method: 'POST',
                        data: myTeam
                    });
                },
                getCity: function (id) {
                    return $http.get('/City/GetCity/' + id);
                },
                getAllCity: function () {
                    return $http.get('/City/GetCityList');
                },
                getAllCountry: function () {
                    return $http.get('/Country/GetCountryList');
                },
                deleteCity: function (city) {
                    return $http({
                        url: '/City/DeleteCity',
                        method: 'POST',
                        data: city
                    });
                },

                getAllTeamPlayerListByTournamentId: function (id) {
                    return $http.get('/TeamPlayer/GetTeamPlayerListByTournamentId?tournamentId=' + id);
                },
                getAllTournamentTeamList: function (tournamentId) {
                    return $http.get('/Team/GetTournamentTeamList?tournamentId=' + tournamentId);
                },
                getTeamFormationRule: function (tournamentId) {
                    return $http.get('/TeamFormationRule/GetTeamFormationRuleByTournamentId/' + tournamentId);
                }
            };
        }
    ]);