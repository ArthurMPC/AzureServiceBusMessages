[![.Net Core 3.1](https://img.shields.io/badge/.NET%20Core-3.1-green)](https://dotnet.microsoft.com/download/dotnet-core/3.1)

# AzureServiceBusMessages

This project is a demo for a application using Azure Service Bus.
The Demo covers some features from ServiceBus like:
  - DeadLeatter Queue
  - Peack-Lock Messaging
  - MaxDeliveryCount
  - Infra creating with code
  - Queue Descriptions

### Pre-requisites

A running ServiceBus on Azure Cloud.

### Running Demo

Using Visual Studio: 

```sh
# FirstStep:
Configure ServiceBusEndpoint and QueuePath on */AzureServiceBusMessages/Settings.cs*

#Second Step: 
Use Multiple Startup Projects for Sender and Receiver.
```

### Azure ServiceBus Package
NuGet: https://www.nuget.org/packages/Microsoft.Azure.ServiceBus
SourceCode: https://github.com/Azure/azure-sdk-for-net/tree/Microsoft.Azure.ServiceBus_5.1.0

