/// <reference path="../_ref.ts" />
module Messaging {
  "use strict";

  export interface IChangeNotification {
    Timestamp?: Date;
    UniqueId?: number;
    Version?: number;
    Operation?: string;
    Namespace?: string;
    Key?: string;
  }
}
