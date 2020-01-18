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

app.factory('countryService',
    [
        '$http', function ($http) {

            return {
                saveCountry: function (country) {
                    return $http({
                        url: '/api/Country2',
                        method: 'POST',
                        data: country
                    });
                },
                getCountry: function (id) {
                    return $http.get('/api/Country2/' + id);
                },
                getAllCountry: function () {
                    return $http.get('/api/Country2');
                },
                deleteCountry: function (id) {
                    return $http({
                        url: '/api/Country2/' + id,
                        method: 'DELETE'
                    });
                }
            };
        }
    ]);
