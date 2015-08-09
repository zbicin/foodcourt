angular.module('FoodCourtApp').service('KindService', ['apiUrl', 'isDebug', '$http', '$q', function (apiUrl, isDebug, $http, $q) {
    function basicGet(localUrl) {
        var deferred = $q.defer();

        $http.get(apiUrl + localUrl)
        .then(function (response) {
            deferred.resolve(response);
        }, function (response) {
            deferred.reject(response);
        });

        return deferred.promise;
    }

    function basicPost(localUrl, data) {
        var deferred = $q.defer();

        $http.post(apiUrl + localUrl, data)
        .then(function (response) {
            deferred.resolve(response);
        }, function (response) {
            deferred.reject(response);
        });

        return deferred.promise;
    }

    return {
        getList: function () {
            return basicGet('api/Kind/GetList');
        }
    };
}]);