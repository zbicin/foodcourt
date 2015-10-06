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
            },
            update: function(restaurant) {
                var deferred = $q.defer();

                $http.post(apiUrl + 'api/Restaurant/Update', restaurant)
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