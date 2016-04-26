/// <reference path="../_ref.ts" />
module Messaging {
  "use strict";

  export interface IQueryyResult<T> {
    Data?: T[];

    Page?: number;
    PageSize?: number;
    PageCount?: number;

    Sort?: string;
    Descending?: boolean;

    Filter?: any;

    Total?: number;
  }
}
