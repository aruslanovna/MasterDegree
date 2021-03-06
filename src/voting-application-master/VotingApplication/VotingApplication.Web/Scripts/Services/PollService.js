/// <reference path="AccountService.js" />
(function () {
    'use strict';

    angular
        .module('GVA.Poll')
        .factory('PollService', PollService);


    PollService.$inject = ['$http', '$q', 'AccountService', 'Errors'];

    function PollService($http, $q, AccountService, Errors) {

        var service = {
            getPoll: getPoll,
            getUserPolls: getUserPolls,
            createPoll: createPoll,
            copyPoll: copyPoll
        };

        return service;


        function getPoll(pollId, ballotToken) {

            if (!pollId) {
                var deferred = $q.defer();
                deferred.reject();
                return deferred.promise;
            }

            var tokenGuidHeader = null;

            if (ballotToken) {
                tokenGuidHeader = { 'X-TokenGuid': ballotToken };
            }

            var prom = $q.defer();

            return $http({
                method: 'GET',
                url: '/api/poll/' + pollId,
                headers: tokenGuidHeader
            })
                .then(function (response) {
                    prom.resolve(response.data);
                    return prom.promise;
                })
            .catch(function (response) { return transformError(response, prom); });
        }

        function transformError(response, promise) {

            switch (response.status) {
                case 401:
                    {
                        promise.reject(Errors.PollInviteOnlyNoToken);
                        break;
                    }
                case 404:
                    {
                        promise.reject(Errors.PollNotFound);
                        break;
                    }
                default:
                    promise.reject(response.readableMessage);
            }

            return promise.promise;
        }

        function getUserPolls() {

            return $http({
                method: 'GET',
                url: '/api/dashboard/polls',
                headers: { 'Authorization': 'Bearer ' + AccountService.account.token }
            });
        }

        function createPoll(question, choices) {
            var token;
            if (AccountService.account) {
                token = AccountService.account.token;
            }
            else {
                token = null;
            }

            return $http({
                method: 'POST',
                url: 'api/poll',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8',
                    'Authorization': 'Bearer ' + token
                },
                data: JSON.stringify({
                    PollName: question,
                    Choices: choices
                })
            });

        }

        function copyPoll(pollId) {
            return $http({
                method: 'POST',
                url: '/api/dashboard/copy',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8',
                    'Authorization': 'Bearer ' + AccountService.account.token
                },
                data: JSON.stringify({ UUIDToCopy: pollId })
            });
        }
    }
})();

(function () {
    'use strict';

    angular
        .module('VoteOn-Common')
        .factory('PollService', PollService);

    PollService.$inject = ['$http', '$q', 'AccountService', 'ErrorService'];

    function PollService($http, $q, AccountService, ErrorService) {

        var service = {
            getPoll: getPoll,
            createPoll: createPoll,
            addChoice: addChoice,
            getUserPolls : getUserPolls
        };

        return service;

        function getPoll(pollId, ballotToken) {

            var prom = $q.defer();

            if (!pollId) {
                prom.reject();
                return prom.promise;
            }

            var tokenGuidHeader = null;

            if (ballotToken) {
                tokenGuidHeader = { 'X-TokenGuid': ballotToken };
            }

            return $http({
                method: 'GET',
                url: '/api/poll/' + pollId,
                headers: tokenGuidHeader
            })
                .then(function (response) {
                    prom.resolve(response.data);
                    return prom.promise;
                });
        }

        function createPoll(poll) {
            var token;
            if (AccountService.account) {
                token = AccountService.account.token;
            }
            else {
                token = null;
            }

            var prom = $q.defer();

            return $http({
                method: 'POST',
                url: 'api/poll',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8',
                    'Authorization': 'Bearer ' + token
                },
                data: JSON.stringify(poll)
            })
                .then(function (response) {
                    prom.resolve(response.data);
                    return prom.promise;
                });
        }

        function addChoice(pollId, newChoice) {
            return $http
                .post(
                    '/api/poll/' + pollId + '/choice',
                    newChoice
                );
        }

        function getUserPolls() {

            var prom = $q.defer();

            if (!AccountService.account || !AccountService.account.token) {

                ErrorService.handleNotLoggedInError();

                prom.reject();
                return prom.promise;
            }

            return $http({
                method: 'GET',
                url: '/api/dashboard/polls',
                headers: { 'Authorization': 'Bearer ' + AccountService.account.token }
            })
                .then(function (response) {
                    prom.resolve(response.data);
                    return prom.promise;
                });
        }
    }
})();
