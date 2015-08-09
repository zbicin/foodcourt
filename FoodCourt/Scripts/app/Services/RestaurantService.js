angular.module('FoodCourtApp').service('RestaurantService', [
    'apiUrl',
    'isDebug',
    '$http',
    '$q', function (apiUrl,
        isDebug,
        $http,
        $q) {
        return {
            getListForKindId: function (kindId) {
                var deferred = $q.defer();

                $http.get(apiUrl + 'api/Restaurant/Search?searchPhrase=&kindId=' + kindId)
                .then(function (response) {
                    deferred.resolve(response);
                }, function (response) {
                    deferred.reject(response);
                });

                return deferred.promise;
            },
            put: function (restaurant) {
                var deferred = $q.defer();

                $http.put(apiUrl + 'api/Restaurant/Put', restaurant)
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