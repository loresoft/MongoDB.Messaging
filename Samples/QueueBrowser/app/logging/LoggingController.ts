/// <reference path="../_ref.ts" />

module Messaging {
  "use strict";

  export class LoggingController {

    // protect for minification, must match constructor signature.
    static $inject = [
      '$scope',
      '$window',
      '$location',
      'loggingRepository',
      'toastr',
      'logger'
    ];

    constructor(
      $scope,
      $window: angular.IWindowService,
      $location: angular.ILocationService,
      loggingRepository: LoggingRepository,
      toastr: angular.toastr.IToastrService,
      logger: Logger) {

      var self = this;

      // assign vm to controller
      $scope.vm = this;
      self.$scope = $scope;
      self.$window = $window;
      self.$location = $location;
      self.loggingRepository = loggingRepository;
      self.toastr = toastr;
      self.logger = logger;

      self.loadGrid();
    }

    $scope: any;
    $window: angular.IWindowService;
    $location: angular.ILocationService;
    loggingRepository: LoggingRepository;
    toastr: angular.toastr.IToastrService;
    logger: Logger;

    logs: IQueryyResult<ILogEvent> = {
      Page: 1,
      PageSize: 10,
      Sort: "Date",
      Descending: true
    };

    showFilter: boolean = false;
    sourceFilter: string;
    loggerFilter: string;
    levelFilter: string;
    messageFilter: string;
    dateFilter: Date;

    loadGrid() {
      var self = this;
      var request = <IQueryRequest>{
        Page: self.logs.Page,
        PageSize: self.logs.PageSize,
        Sort: self.logs.Sort,
        Descending: self.logs.Descending,
        Filter: self.createFilter()
      };

      self.loggingRepository.get(request)
        .success((data, status, headers, config) => {
          self.logs = data;
        })
        .error(self.logger.logError);
    }

    reload() {
      var self = this;
      self.loadGrid();
    }

    expand(log: ILogEvent) {
      log.Expanded = !log.Expanded;
    }

    sortClick(column: string) {
      var self = this;

      if (self.logs.Sort == column)
        self.logs.Descending = !self.logs.Descending;
      else
        self.logs.Descending = false;

      self.logs.Sort = column;

      self.reload();
    }

    toggleFilter() {
      var self = this;
      self.showFilter = !self.showFilter;
    }

    createFilter() {
      var self = this;
      var filter = "";

      if (self.sourceFilter) {
        filter += 'Source = "' + self.sourceFilter + '"';
      }

      if (self.loggerFilter) {
        if (filter.length > 0)
          filter += ' && ';

        filter += 'Logger = "' + self.loggerFilter + '"';
      }

      if (self.levelFilter) {
        if (filter.length > 0)
          filter += ' && ';

        filter += 'Level = "' + self.levelFilter + '"';
      }

      if (self.messageFilter) {
        if (filter.length > 0)
          filter += ' && ';

        filter += 'Message = "' + self.messageFilter + '"';
      }

      if (self.dateFilter) {
        if (filter.length > 0)
          filter += ' && ';

        var yy = self.dateFilter.getFullYear(),
          mm = self.dateFilter.getMonth() + 1,
          dd = self.dateFilter.getDate();

        filter += 'Date >= DateTime(' + yy + ', ' + mm + ', ' + dd + ')';
      }


      return filter;
    }
  }

  // register controller
  angular.module(Messaging.applicationName).controller('loggingController', [
    '$scope',
    '$window',
    '$location',
    'loggingRepository',
    'toastr',
    'logger',
    LoggingController // class must be last
  ]);
}

