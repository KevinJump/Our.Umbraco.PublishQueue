(function () {

    'use strict';

    function dashboardController($scope, publishQueueService, publishQueueHub) {

        var vm = this;

        vm.loaded = false;
        vm.status = {
            Count: 0
        };
        vm.maxCount = 0;

        vm.loadQueue = loadQueue;
        vm.getQueueItems = getQueueItems;
        vm.getStatus = getStatus;

        vm.processQueue = processQueue;
        vm.clearQueue = clearQueue;
        vm.updateQueue = updateQueue;

        vm.percentQueue = percentQueue;

        vm.nextPage = nextPage;
        vm.prevPage = prevPage;
        vm.goToPage = goToPage;

        vm.showPos = showPos;

        vm.page = 1;

        vm.actions = ['None', 'Publish', 'Save', 'Unpublish', 'Delete'];
        vm.priorities = ["Low", "Normal", "High", "Super High"];


        //////////////////

        function loadQueue() {
            vm.loaded = false;
            getQueueItems(vm.page);
        }

        function getQueueItems(page) {
            publishQueueService.getItems(page)
                .then(function (result) {
                    vm.queue = result.data;
                    vm.loaded = true;
                    vm.getStatus();
                }, function (error) {

                });
        }

        function getStatus() {
            publishQueueService.getStatus()
                .then(function (result) {
                    vm.status = result.data;
                    vm.updateCount = vm.status.Count;
                    vm.maxCount = Math.max(vm.maxCount, vm.status.Count);
                });
        }

        function processQueue() {
            publishQueueService.process()
                .then(function (result) {
                    vm.getQueueItems(vm.page);
                })
        }

        function clearQueue() {
            if (!confirm("are you sure you want to clear the queue?")) {
                return;
            }

            publishQueueService.clear()
                .then(function (respose) {
                    // cleared
                });
        }

        function updateQueue() {
            vm.loadQueue();
        }

        function percentQueue() {
            if (vm.maxCount > 0) {
                return (vm.status.Count / vm.maxCount) * 100;
            }

            return 0;
        }

        function showPos(index) {
            return ((vm.queue.CurrentPage - 1) * vm.queue.ItemsPerPage) + (index + 1)
        }

        ///// signal r
        vm.queueHub = publishQueueHub.initHub(function (hub) {

            vm.queueHub = hub;

            vm.queueHub.on('progress', function (data) {
                vm.status = data;
                // more than 100, and we have a page, anyway.?
                if (Math.abs(vm.updateCount - vm.status.Count) > 10) {
                    vm.getQueueItems(vm.page);
                }
            })

            vm.queueHub.start();

        });

        vm.loadQueue();

        /////////////

        function nextPage () {
           vm.page++;
           refreshView();
        }

        function prevPage () {
           vm.page--;
           refreshView();
        }

        function goToPage (pageNo) {
           vm.page = pageNo;
            refreshView();
        }

        function refreshView() {
            vm.getQueueItems(vm.page);
        }
    }

    angular.module('umbraco')
        .controller('publishQueueDashboardController', dashboardController);

})();
