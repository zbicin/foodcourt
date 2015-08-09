angular.module('FoodCourtApp').service('KindService', ['apiUrl', 'isDebug', '$http', '$q', function (apiUrl, isDebug, $http, $q) {
    return {
        getList: function () {
            var deferred = $q.defer();

            $http.get(apiUrl + 'api/Kind/GetList')
            .then(function (response) {
                deferred.resolve(response);
            }, function (response) {
                deferred.reject(response);
            });

            return deferred.promise;
        }
    };
}]);