angular.module('FoodCourtApp').controller('PollController', ['$scope', 'PollService', 'KindService', '$filter', 'RestaurantService', 'DishService', 'OrderService', function ($scope, PollService, KindService, $filter, RestaurantService, DishService, OrderService) {
    $scope.kinds = [];
    $scope.restaurants = [];
    $scope.dishes = [];

    $scope.newOrder = {};
    clearNewOrder(); // sets default values

    $scope.getRestaurantsForKind = getRestaurantsForKind;
    $scope.getDishedForRestaurant = getDishedForRestaurant;
    $scope.processOrderForm = processOrderForm;

    $scope.$watch('newOrder.kindId', function (newValue, oldValue) {
        if (newValue === oldValue) return;
        if (newValue) {
            var filtered = $filter('filter')($scope.kinds, { Id: newValue });
            $scope.newOrder.kind = filtered.length > 0 ? filtered[0].Name : '';
        } else {
            $scope.newOrder.kind = '';
        }
    });

    $scope.$watch('newOrder.restaurantId', function (newValue, oldValue) {
        if (newValue === oldValue) return;
        if (newValue) {
            var filtered = $filter('filter')($scope.restaurants, { Id: newValue });
            $scope.newOrder.restaurant = filtered.length > 0 ? filtered[0].Name : '';
        } else {
            $scope.newOrder.restaurant = '';
        }
    });

    $scope.$watch('newOrder.dishId', function (newValue, oldValue) {
        if (newValue === oldValue) return;
        if (newValue) {
            var filtered = $filter('filter')($scope.dishes, { Id: newValue });
            $scope.newOrder.dish = filtered.length > 0 ? filtered[0].Name : '';
        } else {
            $scope.newOrder.dish = '';
        }
    });

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
        RestaurantService.getListForKindId(kind.Id).then(function (response) {
            $scope.restaurants = response.data;
            $scope.newOrder.dishId = null;
            $scope.dishes = [];
        }, function (error) {
            console.log(error);
        });
    }

    function getDishedForRestaurant(restaurant) {
        DishService.getListForKindIdAndRestaurantId($scope.newOrder.kindId, restaurant.Id).then(function (response) {
            $scope.dishes = response.data;
        }, function (error) {
            console.log(error);
        });
    }

    function processOrderForm() {
        OrderService.add($scope.newOrder.dishId, $scope.newOrder.isHelpNeeded, $scope.newOrder.isOptional).then(function (response) {
            clearNewOrder();
        }, function(error) {
            console.log(error);
        });
    }

    // private --------
    function clearNewOrder() {
        $scope.newOrder = {
            kindId: null,
            kind: '',
            restaurantId: null,
            restaurant: '',
            dishId: null,
            dish: '',
            isOptional: false,
            isHelpNeeded: false
        };
    }
}]);