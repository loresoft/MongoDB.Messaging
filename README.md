# MongoDB Messaging

## Overview

The MongoDB Messaging library is a lightweight queue pub/sub processing library based on MongoDB data store.

[![Build status](https://ci.appveyor.com/api/projects/status/anoiwx8md0uoo65p?svg=true)](https://ci.appveyor.com/project/LoreSoft/mongodb-messaging)

[![NuGet Version](https://img.shields.io/nuget/v/MongoDB.Messaging.svg?style=flat-square)](https://www.nuget.org/packages/MongoDB.Messaging/)

## Download

The MongoDB.Messaging library is available on nuget.org via package name `MongoDB.Messaging`.

To install MongoDB.Messaging, run the following command in the Package Manager Console

    PM> Install-Package MongoDB.Messaging
    
More information about NuGet package available at
<https://nuget.org/packages/MongoDB.Messaging>

## Development Builds

Development builds are available on the myget.org feed.  A development build is promoted to the main NuGet feed when it's determined to be stable. 

In your Package Manager settings add the following package source for development builds:
<http://www.myget.org/F/loresoft/>

### Features

* Easy to use Fluent API
* Self creating and cleaning of Queues
* Configurable message expiration
* Generic data payload
* Trigger processing from oplog change monitoring
* Configurable auto retry on error
* Message processing timeout
* Scalable via subscriber worker count
* Supports distributed locks

### Concepts

#### Queue

A queue is equivalent to a MongoDB collection.  The name of the queue will match the MongoDB collection name.

Queue names must be alphanumeric, without spaces or symbols.

It is a good practice to suffix the queue name with `Queue`.

#### Message

A message is the high level object that is a generic definition of a messages.  The message contains processing level information.  The message object is automatically created and updated by the Fluent API and should not be updated directly by the publisher or subscriber.

#### Data

Data is the message payload to be processed with the message.  Use data to pass information you need to process the message.

The data object must be serializable by MongoDB driver.

It is a good practice to have one queue per data object being passed to limit confusion and to maintain simplicity when subscribing to the queue. 

#### Publish

Publishing a message adds the message with the corresponding data to a queue for processing.  

#### Subscribe

In order to process a message on a queue, an application needs to subscribe to a queue.  There can be many subscribers to a queue to scale the load across processes. A subscriber can also set the worker count to scale the number of processing threads for that subscriber.

The framework ensures that only one subscriber can process a messages.

## Queue Configuration

The queue configuration is used to set default values on messages published to a queue.

An example of using the fluent api to configure the sleep queue.

```c#
MessageQueue.Default.Configure(c => c
    .Connection("MongoMessaging")
    .Queue(s => s
        .Name(SleepMessage.QueueName)
        .Priority(MessagePriority.Normal)
        .ResponseQueue("ReplyQueueName")
        .Retry(5)
    )
);
```

#### Properties

**Connection** is the app.config connection string name used to connect to MongoDB.   

**Name** is the name of the queue to configure.   
**Retry** is the number of times the message should be retried on error. Set to zero, default, to not retry.   
**Priority** is the default priority to publish the message with.    
**ResponseQueue** is the name of the queue where responses should be sent.    

## Publish Message

To publish a message to a queue, use the fluent api.

```c#
var message = await MessageQueue.Default.Publish(m => m
    .Queue(SleepMessage.QueueName)
    .Data(sleepMessage)
    .Correlation("321B4671-3B4C-4B97-8E81-D6A8CF22D4F0")
    .Description("User friendly description of the message")
    .Priority(MessagePriority.Normal)
    .Retry(1)
);
```

#### Properties

*Required*   
**Queue** is the name of the queue to publish to.   
**Data** is the object to pass in the message.  Used to process the message by the subscriber.   

*Optional*   
**Correlation** is an identifier used to link messages together.   
**Description** is a user friendly description of the message.   

*Overrides*   
**Retry** is the number of times the message should be retried on error.   
**Priority** is the default priority to publish the message with.   
**ResponseQueue** is the name of the queue where responses should be sent.    

#### Notes

* When setting the Data property, the message Name will be set to the Type name of the data object.
* When setting the Data property and Description hasn't been set, the data object `ToString()` value will be set as the description.
* If the underlying storage collection doesn't exist, it will be created on first publish

## Subscribe to Message

To subscribe to a queue, use the fluent api. The subscribe handler must implement `IMessageSubscriber`.

```c#
MessageQueue.Default.Configure(c => c
    .Connection("MongoMessaging")
    .Subscribe(s => s
        .Queue(SleepMessage.QueueName)
        .Handler<SleepHandler>()
        .Workers(4)
    )
);
```
To speed up processing, you can monitor the oplog for changes to trigger processing.  The connection must have access to local.oplog.rs

```c#
MessageQueue.Default.Configure(c => c
    .Connection("MongoMessaging")
    .Subscribe(s => s
        .Queue(SleepMessage.QueueName)
        .Handler<SleepHandler>()
        .Workers(4)
        .Trigger()
    )
);
```

#### Properties

*Required*   
**Queue** is the name of the queue to publish to.   
**Handler** is the class that implements IMessageSubscriber.  This is what processes the message.         

*Optional*   
**Workers** is the number of worker processes.   
**ExpireError** is how long to keep error messages.      
**ExpireWarning** is how long to keep warning messages.   
**ExpireSuccessful** is how long to keep successful messages.   
**PollTime** is the amount of time between work polling. If using Triggger, set to longer time.
**Retry** is a class that implements IMessageRetry.  IMessageRetry controls if an error message should be retried.   
**Timeout** is the amount of time before a processing message times out.   
**TimeoutAction** is how to handle timed out messages. Options are Fail or Retry.   
**Trigger** to enable monitoring of the oplog for changes to trigger processing.

## Message Service

In order for the message subscribers to process messages off queue, the `MessageService` needs to be created and `Start` called. Note, the `MessageService.Stop()` method tries to gracefully stop by waiting for active processes to finish.

```c#
_messageService = new MessageService();

// on service or application start
_messageService.Start();

// on service stop.  
_messageService.Stop();
```

## IMessageSubscriber Interface

The following is a sample implementation of `IMessageSubscriber`

```c#
public class SleepHandler : IMessageSubscriber
{
    public MessageResult Process(ProcessContext context)
    {
        // get message data
        var sleepMessage = context.Data<SleepMessage>();

        // Do processing here

        return MessageResult.Successful;
    }

    public void Dispose()
    {
        // free resources
    }
}
```

## IMessageRetry Interface

The `IMessageRetry` interface allows for customization of the retry of failed messages.

The following is the default implementation of `IMessageRetry`

```c#
public class MessageRetry : IMessageRetry
{
    public virtual bool ShouldRetry(ProcessContext processContext, Exception exception)
    {
        // get current message 
        var message = processContext.Message;

        // true to retry message
        return message.ErrorCount < message.RetryCount;
    }

    public virtual DateTime NextAttempt(ProcessContext processContext)
    {
        var message = processContext.Message;

        // retry weight, 1 = 1 min, 2 = 30 min, 3 = 2 hrs, 4+ = 8 hrs
        if (message.ErrorCount > 3)
            return DateTime.Now.AddHours(8);

        if (message.ErrorCount == 3)
            return DateTime.Now.AddHours(2);

        if (message.ErrorCount == 2)
            return DateTime.Now.AddMinutes(30);

        // default
        return DateTime.Now.AddMinutes(1);
    }
}
```

## Process Locks

The library has supports distributed locks.  The following are the lock types supported.

**DistributedLock** Distributed Lock manager provides synchronized access to a resources over a network    
**ThrottleLock** Throttle Lock Manager controls how frequent a process can run    

This is an example of using the DistributedLock.

```c#
var lockName = "PrintMessage";

// get MongoDB collection to store lock
var collection = GetCollection();

// create lock with timeout, max time it will wait for lock, of 5 minutes
var locker = new DistributedLock(collection, TimeSpan.FromMinutes(5));

// acquire lock; if can't, it will retry to get lock up to timeout value
var result = locker.Acquire(lockName);
if (!result)
    return; // acquire lock timeout

try
{
    // do processing here

}
finally
{
    // release lock
    locker.Release(lockName);
}
```