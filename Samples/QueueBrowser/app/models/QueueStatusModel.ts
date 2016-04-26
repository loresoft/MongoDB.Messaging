/// <reference path="../_ref.ts" />
module Messaging {
    "use strict";

    export interface IQueueStatusModel {
        Name?: string;
        Namespace?: string;
        Queued?: number;
        Processing?: number;
        Complete?: number;
        Timeout?: number;
        Scheduled?: number;
        Successful?: number;
        Warning?: number;
        Error?: number;
    }
}
