angular.module('FoodCourtApp').service('PollService', ['apiUrl', 'isDebug', '$http', '$q', function (apiUrl, isDebug, $http, $q) {
    return {
        tryGetCurrentPoll: function () {
            var deferred = $q.defer();

            $http.post(apiUrl + 'Poll/TryGetCurrentPoll', {})
            .then(function (response) {
                deferred.resolve(response);
            }, function (response) {
                deferred.reject(response);
            });

            return deferred.promise;
        }
    };
}]);