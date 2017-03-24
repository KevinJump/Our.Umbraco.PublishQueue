# Our.Umbraco.PublishQueue
Simple Publication Queue for Umbraco 

Adds a "Sent to Publication" option to the context menu of a content node so you can background publish a page or a tree of pages. 


Runs via a db table, and a api controller to process the publishes in the background.

## Scheduled task
if you want the publish queue to truely run in the background you need to add a scheduled task to your umbracosettings.config.

this needs to know the host name so can't realiably be added as part of the package (it probibly can but i haven't done all the checking yet.)

    <task log="true" alias="publishQueue" interval="120" 
          url="http://localhost:54964/umbraco/api/PublishQueueSchedule/ProcessQueue?throttle=250" />


## Dashboard
The queue can also be monitored via a dashboard in the settings section, if you think this is the wrong place for this (like should it really be on the content section?) then do drop me a line, i am unsure about location too. 


## Api

The project also adds an api so you can add stuff to the queue in your own code. 

if you have a refrence to the content service:

```contentService.QueueForPublish(node);```

will add a node to the queue for publishing.

```contentService.QueueForUnPublish(child);```

will add a node for unpublishing 

you can also hit the publishing queue direct via it's singleton

```PublishQueueContext.Current.QueueService.Enqueue(key, name, userId, action);```

at the moment supported actions are publish, unpublish, save, delete (but not trash)
 









