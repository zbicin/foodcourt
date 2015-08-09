angular.module('FoodCourtApp').service('DishService', [
    'apiUrl',
    'isDebug',
    '$http',
    '$q', function (apiUrl,
        isDebug,
        $http,
        $q) {
        return {
            getListForKindIdAndRestaurantId: function (kindId, restaurantId) {
                var deferred = $q.defer();

                var localUrl = 'api/Dish/Search?searchPhrase=&kindId=' + kindId + '&restaurantId=' + restaurantId;

                $http.get(apiUrl + localUrl)
                .then(function (response) {
                    deferred.resolve(response);
                }, function (response) {
                    deferred.reject(response);
                });

                return deferred.promise;
            },
            put: function (dish) {
                var deferred = $q.defer();

                $http.put(apiUrl + 'api/Dish/Put', dish)
                .then(function (response) {
                    deferred.resolve(response);
                }, function (response) {
                    deferred.reject(response);
                });

                return deferred.promise;
            }
        };
    }
]);