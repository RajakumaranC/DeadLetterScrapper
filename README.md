# Dead Letter Scrapper 

## Deadletter Message Scrapper
Deadlettter scrapper is a console based app to connect to your Azure Service bus to Peek/Recieve deadletter messages and store it in a file/SQL Service db for analysing the data. 

Note: This app doesn't process your messages, it simply recieves the message and stores it in a place for easy access. 

### Motivation: 

The main motivation for this project is that if you have a Az ServiceBus topic where the deadletter messages are accumulating, there is no easy way to check the deadletter messages why it wasn't processed using service bus explorer. This is because ServiceBus explorer is not easy to work with if you have many messages in the DLQ. This app aims to simply peek/recieve these messages and store it in a convenent way to analyze it. 

### How to run this tool. 
Use the help mechanism to figure out what parameters can be used with the tool. 
```
deadletterscrapper.exe -h
```

Basic option to run the tool
```
deadletterscrapper.exe -c <connection_string_of_SB> -t <topic_name> -s <subscirption_name> -b <batch_size> -p <path_to_store_file>
```

### Storing Data in CSV file: 
CSV file is the default way of storing messages. If path to store wasn't mentioned, the scrapped messages are stored in a csv file on the same directory of the tool. You can control this using the -p flag and mention a custom directory to store. This directory must exisit or an error will be thrown. 

### Storing Data in SQLSever: 
When using SQL for datastore, make sure the database has a table named 'DeadLetterMessages' with the requried fields. You can use this table creation script below to create the required sturcture. 

```sql
Create Table DeadLetterMessages 
(
Id nvarchar(50) not null,
SBNamespace nvarchar(100) not null, 
TopicName nvarchar(100) not null, 
SubscriptionName nvarchar(100) not null, 
DeadletterErrorDescription nvarchar(100) not null,
DeadletterSource nvarchar(100) null, 
DeadletterReason nvarchar(100) null, 
EnqueueTime datetime not null, 
EnqueueSequenceNumber bigint not null, 
Reciepent nvarchar(100) null, 
Body  nvarchar(max) null
);
```
