/// <reference path="../_ref.ts" />
module Enterprise {
    "use strict";

    export interface IQueueStatusModel {
        Name?: string;
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
