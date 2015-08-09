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
        },
        delete: function(orderId) {
            var deferred = $q.defer();

            $http.delete(apiUrl + 'api/Order/Delete?orderId=' + orderId)
            .then(function (response) {
                deferred.resolve(response);
            }, function (response) {
                deferred.reject(response);
            });

            return deferred.promise;
        },
        getMatchesForPoll: function (poll) {
            var deferred = $q.defer();

            $http.get(apiUrl + 'api/Order/GetMatchesForPoll?pollId=' + poll.Id)
            .then(function (response) {
                deferred.resolve(response);
            }, function (response) {
                deferred.reject(response);
            });

            return deferred.promise;
        }
    };
}]);