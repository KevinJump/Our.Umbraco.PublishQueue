(function () {

    'use strict';

    function dialogController($scope, publishQueueService, publishQueueHub) {

        var vm = this;

        vm.processing = false;
        vm.processed = false;
        vm.count = 0;

        vm.publishAll = true;
        vm.includeUnpublished = false; 

        vm.node = $scope.dialogOptions.currentNode;

        vm.send = send;

        //////// signalR queue hub.

        vm.queueHub = publishQueueHub.initHub(function (hub) {

            vm.queueHub = hub;

            vm.queueHub.on('Add', function (data) {
                vm.update = data;
            });

            vm.queueHub.start();

        });

        ///////////////////

        function send() {
            vm.processing = true;

            publishQueueService.enqueue(vm.node.id, vm.publishAll, vm.includeUnpublished)
                .then(function (result) {
                    vm.count = result.data;
                    getStatus();
                    vm.processing = false;
                    vm.processed = true;
                }, function (error) {
                    vm.processing = false; 
                });
        }

        function getStatus() {
            publishQueueService.getStatus()
                .then(function (result) {
                    vm.status = result.data;
                });
        }

    }

    angular.module('umbraco')
        .controller('publishQueueDialogController', dialogController);

})();