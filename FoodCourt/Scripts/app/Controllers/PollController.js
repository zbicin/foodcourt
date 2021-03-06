﻿angular.module('FoodCourtApp').controller('PollController', ['$scope', 'PollService', 'KindService', '$filter', 'RestaurantService', 'DishService', 'OrderService', function ($scope, PollService, KindService, $filter, RestaurantService, DishService, OrderService) {
    $scope.kinds = [];
    $scope.restaurants = [];
    $scope.dishes = [];
    $scope.matches = [];
    $scope.poll = {};

    $scope.newOrder = {};
    clearNewOrder(); // sets default values

    $scope.cancelOrder = cancelOrder;
    $scope.getRestaurantsForKindId = getRestaurantsForKindId;
    $scope.getDishesForRestaurantId = getDishesForRestaurantId;
    $scope.finishPoll = finishPoll;
    $scope.processOrderForm = processOrderForm;
    $scope.refreshMatches = refreshMatches;
    $scope.showAddDishPrompt = showAddDishPrompt;
    $scope.showAddKindPrompt = showAddKindPrompt;
    $scope.showAddRestaurantPrompt = showAddRestaurantPrompt;
    $scope.showAddRestaurantMenuUrlPrompt = showAddRestaurantMenuUrlPrompt;
    $scope.showAddRestaurantPhoneNumberPrompt = showAddRestaurantPhoneNumberPrompt;

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
        getKinds();
        $scope.poll = response.data;
        refreshMatches();
    }, function (error) {
        console.log(error);
        alert('Something went terribly wrong. See console for details.');
    });


    // public ------
    function cancelOrder(order) {
        if (confirm('Are you sure you want to cancel your order \"' + order.Dish + '\" from \"' + order.Restaurant + '\"?')) {
            OrderService.delete(order.Id).then(function (response) {
                refreshMatches();
            }, function (error) {
                console.log(error);
            });
        }
    }

    function finishPoll() {
        if (confirm('Are you sure you want to finish poll?')) {
            PollService.finish().then(function(response) {
                var poll = response.data;

                if (poll.IsResolved) {
                    alert('The poll has been successfuly resolved. Check your email inbox for details.');
                } else {
                    alert('The poll has been partially resolved. There are still people that haven\'t been matched. Check your email inbox for details.')
                }
                location.reload();
            }, function(error) {
                console.log(error);
            });
        }
    }

    function getRestaurantsForKindId(kindId) {
        RestaurantService.getListForKindId(kindId).then(function (response) {
            $scope.restaurants = response.data;
            $scope.newOrder.dishId = null;
            $scope.dishes = [];
        }, function (error) {
            console.log(error);
        });
    }

    function getDishesForRestaurantId(restaurantId) {
        DishService.getListForKindIdAndRestaurantId($scope.newOrder.kindId, restaurantId).then(function (response) {
            $scope.dishes = response.data;
        }, function (error) {
            console.log(error);
        });
    }

    function processOrderForm() {
        OrderService.add($scope.newOrder.dishId, $scope.newOrder.isHelpNeeded, $scope.newOrder.isOptional).then(function (response) {
            clearNewOrder();
            refreshMatches();
            getKinds();
        }, function (error) {
            console.log(error);
        });
    }

    function refreshMatches() {
        OrderService.getMatchesForPoll($scope.poll).then(function (response) {
            $scope.matches = response.data;
        }, function (error) {
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
                if (error.status === 409) {
                    getDishesForRestaurantId($scope.newOrder.restaurantId);
                } else {
                    console.log(error);
                }
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
                if (error.status === 409) {
                    getKinds();
                } else {
                    console.log(error);
                }
                console.log(error);
            });
        }
    }

    function showAddRestaurantPrompt() {
        var name = prompt('Give name of new restaurant (i.e. \"Da Grasso Zachodnia\", \"Ha Long Piotrkowska\")');
        if (name) {
            RestaurantService.put({
                Name: name
            }).then(function (response) {
                $scope.restaurants.push(response.data);
            }, function (error) {
                if (error.status === 409) {
                    getRestaurantsForKindId($scope.newOrder.kindId);
                } else {
                    console.log(error);
                }
                console.log(error);
            });
        }
    }

    function showAddRestaurantPhoneNumberPrompt(restaurant) {
        var phoneNumber = prompt('Give a contact number to \"' + restaurant.Name + '\", it will simplify the ordering process. You can leave this field blank.');
        
        if (phoneNumber) {
            restaurant.PhoneNumber = phoneNumber;

            RestaurantService.update(restaurant).then(function (response) {
                restaurant = response.data;
            }, function (error) {
                if (error.status === 409) {
                    getRestaurantsForKindId($scope.newOrder.kindId);
                } else {
                    console.log(error);
                }
                console.log(error);
            });
        }
    }

    function showAddRestaurantMenuUrlPrompt(restaurant) {
        var menuUrl = prompt('Give a url to the menu of \"' + restaurant.Name + '\". You can leave this field blank.');

        if (menuUrl) {
            restaurant.MenuUrl = menuUrl;

            RestaurantService.update(restaurant).then(function (response) {
                restaurant = response.data;
            }, function (error) {
                if (error.status === 409) {
                    getRestaurantsForKindId($scope.newOrder.kindId);
                } else {
                    console.log(error);
                }
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

        $scope.dishes = [];
        $scope.restaurants = [];
    }

    function getKinds() {
        KindService.getList().then(function (response) {
            $scope.kinds = response.data;
        }, function (error) {
            console.log(error);
        });
    }
}]);