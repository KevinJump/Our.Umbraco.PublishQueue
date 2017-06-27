angular.module('umbraco').controller('publishQueueDashboardController',
    function ($scope, $http, $timeout, publishQueueDashboardService, publishQueueHub) {

        $scope.loading = true; 
        $scope.status = { Count: 0 };
        $scope.maxCount = 0;

        $scope.getQueue = function (loading) {
            $scope.loading = loading;
            publishQueueDashboardService.getItems()
                .then(function (response) {
                    $scope.queue = response.data;
                    $scope.loading = false;
                    $scope.getStatus();
                });
        };

        $scope.getStatus = function () {
            publishQueueDashboardService.getStatus()
                .then(function (response) {
                    $scope.status = response.data;
                    $scope.updateCount = $scope.status.Count;

                    if ($scope.maxCount < $scope.status.Count) {
                        $scope.maxCount = $scope.status.Count;
                    }
                });
        };


        $scope.processQueue = function () {
            publishQueueDashboardService.process()
                .then(function (response) {
                    $scope.getQueue(true);
                });
            $scope.getStatus();
        };

        $scope.clearQueue = function () {
            if (confirm("are you sure you want to clear the queue - all actions will be lost")) {

                publishQueueDashboardService.clear()
                    .then(function (response) {
                        alert("cleared " + response.data + " items from queue");
                    });
            }
        };

        $scope.progressValue = function () {
            if ($scope.maxCount > 0) {
                return ($scope.status.Count / $scope.maxCount) * 100;
            }
            return 0;
        }


        $scope.actionNames = ['None', 'Publish', 'Save', 'Unpublish', 'Delete'];
        $scope.priorities = ["Low", "Normal", "High", "Super High"];

        $scope.updateCount = 0; 

        // buttons 
        //
        $scope.updateQueue = function () {
            $scope.getQueue(true);
        }

        // signal R 
        var queueHub = publishQueueHub.Connect('PublishQueueHub');

        queueHub.on('progress', function (data) {
            $scope.status = data;
            if (Math.abs($scope.updateCount - $scope.status.Count) > 10)
            {
                $scope.getQueue(false);
            }
        });

        queueHub.start();

        $scope.getQueue(true);
    }
);