/// <reference path="../_ref.ts" />

module Enterprise {
  "use strict";

  export class QueueRepository {
    static $inject = [
      '$http'
    ];

    urlBase: string = 'api/Queue';
    $http: ng.IHttpService;

    constructor(
      $http: ng.IHttpService) {
      this.$http = $http;
    }

    list(): ng.IHttpPromise<INameValueModel[]> {
      var url = this.urlBase;
      return this.$http.get<INameValueModel[]>(url);

    }

    status(queue: string): ng.IHttpPromise<IQueueStatusModel> {
      var url = this.urlBase + '/' + encodeURIComponent(queue) + '/Status';
      return this.$http.get<IQueueStatusModel>(url);
    }

    messages(queue: string, request: IQueryRequest): ng.IHttpPromise<IQueryyResult<IQueueMessageModel>> {
      var url = this.urlBase + '/' + encodeURIComponent(queue) + '/Messages';
      var config = <IQueryRequest>{ params: request };
      return this.$http.get<IQueryyResult<IQueueMessageModel>>(url, config);
    }

    requeue(queue: string, action: IQueueActionModel): ng.IHttpPromise<void> {
      var url = this.urlBase + '/' + encodeURIComponent(queue) + '/Requeue';
      return this.$http.post<void>(url, action);
    }

    delete(queue: string, action: IQueueActionModel): ng.IHttpPromise<void> {
      var url = this.urlBase + '/' + encodeURIComponent(queue) + '/Messages';
      var config = <ng.IRequestShortcutConfig>{
        headers: {
          'Content-Type': 'application/json'
        },
        data: action
      };

      return this.$http.delete<void>(url, config);
    }


  }

  // register service
  angular.module(Enterprise.applicationName).service('queueRepository', [
    '$http',
    QueueRepository
  ]);
}  