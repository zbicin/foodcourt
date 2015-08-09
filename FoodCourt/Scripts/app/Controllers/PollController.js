angular.module('FoodCourtApp').controller('PollController', ['$scope', 'PollService', 'KindService', function ($scope, PollService, KindService) {
    $scope.kinds = [];

    $scope.newOrder = {
        kindId: null,
    };

    $scope.getRestaurantsForKind = getRestaurantsForKind;

    PollService.tryGetCurrentPoll().then(function (poll) {
        console.log(poll);
    }, function (error) {
        console.log(error);
        alert('Something went terribly wrong. See console for details.');
    });

    KindService.getList().then(function (response) {
        $scope.kinds = response.data;
    }, function (error) {
        console.log(error);
    });

    // public ------
    function getRestaurantsForKind(kind) {
        return [
            {
                Name: 'rest 1',
                MenuUrl: 'http://google.pl'
            },
            {
                Name: 'rest 1',
                MenuUrl: 'http://google.pl'
            }
        ];
    }
}]);