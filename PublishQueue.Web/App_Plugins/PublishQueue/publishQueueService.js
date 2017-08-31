(function () {

    function queueService($http) {

        var serviceRoot = 'backoffice/queue/PublishQueueApi/';

        var service = {
            getItems: getItems,
            getStatus: getStatus,
            process: process,
            clear: clear,

            enqueue: enqueue

        };

        return service; 

        ///////////////

        function getItems(page) {
            return $http.get(serviceRoot + "GetItems?page=" + page);
        }

        function getStatus() {
            return $http.get(serviceRoot + "GetStatus");
        }

        function process() {
            return $http.get(serviceRoot + "ProcessQueue?throttle=250");
        }

        function clear() {
            return $http.post(serviceRoot + "ClearQueue");
        }

        function enqueue(nodeId, includeChildren, includeUnpublished) {
            var options = {
                IncludeChildren: includeChildren,
                IncludeUnpublished: includeUnpublished
            };

            return $http.post(serviceRoot + "EnqueueTree/" + nodeId, options);
        }
    }

    angular.module('umbraco.resources')
        .factory('publishQueueService', queueService);

})();
