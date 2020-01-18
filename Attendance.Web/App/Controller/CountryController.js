app.controller('countryCtrl', function ($scope, $timeout, focus, $anchorScroll, countryService, $modal, $log, $ngBootbox, dialogs) {
    $scope.init = function (permission) {
        if (permission === "NO") {
            $scope.NoPermission = true;
        }
    };
    $scope.messageModalObj = {};
    $scope.messageModalObj.message = '';
    loadAllCountry();
    $scope.showModalforSearch = false;
    // Add a new country
    $scope.addCountry = function () {
        $scope.$broadcast('show-errors-check-validity');
        if ($scope.form.$valid) {
            var country = {};
            country["Id"] = $scope.Id;
            country["Name"] = $scope.Name;
            country["Code"] = $scope.CountryCode;

            countryService.saveCountry(country)
                .then(function onSuccess(response) {
                    loadAllCountry();
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
    // Populate country

    $scope.selectedRow = null;
    $scope.populateCountry = function(rowvalue) {
        $scope.selectedRow = rowvalue;
        $scope.Id = rowvalue.Id;
        $scope.Name = rowvalue.Name;
        $scope.CountryCode = rowvalue.Code;
        $anchorScroll();
        focus('focusMe');
        $scope.desableIdField = true;
    };

    $scope.reset = function () {
        $scope.$broadcast('show-errors-reset');
        $scope.selectedRow = null;
        $scope.Id = '';
        $scope.Name = '';
        $scope.CountryCode = '';
    }

    $scope.deleteCountry = function (id) {
        countryService.deleteCountry(id)
            .then(function onSuccess(response) {
                $scope.messageModalObj.message = response.data.message;
                $scope.showModalforSearch = true;
                $timeout(function () { $scope.showModalforSearch = false; }, 3000);
                $scope.reset();
                loadAllCountry();
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Can not be deleted');
            });
    }


    // Load employee
    function loadAllCountry() {
        $scope.allCountryFromDb = [];
        countryService.getAllCountry()
            .then(function onSuccess(response) {
                $scope.allCountryFromDb = response.data;
            })
            .catch(function onError(response) {
                $ngBootbox.alert('Error Data Loading');
            });
    }



    $scope.currentPage = 1; //current page
    $scope.itemsPerPage = 5; //max no of items to display in a page
    $scope.filteredItems = $scope.allCountryFromDb.length; //Initially for no filter
    $scope.totalItems = $scope.allCountryFromDb.length;
    //$scope.maxSize = 15;

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

