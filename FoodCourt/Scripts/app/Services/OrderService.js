angular.module('FoodCourtApp').service('OrderService', ['apiUrl', 'isDebug', '$http', '$q', function (apiUrl, isDebug, $http, $q) {
    return {
        add: function (dishId, isHelpNeeded, isOptional) {
            var deferred = $q.defer();

            var data = {
                dishId: dishId,
                isOptional: isOptional,
                isHelpNeeded: isHelpNeeded
            };

            $http.put(apiUrl + 'api/Order/Put', data)
            .then(function (response) {
                deferred.resolve(response);
            }, function (response) {
                deferred.reject(response);
            });

            return deferred.promise;
        }
    };
}]);