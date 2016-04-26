/// <reference path="../_ref.ts" />
module Messaging {
  "use strict";

  export interface IChangeNotification {
    Timestamp?: number;
    UniqueId?: number;
    Version?: number;
    Operation?: string;
    Namespace?: string;
    Key?: string;
  }
}
