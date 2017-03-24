angular.module('umbraco').controller('publishQueueDialogController',
    function ($scope, $routeParams, editorState, $http, $q, $location, publishQueueDashboardService, publishQueueHub) {
        $scope.loading = true;
        $scope.processing = false; 
        $scope.processed = false;
        $scope.count = 0;

        $scope.publishAll = true;
        $scope.includeUnpublished = false;

        var dialogOptions = $scope.dialogOptions;
        var node = dialogOptions.currentNode;

        $scope.send = function () {

            $scope.processing = true;

            $http({
                method: 'GET',
                url: '/umbraco/backoffice/queue/PublishQueueApi/EnqueueTree?id=' + node.id + "&all=" + $scope.publishAll + "&unpub=" + $scope.includeUnpublished
            })
                .success(function (data, status, headers, config) {
                    $scope.count = data.data;
                    $scope.loading = false;
                    $scope.processing = false;
                    $scope.processed = true;
                    $scope.getStatus();
                })
                .error(function (data, status, headers, config) {
                    $scope.loading = false;
                    $scope.processing = false;
                    $scope.processed = true;
                });
        }

        var queueHub = publishQueueHub.Connect('PublishQueueHub');

        queueHub.on('add', function (data) {
            $scope.update = data;
        });

        $scope.getStatus = function () {
            publishQueueDashboardService.getStatus()
                .then(function (response) {
                    $scope.status = response.data;
                });
        };

        queueHub.start();
    }
);