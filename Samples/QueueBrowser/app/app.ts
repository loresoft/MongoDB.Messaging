/// <reference path="_ref.ts" />

module Messaging {
  "use strict";

  export var applicationName: string = 'app';

  export var application: angular.IModule = angular.module(
    Messaging.applicationName,
    [
      'ngAnimate',
      'ngSanitize',
      'ngMessages',
      'angularMoment',
      'ui.bootstrap',
      'toastr',
      'SignalR'
    ]
  );
}
