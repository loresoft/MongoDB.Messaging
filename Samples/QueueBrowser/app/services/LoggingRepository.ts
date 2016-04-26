/// <reference path="../_ref.ts" />

module Messaging {
  "use strict";

  export class LoggingRepository {
    static $inject = [
      '$http'
    ];

    urlBase: string = 'api/Logging';
    $http: angular.IHttpService;

    constructor(
      $http: angular.IHttpService) {
      this.$http = $http;
    }

    get(request: IQueryRequest): angular.IHttpPromise<IQueryyResult<IQueueMessageModel>> {
      var url = this.urlBase + '/';
      var config = <IQueryRequest>{ params: request };
      return this.$http.get<IQueryyResult<IQueueMessageModel>>(url, config);
    }

  }

  // register service
  angular.module(Messaging.applicationName).service('loggingRepository', [
    '$http',
    LoggingRepository
  ]);
}  