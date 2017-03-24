angular.module('umbraco.resources').factory('publishQueueHub',
    function ($rootScope) {

        return {
            Connect: function (hubName, progressCallBack) {

                var connection = $.hubConnection('/umbraco/backoffice/signalR');
                var proxy = connection.createHubProxy(hubName);

                return {
                    start: function () {
                        connection.start();
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
                }
            }
        }
    });