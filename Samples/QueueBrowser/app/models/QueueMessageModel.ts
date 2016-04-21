/// <reference path="../_ref.ts" />
module Enterprise {
    "use strict";

    export interface IQueueMessageModel {
        Id?: string;
        Name?: string;
        Description?: string;
        Correlation?: string;
        State?: string;
        Result?: string;
        ResponseQueue?: string;
        Step?: number;
        Status?: string;
        Priority?: number;
        RetryCount?: number;
        ErrorCount?: number;
        Scheduled?: Date;
        StartTime?: Date;
        EndTime?: Date;
        Expire?: Date;
        UserName?: string;
        Created?: Date;
        Updated?: Date;
        Selected?: boolean;
    }
}
