/// <reference path="../_ref.ts" />

module Messaging {
  "use strict";

  export class QueueBrowserController {

    // protect for minification, must match constructor signature.
    static $inject = [
      '$scope',
      '$window',
      '$location',
      'queueRepository',
      'toastr',
      'logger'
    ];

    constructor(
      $scope,
      $window: angular.IWindowService,
      $location: angular.ILocationService,
      queueRepository: QueueRepository,
      toastr: angular.toastr.IToastrService,
      logger: Logger) {

      var self = this;

      // assign vm to controller
      $scope.vm = this;
      self.$scope = $scope;
      self.$window = $window;
      self.$location = $location;
      self.queueRepository = queueRepository;
      self.toastr = toastr;
      self.logger = logger;

      self.loadDropdown();

      self.$scope.$on('changeNotification', self.onchange.bind(self));
      self.refresh = _.throttle(self.reload.bind(self), 300, { leading: false, trailing: true });
    }

    $scope: any;
    $window: angular.IWindowService;
    $location: angular.ILocationService;
    queueRepository: QueueRepository;
    toastr: angular.toastr.IToastrService;
    logger: Logger;

    refresh: Function;

    queue: INameValueModel;
    queues: INameValueModel[] = [];
    status: IQueueStatusModel;
    messages: IQueryyResult<IQueueMessageModel> = {
      Page: 1,
      PageSize: 10,
      Sort: "Updated",
      Descending: true
    };

    checkAll: boolean = false;
    showFilter: boolean = false;

    descriptionFilter: string;
    stateFilter: string;
    resultFilter: string;
    statusFilter: string;
    updateFilter: Date;

    loadDropdown() {
      var self = this;
      self.queueRepository.list()
        .success((data, status, headers, config) => {
          self.queues = data;
          // select first
          if (data.length) {
            self.queue = data[0];
            self.reload();
          }
        })
        .error(self.logger.logError);

    }

    loadGrid() {
      var self = this;
      if (!self.queue)
        return;

      var request = <IQueryRequest>{
        Page: self.messages.Page,
        PageSize: self.messages.PageSize,
        Sort: self.messages.Sort,
        Descending: self.messages.Descending,
        Filter: self.createFilter()
      };

      self.queueRepository.messages(self.queue.Value, request)
        .success((data, status, headers, config) => {
          self.messages = data;
        })
        .error(self.logger.logError);

    }

    loadStatus() {
      var self = this;
      if (!self.queue)
        return;

      self.queueRepository.status(self.queue.Name)
        .success((data, status, headers, config) => {
          self.status = data;
        })
        .error(self.logger.logError);
    }

    reload() {
      var self = this;

      self.loadGrid();
      self.loadStatus();
      self.checkAll = false;
    }

    selectAll() {
      var self = this;
      angular.forEach(self.messages.Data, (v, k) => {
        v.Selected = self.checkAll;
      });
    }

    requeueSelected() {
      var self = this;
      var model = self.getSelected();

      if (model.Ids.length === 0)
        return;

      BootstrapDialog.confirm("Are you sure you want to requeue the selected messages?", (result: boolean) => {

        if (!result)
          return;

        self.queueRepository.requeue(self.queue.Name, model)
          .success((data, status, headers, config) => {
            self.toastr.success('The selected messages have been requeued.', 'Success', { timeOut: 8000 });
            self.reload();
          })
          .error(self.logger.logError);

      });
    }

    deleteSelected() {
      var self = this;
      var model = self.getSelected();

      if (model.Ids.length === 0)
        return;

      BootstrapDialog.confirm("Are you sure you want to delete the selected messages?", (result: boolean) => {

        if (!result)
          return;

        self.queueRepository.delete(self.queue.Name, model)
          .success((data, status, headers, config) => {
            self.toastr.success('The selected messages have been deleted.', 'Success', { timeOut: 8000 });
            self.reload();
          })
          .error(self.logger.logError);

      });
    }

    getSelected(): IQueueActionModel {
      var self = this;
      var model = <IQueueActionModel>{
        Name: self.queue.Name,
        Ids: []
      }

      angular.forEach(self.messages.Data, (v, k) => {
        if (v.Selected)
          model.Ids.push(v.Id);
      });

      return model;
    }

    sortClick(column: string) {
      var self = this;

      if (self.messages.Sort == column)
        self.messages.Descending = !self.messages.Descending;
      else
        self.messages.Descending = false;

      self.messages.Sort = column;

      self.reload();
    }

    toggleFilter() {
      var self = this;
      self.showFilter = !self.showFilter;
    }

    createFilter() {
      var self = this;
      var filter = "";

      if (self.descriptionFilter) {
        filter += 'Description = "' + self.descriptionFilter + '"';
      }

      if (self.stateFilter) {
        if (filter.length > 0)
          filter += ' && ';

        filter += 'State = "' + self.stateFilter + '"';
      }

      if (self.resultFilter) {
        if (filter.length > 0)
          filter += ' && ';

        filter += 'Result = "' + self.resultFilter + '"';
      }

      if (self.statusFilter) {
        if (filter.length > 0)
          filter += ' && ';

        filter += 'Status = "' + self.statusFilter + '"';
      }

      if (self.updateFilter) {
        if (filter.length > 0)
          filter += ' && ';

        var yy = self.updateFilter.getFullYear(),
          mm = self.updateFilter.getMonth() + 1,
          dd = self.updateFilter.getDate();

        filter += 'Updated >= DateTime(' + yy + ', ' + mm + ', ' + dd + ')';
      }


      return filter;
    }

    onchange(e: angular.IAngularEvent, notification: IChangeNotification) {
      var self = this;
      if (!notification)
        return;

      if (notification.Namespace !== self.status.Namespace)
        return;

      self.refresh();
    }
  }

  // register controller
  angular.module(Messaging.applicationName).controller('queueBrowserController', [
    '$scope',
    '$window',
    '$location',
    'queueRepository',
    'toastr',
    'logger',
    QueueBrowserController // class must be last
  ]);
}

