angular.module('umbraco.resources').factory('publishQueueDashboardService',
    function ($q, $http) {

        var serviceRoot = 'backoffice/queue/PublishQueueApi/';

        return {

            getItems: function () {
                return $http.get(serviceRoot + "GetItems");
            }, 

            getStatus : function () {
                return $http.get(serviceRoot + "GetStatus");
            },

            process: function () {
                return $http.get(serviceRoot + "ProcessQueue?throttle=250");
            },
            
            clear: function () {
                return $http.get(serviceRoot + "ClearQueue")
            }
        }
    }
);