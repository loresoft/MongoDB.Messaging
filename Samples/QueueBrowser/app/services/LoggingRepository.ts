/// <reference path="../_ref.ts" />

module Enterprise {
  "use strict";

  export class LoggingRepository {
    static $inject = [
      '$http'
    ];

    urlBase: string = 'api/Logging';
    $http: ng.IHttpService;

    constructor(
      $http: ng.IHttpService) {
      this.$http = $http;
    }

    get(request: IQueryRequest): ng.IHttpPromise<IQueryyResult<IQueueMessageModel>> {
      var url = this.urlBase + '/';
      var config = <IQueryRequest>{ params: request };
      return this.$http.get<IQueryyResult<IQueueMessageModel>>(url, config);
    }

  }

  // register service
  angular.module(Enterprise.applicationName).service('loggingRepository', [
    '$http',
    LoggingRepository
  ]);
}  