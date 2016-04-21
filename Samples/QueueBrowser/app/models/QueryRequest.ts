/// <reference path="../_ref.ts" />
module Enterprise {
  "use strict";

  export interface IQueryRequest {
    Page?: number;
    PageSize?: number;

    Sort?: string;
    Descending?: boolean;

    Filter?: any;
  }
}
