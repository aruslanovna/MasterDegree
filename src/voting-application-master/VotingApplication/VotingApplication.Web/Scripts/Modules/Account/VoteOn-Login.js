(function () {
    'use strict';

    angular
        .module('VoteOn-Login', ['VoteOn-Account'])
            .config([
            '$routeProvider',
            function ($routeProvider) {
                $routeProvider
                    .when('/ForgottenPassword', {
                        templateUrl: function () {
                            return '../Login/ForgottenPassword';
                        }
                    })
                    .when('/EmailNotConfirmed/:emailAddress', {
                        templateUrl: function () {
                            return '../Login/EmailNotConfirmed';
                        }
                    })
                    .when('/ResetPassword/', {
                        templateUrl: function () {
                            return '../Login/PasswordReset';
                        }
                    })
                    .otherwise({
                        templateUrl: function () {
                            return '../Login/Login';
                        }
                    });
            }]);
})();