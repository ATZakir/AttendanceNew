
app.filter('startFrom',
	function () {
		return function (input, start) {
			if (input) {
				start = +start; //parse to int
				return input.slice(start);
			}
			return [];
		}
	});
app.factory('focus',
	function ($rootScope, $timeout) {
		return function (name) {
			$timeout(function () {
				$rootScope.$broadcast('focusOn', name);
			});
		};
	});
app.factory('leave2Service',
	[
		'$http', function ($http) {

			return {
				saveLeave: function (leave) {
					return $http({
						url: '/Leave/CreateLeave',
						method: 'POST',
						data: leave
					});
				},
				getLeave: function (id) {
					return $http.get('/Leave/GetLeave/' + id);
				},
				getAllLeave: function () {
					return $http.get('/Leave/GetLeaveList');
				},
				getAllLeaveType: function () {
					return $http.get('/LeaveType/GetLeaveTypeList');
				},
				getAllDivision: function () {
					return $http.get('/UserPermission/GetAllDivisionByUserPermission');
				},
				getAllDistrictListByDivision: function (id) {
					return $http.get('/UserPermission/GetAllDistrictByUserPermission/' + id);
				},
				getAllUpazilaListByDistrict: function (id) {
					return $http.get('/UserPermission/GetAllUpazilaByUserPermission/' + id);
				},
				getAllSchoolListByUpazila: function (id) {
					return $http.get('/UserPermission/GetAllSchoolByUserPermission/' + id);
				},
				getAllEmployeeFromDb: function (id) {
					return $http.get('/Employee/GetEmployeeSimpleListBySchool/' + id);
				},
				deleteLeave: function (leave) {
					return $http({
						url: '/Leave/DeleteLeave',
						method: 'POST',
						data: leave
					});
				},
				getEmpLeaveBalance: function (id) {
					return $http.get('/Employee/GetEmployeeLeaveBalanceForLeave/' + id);
				}
			};
		}
	]);
app.controller('leave2Ctrl',
	function ($scope, $timeout, $http, focus, $location, $anchorScroll, leave2Service, $log, $ngBootbox, $filter) {
		$scope.messageModalObj = {};
		$scope.messageModalObj.message = '';
		$scope.level = true;

		loadAllLeave();
		$scope.showModalforSearch = false;

		$scope.StartDate = new Date();
		$scope.Date = new Date();
		$scope.showLeaveBalance = false;

		$scope.calculateNoOfDays = function () {

			var endDate = new Date($scope.EndDate.getFullYear(),
				$scope.EndDate.getMonth(),
				$scope.EndDate.getDate());
			var startDate = new Date($scope.StartDate.getFullYear(),
				$scope.StartDate.getMonth(),
				$scope.StartDate.getDate());
			var diffTime = Math.abs(endDate - startDate);
			var diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
			$scope.NoofDays = diffDays;
		}


		$scope.$watch('EmployeeId', function (empId) {
			if (empId > 0) {
				$scope.setEmployeeInfo(empId);
			} else {

			}
			
		});

		$scope.setEmployeeInfo = function (empId) {

			leave2Service.getEmpLeaveBalance(empId)
				.then(function onSuccess(response) {
					$scope.EmpLeaveBalance = response.data;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('Employee Leave Balance Loading error');
				});

			var obj = $filter('filter')($scope.allEmployeeFromDb, { Id: empId }, true)[0];
			if (obj) {
				$scope.Designation = obj.Designation;
				$scope.Department = obj.Department;
			}
		}

		// Add a new leave
		$scope.addLeave = function () {
			$scope.$broadcast('show-errors-check-validity');
			if ($scope.form.$valid) {
				var leave = {};
				leave["Id"] = $scope.Id;
				leave["LeaveTypeId"] = $scope.LeaveTypeId;
				leave["EmployeeId"] = $scope.EmployeeId;
				leave["Date"] = $scope.Date;
				leave["StartDate"] = $scope.StartDate;
				leave["EndDate"] = $scope.EndDate;
				leave["LeaveReason"] = $scope.LeaveReason;
				leave["ContactDuringLeave"] = $scope.ContactDuringLeave;
				leave["AddressDuringLeave"] = $scope.AddressDuringLeave;
				leave["Status"] = $scope.Status;
				leave["SchoolId"] = $scope.SchoolId;

				leave2Service.saveLeave(leave)
					.then(function onSuccess(response) {
						loadAllLeave();
						$scope.messageModalObj.message = response.data.message;
						$scope.showModalforSearch = true;
						$timeout(function () { $scope.showModalforSearch = false; }, 3000);
						$scope.reset();

					})
					.catch(function onError(response) {
						$ngBootbox.alert('Error in saving');
					});
			}
		};
		// Populate leave
		$scope.selectedRow = null;
		$scope.populateLeave = function (leave) {
			$scope.selectedRow = leave;
			$scope.Id = leave.Id;
			$scope.Name = leave.Name;
			$scope.LeaveTypeId = leave.LeaveTypeId;
			$scope.DivisionId = leave.DivisionId;
			$scope.DistrictId1 = leave.DistrictId;
			$scope.UpazilaId1 = leave.UpazilaId;
			$scope.SchoolId1 = leave.SchoolId;
			$scope.EmployeeId1 = leave.EmployeeId;
			$scope.GetAllDistrict($scope.DivisionId);
			$scope.GetAllUpazila($scope.DistrictId1);
			$scope.GetAllSchool($scope.UpazilaId1);
			$scope.loadAllEmployee($scope.SchoolId1);

			$scope.StartDate = moment(leave.StartDate).toDate();
			$scope.EndDate = moment(leave.EndDate).toDate();
			$location.hash('searchDivId');
			$anchorScroll();
			focus('focusMe');
		}


		$scope.reset = function () {
			$scope.$broadcast('show-errors-reset');
			$scope.Id = '';
			$scope.Name = '';
			$scope.LeaveTypeId = '';
			$scope.DivisionId = null;
			$scope.DistrictId = null;
			$scope.UpazilaId = null;
			$scope.SchoolId = null;
			$scope.EmployeeId = null;
			$scope.DistrictId1 = null;
			$scope.UpazilaId1 = null;
			$scope.SchoolId1 = null;
			$scope.EmployeeId1 = null;
			$scope.selectedRow = null;
		}

		$scope.deleteLeave = function (leave) {
			leave2Service.deleteLeave(leave)
				.then(function onSuccess(response) {
					$scope.messageModalObj.message = response.data.message;
					$scope.showModalforSearch = true;
					$timeout(function () { $scope.showModalforSearch = false; }, 3000);
					loadAllLeave();
				})
				.catch(function onError(response) {
					$ngBootbox.alert('Unable to deleted');
				});
		}

		// Load leave
		function loadAllLeave() {
			$scope.allLeaveFromDb = [];
			leave2Service.getAllLeave()
				.then(function onSuccess(response) {
					$scope.allLeaveFromDb = response.data;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('Error in loading');
				});
		}

		loadAllLeaveType();

		function loadAllLeaveType() {
			$scope.allLeaveTypeFromDb = [];
			leave2Service.getAllLeaveType()
				.then(function onSuccess(response) {
					$scope.allLeaveTypeFromDb = response.data;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('Error in loading Leave Types');
				});
		}


		// Load employee
		$scope.loadAllEmployee = function (schoolId) {
			$scope.allEmployeeFromDb = [];
			if (schoolId == undefined) return;

			leave2Service.getAllEmployeeFromDb(schoolId)
				.then(function onSuccess(response) {
					$scope.allEmployeeFromDb = response.data;
					$scope.EmployeeId = $scope.EmployeeId1;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('@Resources.ResourceCommon.MsgErrorInLoading');
				});
		}

		


		loadAllDivision();

		function loadAllDivision() {
			$scope.allDivisionFromDb = [];
			leave2Service.getAllDivision()
				.then(function onSuccess(response) {
					$scope.allDivisionFromDb = response.data;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('@Resources.ResourceCommon.MsgErrorInLoading');
				});
		}

		$scope.GetAllDistrict = function (divisionId) {
			$scope.allDistrictFromDb = [];
			if (divisionId == undefined) return;

			leave2Service.getAllDistrictListByDivision(divisionId)
				.then(function onSuccess(response) {
					$scope.allDistrictFromDb = response.data;
					$scope.DistrictId = $scope.DistrictId1;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('@Resources.ResourceCommon.MsgErrorInLoading');
				});
		}

		$scope.GetAllUpazila = function (districtId) {
			$scope.allUpazilaFromDb = [];
			if (districtId == undefined) return;

			leave2Service.getAllUpazilaListByDistrict(districtId)
				.then(function onSuccess(response) {
					$scope.allUpazilaFromDb = response.data;
					$scope.UpazilaId = $scope.UpazilaId1;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('@Resources.ResourceCommon.MsgErrorInLoading');
				});
		}

		$scope.GetAllSchool = function (upazilaId) {
			$scope.allSchoolFromDb = [];
			if (upazilaId == undefined) return;

			leave2Service.getAllSchoolListByUpazila(upazilaId)
				.then(function onSuccess(response) {
					$scope.allSchoolFromDb = response.data;
					$scope.SchoolId = $scope.SchoolId1;
				})
				.catch(function onError(response) {
					$ngBootbox.alert('@Resources.ResourceCommon.MsgErrorInLoading');
				});
		}

		$scope.currentPage = 1; //current page
		$scope.itemsPerPage = 5; //max no of items to display in a page
		$scope.filteredItems = $scope.allLeaveTypeFromDb.length; //Initially for no filter
		$scope.totalItems = $scope.allLeaveTypeFromDb.length;
		//$scope.maxSize = 15;

		$scope.setPage = function (pageNo) {
			$scope.currentPage = pageNo;
		};
		$scope.filter = function () {
			$timeout(function () {
				$scope.filteredItems = $scope.filtered.length;
			},
				9000);
		};
		$scope.sort_by = function (predicate) {
			$scope.predicate = predicate;
			$scope.reverse = !$scope.reverse;
		};
	});