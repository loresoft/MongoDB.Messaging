/// <reference path="_ref.ts" />

module Enterprise {
  "use strict";

  export var applicationName: string = 'app';

  export var application: ng.IModule = angular.module(
    Enterprise.applicationName,
    [
      'ngAnimate',
      'ngSanitize',
      'ngMessages',
      'angularMoment',
      'ui.bootstrap',
      'toastr'
    ]
  );
}
