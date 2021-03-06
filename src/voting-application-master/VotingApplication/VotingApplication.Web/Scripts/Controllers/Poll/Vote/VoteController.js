(function () {
    'use strict';

    angular
        .module('VoteOn-Vote')
        .controller('VoteController', VoteController);

    VoteController.$inject = ['$scope', '$routeParams', '$mdDialog', 'TokenService', 'PollService', 'VoteService', 'RoutingService', 'IdentityService', 'ErrorService', 'SignalService'];

    function VoteController($scope, $routeParams, $mdDialog, TokenService, PollService, VoteService, RoutingService, IdentityService, ErrorService, SignalService) {

        $scope.loaded = false;
        $scope.submitting = false;

        $scope.pollId = $routeParams['pollId'];
        $scope.poll = {
            Name: null,
            PollType: null,
            TokenGuid: null,
            ExpiryDateUtc: null,
            NamedVoting: null,
            Choices: []
        };

        $scope.voterIdentity = IdentityService.identity;

        $scope.castVote = castVote;
        $scope.pollExpired = pollExpired;
        $scope.navigateToResultsPage = navigateToResultsPage;

        $scope.userChoice = null;
        $scope.addChoiceFromPage = addChoiceFromPage;
        $scope.addChoiceViaDialog = addChoiceViaDialog;

        activate();

        function activate() {
            getPollData();
            SignalService.registerObserver($scope.pollId, reloadPoll);
        }

        function getPollData() {
            var token = TokenService.retrieveToken($scope.pollId);

            PollService.getPoll($scope.pollId, token)
                .then(function (data) {
                    TokenService.setToken($scope.pollId, data.TokenGuid);
                    return data;
                })
                .then(function (data) {
                    loadPollData(data);
                })
                .catch(function (response) {
                    ErrorService.handleVotingError(response, $scope.pollId);
                });
        }

        function loadPollData(data) {
            var poll = $scope.poll;
            poll.Name = data.Name;
            poll.PollType = data.PollType;
            poll.TokenGuid = data.TokenGuid;
            poll.ExpiryDateUtc = data.ExpiryDateUtc;
            poll.NamedVoting = data.NamedVoting;
            poll.ChoiceAdding = data.ChoiceAdding;

            poll.MaxPoints = data.MaxPoints;
            poll.MaxPerVote = data.MaxPerVote;

            // Clear existing options
            poll.Choices.length = 0;
            poll.Choices = data.Choices;

            $scope.loaded = true;
        }

        function castVote() {
            $scope.submitting = false;

            if ($scope.poll.NamedVoting) {
                $scope.voteClicked = true;

                if ($scope.voterNameForm.$valid) {
                    submitVote($scope.voterIdentity.name);
                }
            } else {
                submitVote(null);
            }

            $scope.submitting = false;
        }

        function submitVote(voterName) {

            var votes = createVotes();

            var ballot = {
                VoterName: voterName,
                Votes: votes
            };

            VoteService.submitVote($scope.pollId, ballot, $scope.poll.TokenGuid)
                .then(function () {
                    RoutingService.navigateToResultsPage($scope.pollId);
                });
        }

        function createVotes() {
            return $scope.poll.Choices
                .filter(function (choice) {
                    return choice.VoteValue;
                })
                .map(function (choice) {
                    return {
                        ChoiceId: choice.Id,
                        VoteValue: choice.VoteValue
                    };
                });
        }

        function pollExpired() {
            RoutingService.redirectToResultsPage($scope.pollId);
        }

        function addChoiceFromPage() {
            addChoice($scope.userChoice);
        }

        function addChoice(choiceName) {
            if (choiceName !== null) {
                var newChoiceRequest = { Name: choiceName };

                $scope.submitting = true;

                PollService.addChoice($scope.pollId, newChoiceRequest)
                    .then(reloadPoll)
                    .then(function () {
                        $scope.userChoice = null;
                        $scope.submitting = false;
                    });
            }
        }

        function addChoiceViaDialog(triggeringEvent) {
            var addChoiceDialog = {
                parent: angular.element(document.body),
                targetEvent: triggeringEvent,
                clickOutsideToClose: true,
                templateUrl: '/Scripts/Dialogs/AddUserChoiceDialog/AddUserChoiceDialog.html',
                controller: 'AddUserChoiceDialogController'
            };
            $mdDialog
                .show(addChoiceDialog)
                .then(function (name) {
                    addChoice(name);
                });
        }

        function reloadPoll() {
            var token = TokenService.retrieveToken($scope.pollId);

            PollService.getPoll($scope.pollId, token)
                .then(updateChoices);
        }

        function updateChoices(data) {

            var currentChoices = $scope.poll.Choices;
            var currentChoiceIds = currentChoices.map(function (elem) { return elem.Id; });

            var updatedPollChoices = data.Choices;
            var updatedPollChoiceIds = updatedPollChoices.map(function (elem) { return elem.Id; });

            // Find intersection between existing and old, and select those from the current array
            // (This then keeps any changed voteValues).
            $scope.poll.Choices = currentChoices.filter(function (choice) {
                return updatedPollChoiceIds.indexOf(choice.Id) !== -1;
            });

            // Find newly added choices and concat them to the choices.
            var addedChoices = updatedPollChoices.filter(function (choice) {
                return currentChoiceIds.indexOf(choice.Id) < 0;
            });

            $scope.poll.Choices = $scope.poll.Choices.concat(addedChoices);
        }

        function navigateToResultsPage() {
            RoutingService.navigateToResultsPage($scope.pollId);
        }
    }
})();
