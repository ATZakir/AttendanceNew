/// <reference path="../app.js" />
/// <reference path="../services/ManageTeamService.js" />


app.controller('manageTeamCtrl',
    function ($scope,
        $timeout,
        $filter,
        focus,
        $location,
        $anchorScroll,
        manageTeamService,
        $log,
        $ngBootbox,
        blockUI) {


        $scope.selectedType = null;
        $scope.selectedTeam = null;
        $scope.teamPlayerListMain = [];
        $scope.allPlayerList = [];
        $scope.selectedPlayerList = [];
        $scope.TeamList = [];
        $scope.tournamentId = tournamentId;
        $scope.HomeCountryPlayerCount = 4;
        $scope.Id = 2;
        $scope.MaximumTeamPoints = 11000;
        $scope.MinimumAllRounder = 2;
        $scope.MinimumBatsman = 2;
        $scope.MinimumBowlers = 2;
        $scope.MinimumBowlers = 3;
        $scope.TotalTeamSize = 11;
        $scope.CurrentTeamSize = 0;
        $scope.TournamentTypeId = 1;
        $scope.TournamentTypeName = "ODI";
        $scope.WicketKeeperCount = 1;
        $scope.msgStatusClass = 'error';
        $scope.ProgressType = 'warning';
        $scope.total = 0;
        $scope.teamCreateBtn = false;
        $scope.isCapSelected = false;
        $scope.isViceCapSelected = false;
        $scope.pointDifference = $scope.MaximumTeamPoints - $scope.total;
        if ($scope.CurrentTeamSize === 0) {
            $scope.createTeamMessage = "Select 11 Players (Min 4 player from BDESH)";
        }


        $scope.filterPlayer = function(value) {
            $scope.selectedType = value;
            $scope.typeId = value;
        };

        $scope.allTeam = function() {
            $scope.selectedTeam = null;
        };

        $scope.loadAllTeamPlayerList = function() {
            var id = parseInt($scope.tournamentId);
            manageTeamService.getAllTeamPlayerListByTournamentId(id)
                .then(function onSuccess(response) {
                    $scope.teamPlayerListMain = response.data;
                    $scope.allPlayerList = angular.copy(response.data);
                })
                .catch(function onError(response) {
                    $ngBootbox.alert('Data Loading Error');
                });
        };
        $scope.loadAllTeamPlayerList();


        $scope.loadAllTournamentTeamList = function() {
            var id = parseInt($scope.tournamentId);
            manageTeamService.getAllTournamentTeamList(id)
                .then(function onSuccess(response) {
                    $scope.TeamList = response.data;
                })
                .catch(function onError(response) {
                    $ngBootbox.alert('Team List Loading Error.');
                });
        };
        $scope.loadAllTournamentTeamList();


        $scope.loadTeamFormationRule = function () {
            var id = parseInt($scope.tournamentId);
            manageTeamService.getTeamFormationRule(id)
                .then(function onSuccess(response) {
                    $scope.teamFormationRule = response.data;
                    $scope.populateTeamFormationInfo($scope.teamFormationRule);
                })
                .catch(function onError(response) {
                    blockUI.stop();
                    $ngBootbox.alert('Team Formation Rule Loading Error');
                });
        };
        $scope.loadTeamFormationRule();

        $scope.populateTeamFormationInfo = function (teamFormationRule) {
            $scope.HomeCountryPlayerCount = teamFormationRule.HomeCountryPlayerCount;
            $scope.Id = teamFormationRule.Id;
            $scope.MaximumTeamPoints = teamFormationRule.MaximumTeamPoints;
            $scope.MinimumAllRounder = teamFormationRule.MinimumAllRounder;
            $scope.MinimumBowlers = teamFormationRule.MinimumBowlers;
            $scope.MinimumBatsman = teamFormationRule.MinimumBatsman;
            $scope.TotalTeamSize = teamFormationRule.TotalTeamSize;
            $scope.TournamentTypeId = teamFormationRule.TournamentTypeId;
            $scope.TournamentTypeName = teamFormationRule.TournamentTypeName;
            $scope.WicketKeeperCount = teamFormationRule.WicketKeeperCount;

        };

        $scope.refreshSelectionStatus = function () {
            $scope.total = 0;
            $scope.CurrentTeamSize = $scope.selectedPlayerList.length;
            var batsmanCount = 0;
            var bowlerCount = 0;
            var keeperCount = 0;
            var allRounderCount = 0;
            var battingRounderCount = 0;
            var bowlingRounderCount = 0;
            var wkBatsmanCount = 0;
            var bdeshPlayerCount = 0;

            if ($scope.selectedPlayerList.length > 0) {
                for (var i = 0; i < $scope.selectedPlayerList.length; i++) {
                    
                    switch ($scope.selectedPlayerList[i].PlayerTypeId) {
                    case 1:
                        batsmanCount++;
                        break;
                    case 2:
                        bowlerCount++;
                        break;
                    case 4:
                        allRounderCount++;
                        break;
                    case 5:
                        keeperCount++;
                        break;
                    case 6:
                        battingRounderCount++;
                        allRounderCount++;
                        batsmanCount++;
                        break;
                    case 7:
                        bowlingRounderCount++;
                        allRounderCount++;
                        bowlerCount++;
                        break;
                    case 8:
                        wkBatsmanCount++;
                        keeperCount++;
                        batsmanCount++;
                        break;
                    default:
                    }
                    if ($scope.selectedPlayerList[i].CountryId === "BD") {
                        bdeshPlayerCount ++;
                    }
                    $scope.total = $scope.total + parseInt($scope.selectedPlayerList[i].PlayerValue);

                }
            }

            

            // ------- selected player number issues -----------------
            if ($scope.selectedPlayerList.length === $scope.TotalTeamSize) {
                $scope.ProgressType = 'success';

                if (bdeshPlayerCount < $scope.HomeCountryPlayerCount) {
                    $scope.createTeamMessage = "Min " + $scope.HomeCountryPlayerCount + " Bangladesh player Required";
                    $scope.msgStatusClass = 'error';
                } else if (batsmanCount < $scope.MinimumBatsman) {
                    $scope.createTeamMessage = "Min " + $scope.MinimumBatsman + " Batsman Required";
                    $scope.msgStatusClass = 'error';
                } else if (keeperCount < $scope.WicketKeeperCount) {
                    $scope.createTeamMessage = "Min " + $scope.WicketKeeperCount + " Wicket Keeper Required";
                    $scope.msgStatusClass = 'error';
                } else if (bowlerCount < $scope.MinimumBowlers) {
                    $scope.createTeamMessage = "Min " + $scope.MinimumBowlers + " Bowler Required";
                    $scope.msgStatusClass = 'error';
                } else if (allRounderCount < $scope.MinimumAllRounder) {
                    $scope.createTeamMessage = "Min " + $scope.MinimumAllRounder + " All-Rounder Required";
                    $scope.msgStatusClass = 'error';
                } else {
                    $scope.createTeamMessage = "Your Team is looking Fine";
                    $scope.msgStatusClass = 'success';
                    $scope.teamCreateBtn = true;
                }  

            } else if ($scope.selectedPlayerList.length > $scope.TotalTeamSize) {

                var exceed = $scope.selectedPlayerList.length - $scope.TotalTeamSize;
                $scope.createTeamMessage = "Remove " + exceed + " Player";
                $scope.msgStatusClass = 'error';
            } else {
                $scope.createTeamMessage = "Select " + $scope.TotalTeamSize
                    + " Players (Min " + $scope.HomeCountryPlayerCount + " player from BDESH)";
                $scope.ProgressType = 'warning';
                $scope.msgStatusClass = 'error';
            }

            // ------- selected player Value issues -----------------
            $scope.pointDifference = $scope.MaximumTeamPoints - $scope.total;

        };


        $scope.selectCaptain = function (teamPlayerId) {

            if ($scope.isCapSelected) {
                $scope.isCapSelected = false;
            } else {
                $scope.isCapSelected = true;
                
            }
            for (var i = $scope.selectedPlayerList.length - 1; i >= 0; i--) {
                if ($scope.selectedPlayerList[i].Id === teamPlayerId) {
                    if ($scope.selectedPlayerList[i].TeamRoleId === 1) {
                        $scope.selectedPlayerList[i].TeamRoleId = 5;
                    } else {
                        $scope.selectedPlayerList[i].TeamRoleId = 1;
                    }
                }
            }
        };

        $scope.selectViceCaptain = function (teamPlayerId) {

            if ($scope.isViceCapSelected) {
                $scope.isViceCapSelected = false;
            } else {
                $scope.isViceCapSelected = true;
            
            }

            for (var i = $scope.selectedPlayerList.length - 1; i >= 0; i--) {
                if ($scope.selectedPlayerList[i].Id === teamPlayerId) {
                    if ($scope.selectedPlayerList[i].TeamRoleId === 2) {
                        $scope.selectedPlayerList[i].TeamRoleId = 5;
                    } else {
                        $scope.selectedPlayerList[i].TeamRoleId = 2;
                    }
                }
            }
        };


        $scope.addPlayer = function(teamPlayerId) {
            // get selected player 
            if ($scope.selectedPlayerList.length > $scope.TotalTeamSize + 1) {
                return;
            }

            $scope.selectedTeamPlayer = $filter('filter')($scope.allPlayerList,
                function(p) {
                    return p.Id === teamPlayerId;
                })[0];

            var pObj = {
                TeamPlayerId: $scope.selectedTeamPlayer.Id,
                TeamRoleId: 5,
                PlayerValue: $scope.selectedTeamPlayer.PlayerValue,
                CurrentScore: $scope.selectedTeamPlayer.CurrentScore,
                PlayerName: $scope.selectedTeamPlayer.PlayerName,
                TeamName: $scope.selectedTeamPlayer.TeamName,
                CountryName: $scope.selectedTeamPlayer.CountryName,
                PlayerTypeShort: $scope.selectedTeamPlayer.PlayerTypeShort
            };

            // add to selected player List
//            $scope.selectedPlayerList.push(pObj);
            $scope.selectedPlayerList.push($scope.selectedTeamPlayer);
       
            // remove selected player from player list
            for (var i = $scope.allPlayerList.length - 1; i >= 0; i--) {
                if ($scope.allPlayerList[i].Id === teamPlayerId) {
                    $scope.allPlayerList.splice(i, 1);
                }
            }


            $scope.refreshSelectionStatus();

        };

        $scope.removePlayer = function(teamPlayerId) {
            // get selected player 
            var selectedTeamPlayer = $filter('filter')($scope.selectedPlayerList,
                function(p) {
                    return p.Id === teamPlayerId;
                })[0];

            // add to selected player List
            $scope.allPlayerList.push(selectedTeamPlayer);


            // remove selected player from player list
            for (var i = $scope.selectedPlayerList.length - 1; i >= 0; i--) {
                if ($scope.selectedPlayerList[i].Id === teamPlayerId) {
                    $scope.selectedPlayerList.splice(i, 1);
                }
            }

            $scope.refreshSelectionStatus();
        };


        $scope.createTeam = function () {

            var capCount = 0;
            var viceCapCount = 0;

            for (var i = 0; i < $scope.selectedPlayerList.length; i++) {
                if ($scope.selectedPlayerList[i].TeamRoleId === 1) {
                    capCount++;
                }
                if ($scope.selectedPlayerList[i].TeamRoleId === 2) {
                    viceCapCount++;
                }
            }

            if (capCount !== 1 && viceCapCount !== 1) {
                $ngBootbox.alert("You have to select 1 Captain and 1 Vice-Captain");
                return;
            }

            var myTeam = {};
            myTeam["Id"] = $scope.Id;
            myTeam["Name"] = "User's Team";
            myTeam["TournamentId"] = parseInt($scope.tournamentId);
            myTeam["Players"] = $scope.selectedPlayerList;

            manageTeamService.saveMyTeam(myTeam)
                .then(function onSuccess(response) {
                    if (response.data.isSuccess) {
                        $ngBootbox.alert(response.data.message);
                        
                    } else {
                        $ngBootbox.alert(response.data.message);
                    }
                })
                .catch(function onError(response) {
                    $ngBootbox.alert('Error in saving');
                });
        };

        $scope.reset = function() {
//            $scope.loadTeamFormationRuleForODI();
        };

    });