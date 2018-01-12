angular.module('umbraco.resources').factory('publishQueueHub',
    function ($rootScope, $q, assetsService) {

        var scripts = [
            "/App_Plugins/PublishQueue/libs/jquery.signalR-2.2.1.js",
            "/umbraco/backoffice/signalr/hubs"];

        var resource = {
            initHub : initHub
        };

        return resource;

        //////////////

        function initHub(callback) {
            if ($.connection == undefined) {

                // we have to load this way, because the 
                // asset service won't reconise the 
                // umbraco call, as it has not extension
                // so we do them our selves and then 
                // wait for them all to resolve. 

                var promises = [];
                scripts.forEach(function (script) {
                    promises.push(assetsService.loadJs(script));
                });

                $q.all(promises)
                    .then(function () {
                        console.log('scripts loaded');
                        hubSetup(callback);
                    });
            }
            else {
                hubSetup(callback);
            }
        }

        function hubSetup(callback) {

            var proxy = $.connection.publishQueueHub;

            var hub = {
                start: function () {
                    $.connection.hub.start();
                },
                on: function (eventName, callback) {
                    proxy.on(eventName, function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                invoke: function (methodName, callback) {
                    proxy.invoke(methodName)
                        .done(function (result) {

                            $rootScope.$apply(function () {
                                if (callback)
                                    callback(result);
                            });
                        });
                }
            };

            return callback(hub);

        }
    });