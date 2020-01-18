
app.factory('homePageService',
    [
        '$http', function($http) {

            return {
                demoFunc: function(homePage) {
                    return $http({
                        url: '/Country/CreateCountry',
                        method: 'POST',
                        data: homePage
                    });
                }
            };
        }
    ]);