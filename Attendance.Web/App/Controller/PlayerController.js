app.controller('playerCtrl', function ($scope, $timeout, $http, focus, $location, $anchorScroll, playerService, $log, $ngBootbox) {
    if ('@ViewBag.permission' == "NO") {
        $scope.NoPermission = true;
    }
    $scope.messageModalObj = {};
    $scope.messageModalObj.message = '';
    $scope.Photo = "defaultPlayer.jpg";

    loadAllPlayer();
    loadAllPlayerType();
    loadAllCountry();

    $scope.showModalforSearch = false;

    // Add a new player
    $scope.addPlayer = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.form.$valid) {
            var player = {};
            player["Id"] = $scope.Id;
            player["Name"] = $scope.Name;
            player["CountryId"] = $scope.CountryId;
            player["PlayerTypeId"] = $scope.PlayerTypeId;

            player["TestRanking"] = $scope.TestRanking;
            player["ODIRanking"] = $scope.ODIRanking;
            player["T20IRanking"] = $scope.T20IRanking;
            player["Photo"] = $scope.Photo;

            if ($scope.file != null) {
                player["file"] = $scope.file
            }

            playerService.savePlayer(player)
                .then(function onSuccess(response) {
                    loadAllPlayer();
                    $scope.messageModalObj.message = response.data.message;
                    $scope.showModalforSearch = true;
                    $timeout(function () { $scope.showModalforSearch = false; }, 3000);
                    $scope.reset();

                })
                .catch(function onError(response) {
                    $ngBootbox.alert('Error in Saving Data');
                });
        }
    };

    // Populate player
    $scope.selectedRow = null;
    $scope.populatePlayer = function (rowvalue) {
        $scope.selectedRow = rowvalue;
        $scope.Id = rowvalue.Id;
        $scope.Name = rowvalue.Name;
        $scope.CountryId = rowvalue.CountryId;
        $scope.PlayerTypeId = rowvalue.PlayerTypeId;
        $scope.file = null;
        $scope.Photo = rowvalue.Photo;
        $scope.TestRanking = rowvalue.TestRanking;
        $scope.ODIRanking = rowvalue.ODIRanking;
        $scope.T20IRanking = rowvalue.T20IRanking;

        $location.hash('searchDivId');
        $anchorScroll();
        focus('focusMe');
        $scope.desableIdField = true;
    }

    $scope.reset = function () {
        $scope.$broadcast('show-errors-reset');
        $scope.selectedRow = null;
        $scope.Id = '';
        $scope.Name = '';
        $scope.CountryId = null;
        $scope.PlayerTypeId = null;
        $scope.TestRanking = null;
        $scope.ODIRanking = null;
        $scope.T20IRanking = null;
        $scope.file = null;
        $scope.Photo = "defaultPlayer.jpg";

        angular.element("input[type='file']").val(null);
        $('#imgshowField').attr('src', "");
    }

    $scope.deletePlayer = function (player) {
        playerService.deletePlayer(player)
            .then(function onSuccess(response) {
                $scope.messageModalObj.message = response.data.message;
                $scope.showModalforSearch = true;
                $timeout(function () { $scope.showModalforSearch = false; }, 3000);
                $scope.reset();
                loadAllPlayer();
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Can not be deleted');
            });
    }
    // Load Player
    function loadAllPlayer() {
        $scope.allPlayerFromDb = [];
        playerService.getAllPlayer()
            .then(function onSuccess(response) {
                $scope.allPlayerFromDb = response.data;
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Error in Data Loading');
            });
    }
    // Load Player
    function loadAllPlayerType() {
        $scope.allPlayerTypeFromDb = [];
        playerService.getAllPlayerType()
            .then(function onSuccess(response) {
                $scope.allPlayerTypeFromDb = response.data;
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Error in Data Loading');
            });
    }
    function loadAllCountry() {
        $scope.allCountryFromDb = [];
        playerService.getAllCountry()
            .then(function onSuccess(response) {
                $scope.allCountryFromDb = response.data;
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Error in Data Loading');
            });
    }

    $("#imgInp").change(function () {
        files = this.files;
        $scope.file = files[0];

        if (files && files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#imgshowField').attr('src', e.target.result);
            }
            reader.readAsDataURL(files[0]);
        }
    });

    $scope.currentPage = 1; //current page
    $scope.itemsPerPage = 5; //max no of items to display in a page
    $scope.filteredItems = $scope.allPlayerFromDb.length; //Initially for no filter
    $scope.totalItems = $scope.allPlayerFromDb.length;

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };
    $scope.filter = function () {
        $timeout(function () {
            $scope.filteredItems = $scope.filtered.length;
        }, 9000);
    };
    $scope.sort_by = function (predicate) {
        $scope.predicate = predicate;
        $scope.reverse = !$scope.reverse;
    };
});