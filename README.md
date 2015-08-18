# MongoDB Messaging

## Overview

The MongoDB Messaging library is a lightweight queue pub/sub processing library based on MongoDB data store.

### Features

* Easy to use Fluent API
* Self creating and cleaning of Queues
* Configurable message expiration
* Generic data payload
* Configurable auto retry on error
* Message processing timeout
* Scalable via subscriber worker count

### Concepts

#### Queue

A queue is equivalent to a MongoDB collection.  The name of the queue will match the MongoDB collection name.

Queue names must be alphanumeric, without spaces or symbols.

It is a good practice to suffix the queue name with `Queue`

#### Message

A message is the high level object that is a generic definition of a messages.  The message contains processing level information.  The message object automatically created and updated by the Fluent API and should not be used directly by the publisher or subscriber.

#### Data

Data is the user defined information to be passed with the message.  Use data to pass what every information you need to process the message.

The data object must be serializable by MongoDB driver.

It is a good practice to have one queue per data object being passed to limit confusion and to maintain simplicity when subscribing to the queue. 

## Queue Configuration

The queue configuration is used to set default values on messages published to a queue.

An example of using the fluent api to configure the sleep queue.

	MessageQueue.Default.Configure(c => c
        .Connection("MongoMessaging")
        .Queue(s => s
            .Name(SleepMessage.QueueName)
            .Priority(MessagePriority.Normal)
			.ResponseQueue("ReplyQueueName")
			.Retry(5)            
        )
    );

#### Properties

**Connection** is the app.config connection string name used to connect to MongoDB.   

**Name** is the name of the queue to configure.   
**Retry** is the number of times the message should be retried on error. Set to zero, default, to not retry.   
**Priority** is the default priority to publish the message with.    
**ResponseQueue** is the name of the queue where responses should be sent.    

## Publish Message

To publish a message to a queue, use the fluent api.

	var message = MessageQueue.Default.Publish(m => m
        .Queue(SleepMessage.QueueName)
        .Data(sleepMessage)
        .Correlation("321B4671-3B4C-4B97-8E81-D6A8CF22D4F0")
        .Description("User friendly description of the message")
        .Priority(MessagePriority.Normal)
        .Retry(1)                
    ).Result;

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

To subscribe a message to a queue, use the fluent api.

    MessageQueue.Default.Configure(c => c
        .Connection("MongoMessaging")
        .Subscribe(s => s
            .Queue(SleepMessage.QueueName)
            .Handler<SleepHandler>()
            .Workers(4)
        )
    );

#### Properties

*Required*   
**Queue** is the name of the queue to publish to.   
**Handler** is the class that implements IMessageSubscriber.  This is what processes the message.         

*Optional*   
**Workers** is the number of worker processes.   
**ExpireError** is how long to keep error messages.      
**ExpireWarning** is how long to keep warning messages.   
**ExpireSuccessful** is how long to keep successful messages.   
**PollTime** is the amount of time between work polling.   
**Retry** is a class that implements IMessageRetry.  IMessageRetry controls if an error message should be retried.   
**Timeout** is the amount of time before a processing message times out.   
**TimeoutAction** is how to handle timed out messages. Options are Fail or Retry.   

## Message Service

In order for the message subscribers to process messages off queue, the `MessageService` needs to be created and `Start` called. Note, the `MessageService.Stop()` method tries to gracefully stop by waiting for active processes to finish.

	_messageService = new MessageService();

	// on service or application start
	_messageService.Start();

	// on service stop.  
	_messageService.Stop();


## IMessageSubscriber Interface

The following is a sample implementation of `IMessageSubscriber`

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


## IMessageRetry Interface

The following is the default implementation of `IMessageRetry`

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
