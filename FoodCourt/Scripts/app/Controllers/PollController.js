angular.module('FoodCourtApp').controller('PollController', ['$scope', 'PollService', 'KindService', '$filter', 'RestaurantService', 'DishService', 'OrderService', function ($scope, PollService, KindService, $filter, RestaurantService, DishService, OrderService) {
    $scope.kinds = [];
    $scope.restaurants = [];
    $scope.dishes = [];
    $scope.matches = [];
    $scope.poll = {};

    $scope.newOrder = {};
    clearNewOrder(); // sets default values

    $scope.getRestaurantsForKind = getRestaurantsForKind;
    $scope.getDishedForRestaurant = getDishedForRestaurant;
    $scope.processOrderForm = processOrderForm;
    $scope.refreshMatches = refreshMatches;
    $scope.showAddDishPrompt = showAddDishPrompt;
    $scope.showAddKindPrompt = showAddKindPrompt;
    $scope.showAddRestaurantPrompt = showAddRestaurantPrompt;

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

    PollService.tryGetCurrentPoll().then(function (response) {
        $scope.poll = response.data;
        refreshMatches();
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
            refreshMatches();
        }, function(error) {
            console.log(error);
        });
    }

    function refreshMatches() {
        OrderService.getMatchesForPoll($scope.poll).then(function(response) {
            $scope.matches = response.data;
        }, function(error) {
            console.log(error);
        });
    }

    function showAddDishPrompt() {
        var usersInput = prompt('Give name of new dish (i.e. \"Margaritha\", \"Dumplings with cheese\")');
        if (usersInput) {
            DishService.put({
                KindId: $scope.newOrder.kindId,
                Name: usersInput,
                RestaurantId: $scope.newOrder.restaurantId
            }).then(function (response) {
                $scope.dishes.push(response.data);
            }, function (error) {
                console.log(error);
            });
        }
    }

    function showAddKindPrompt() {
        var usersInput = prompt('Give name of new dish kind (i.e. \"Pizza\", \"Pasta\")');
        if (usersInput) {
            KindService.put({
                Name: usersInput
            }).then(function (response) {
                $scope.kinds.push(response.data);
            }, function (error) {
                console.log(error);
            });
        }
    }

    function showAddRestaurantPrompt() {
        var name = prompt('Give name of new restaurant (i.e. \"Da Grasso Zachodnia\", \"Ha Long Piotrkowska\")');
        if (name) {
            var phoneNumber = prompt('Give a contact number to \"' + name + '\", it will simplify the ordering process. You can leave this field blank.');
            var menuUrl = prompt('Give a url to the menu of \"' + name + '\". You can leave this field blank.');

            if (!phoneNumber) {
                phoneNumber = null;
            }

            if (!menuUrl) {
                menuUrl = null;
            }

            RestaurantService.put({
                MenuUrl: menuUrl,
                Name: name,
                PhoneNumber: phoneNumber
            }).then(function (response) {
                $scope.restaurants.push(response.data);
            }, function (error) {
                console.log(error);
            });
        }
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

    function addPositionPrompt(question, serviceResponsible, collectionOfElements) {
    }
}]);