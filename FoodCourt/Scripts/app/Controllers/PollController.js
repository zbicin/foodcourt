angular.module('FoodCourtApp').controller('PollController', ['$scope', 'PollService', function ($scope, PollService) {
    PollService.tryGetCurrentPoll().then(function (poll) {
        console.log(poll);
    }, function (error) {
        console.log(error);
        alert('Something went terribly wrong. See console for details.');
    });
}]);