app.filter('startFrom',
    function () {
        return function (input, start) {
            if (input) {
                start = +start; //parse to int
                return input.slice(start);
            }
            return [];
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

app.factory('userHomepageService',
    [
        '$http', function ($http) {

            return {
                saveCountry: function (country) {
                    return $http({
                        url: '/Country/CreateCountry',
                        method: 'POST',
                        data: country
                    });
                },
                getCountry: function (id) {
                    return $http.get('/Country/GetCountry/' + id);
                },
                getAllTournament: function () {
                    return $http.get('/Tournament/GetTournamentList');
                },
                deleteCountry: function (country) {
                    return $http({
                        url: '/Country/DeleteCountry',
                        method: 'POST',
                        data: country
                    });
                }
            };
        }
    ]);
