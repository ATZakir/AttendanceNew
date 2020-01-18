app.controller('userHomepageCtrl', function ($scope, $timeout, focus, $anchorScroll, userHomepageService, $modal, $log, $ngBootbox) {
    $scope.init = function (permission) {
        if (permission === "NO") {
            $scope.NoPermission = true;
        }
    };
    $scope.messageModalObj = {};
    $scope.messageModalObj.message = '';
    loadAllTournament();
    $scope.showModalforSearch = false;



    // Load employee
    function loadAllTournament() {
        $scope.allTournamentFromDb = [];
        userHomepageService.getAllTournament()
            .then(function onSuccess(response) {
                $scope.allTournamentFromDb = response.data;
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Error Data Loading');
            });
    }

    $scope.manageTeam = function(tournamentId) {
        $window.location.assign("/Team/ManageTeam/" + tournamentId);
    };


});

