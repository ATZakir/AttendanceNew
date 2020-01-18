/// <reference path="../angular.js" />


var app = angular.module('app',
    [
        'autocomplete',
        'ngMaterial',
        'ui.bootstrap',
        'dialogs.main',
        'ngMessages',
        'ui.sortable',
        'blockUI',
        'chart.js',
        'ngBootbox',
        'mgcrea.ngStrap',
        'multipleSelect',
        'ngAvatar'
    ]); 


app.config(function (blockUIConfig) {
    blockUIConfig.autoInjectBodyBlock = true; 
});



app.config(function ($popoverProvider) {
    angular.extend($popoverProvider.defaults, {
        html: true
    });
});

app.config(function ($datepickerProvider) {
    window.angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd-MMM-yyyy',
        startWeek: 1
    });
});



