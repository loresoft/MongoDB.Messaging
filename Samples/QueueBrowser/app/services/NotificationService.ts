/// <reference path="../_ref.ts" />

module Messaging {
  "use strict";


  export class NotificationService {
    static $inject = [
      '$rootScope',
      'Hub'
    ];

    private hubProxy: any;
    private $rootScope: angular.IScope;

    constructor($rootScope: angular.IScope, Hub: any) {
      var self = this;

      self.$rootScope = $rootScope;
      self.hubProxy = new Hub('changeNotificationHub', {
        listeners: {
          sendChange: self.onChange.bind(self)
        },

        errorHandler: self.errorHandler.bind(self),
        stateChanged: self.stateChanged.bind(self)
      });
    }


    onChange(notification: IChangeNotification) {
      var self = this;

      self.$rootScope.$broadcast('changeNotification', notification);
    }

    stateChanged(state: SignalR.StateChanged) {
      var self = this;

      self.$rootScope.$broadcast('changeNotificationState', state);
    }

    errorHandler(error) {
      console.error("error:" + error);
    }

    start() {
      console.log("Start Notification Service");
    }
  }

  // register service
  angular.module(Messaging.applicationName)
    .service('notificationService', [
      '$rootScope',
      'Hub',
      NotificationService
    ])
    .run(['notificationService', (notificationService: NotificationService) => {
      notificationService.start();
    }]);
}  