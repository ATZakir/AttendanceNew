app.filter('startFrom', function () {
    return function (input, start) {
        if (input) {
            start = +start; //parse to int
            return input.slice(start);
        }
        return [];
    }
});
app.factory('focus', function ($rootScope, $timeout) {
    return function (name) {
        $timeout(function () {
            $rootScope.$broadcast('focusOn', name);
        });
    };
});
app.factory('playerService', ['$http', function ($http) {

    return {
        savePlayer: function (player) {
            var fdata = new FormData();
            var url = '/Player/CreatePlayer';
            for (var key in player) {
                if (player.hasOwnProperty(key)) {
                    fdata.append(key, player[key] ? player[key] : '');
                }
            }

            return $http.post(url,
                fdata,
                {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                });
        },
        getPlayer: function (id) {
            return $http.get('/Player/GetPlayer/' + id);
        },
        getAllPlayer: function () {
            return $http.get('/Player/GetPlayerList');
        },
        deletePlayer: function (player) {
            return $http({
                url: '/Player/DeletePlayer',
                method: 'POST',
                data: player
            });
        },
        getAllPlayerType: function () {
            return $http.get('/PlayerType/GetPlayerTypeList');
        },
        getAllCountry: function () {
            return $http.get('/Country/GetCountryList');
        }
    };
}]);