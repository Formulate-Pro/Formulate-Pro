﻿// Variables.
var app = angular.module("umbraco");

// Associate directive/controller.
app.directive("formulateDesignedEmailHandler", directive);

// Directive.
function directive() {
    return {
        restrict: "E",
        replace: true,
        templateUrl: "/App_Plugins/Formulate.Pro/Directives/Handlers/DesignedEmailHandler/designedEmailHandler.html",
        scope: {
            configuration: "=",
            fields: "="
        },
        controller: Controller
    };
}

// Controller.
function Controller($scope, notificationsService) {

    // Variables.
    this.injected = {
        $scope: $scope,
        notificationsService: notificationsService
    };

    // Initialize scope variables.
    if (!$scope.configuration.recipients) {
        $scope.configuration.recipients = [];
    }
    $scope.deliveryTypes = [{
        id: "to",
        name: "To"
    }, {
        id: "cc",
        name: "Cc"
    }, {
        id: "bcc",
        name: "Bcc"
    }];

    // Add scope functions.
    $scope.addRecipient = this.addRecipient.bind(this);
    $scope.deleteRecipient = this.deleteRecipient.bind(this);

}

/**
 * Adds a blank email address to the list of recipients of emails.
 */
Controller.prototype.addRecipient = function () {
    var $scope = this.injected.$scope;
    $scope.configuration.recipients.push({
        email: ""
    });
};


/**
 * Deletes the email recipient at the specified index.
 * @param {any} index The index of the email recipient to delete.
 */
Controller.prototype.deleteRecipient = function (index) {
    var $scope = this.injected.$scope;
    $scope.configuration.recipients.splice(index, 1);
};