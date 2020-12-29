using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace AzureServiceBusMessages.Receiver
{
    class Program
    {

        private static QueueClient QueueClient;

        static void Main(string[] args)
        {
            Thread.Sleep(5000);
            Console.WriteLine("Press enter to Receive Orders!");
            Console.ReadLine();

            ReceiveAndProcessOrders(1);
            //ReceiveAndDontProcessOrders(1);
        }

        static void ReceiveAndProcessOrders(int threads)
        {
            WriteLine($"ReceiveAndProcessOrder({ threads })", ConsoleColor.Cyan);
            // Create a new client
            QueueClient = new QueueClient(Settings.ConnectionString, Settings.QueueName);

            // Set the options for the message handler
            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                MaxAutoRenewDuration = TimeSpan.FromSeconds(30)
            };

            // Create a message pump using OnMessage
            QueueClient.RegisterMessageHandler(ProcessTextMessageAsync, options);


            WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
            Console.ReadLine();
            StopReceivingAsync().Wait();
        }

        static void ReceiveAndDontProcessOrders(int threads)
        {
            WriteLine($"ReceiveAndDontProcessOrder({ threads })", ConsoleColor.Cyan);
            // Create a new client
            QueueClient = new QueueClient(Settings.ConnectionString, Settings.QueueName);

            // Set the options for the message handler
            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                MaxAutoRenewDuration = TimeSpan.FromSeconds(30)
            };

            // Create a message pump using OnMessage
            QueueClient.RegisterMessageHandler(DontProcessTextMessageAsync, options);


            WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
            Console.ReadLine();
            StopReceivingAsync().Wait();
        }


        static async Task ProcessTextMessageAsync(Message message, CancellationToken token)
        {
            // Deserialize the message body.
            var messageBodyText = Encoding.UTF8.GetString(message.Body);

            var receivedOrder = JsonConvert.DeserializeObject<NutOrder>(messageBodyText);

            WriteLine("Received: ", ConsoleColor.Yellow);
            Write($"{ receivedOrder.ToString() } \n", ConsoleColor.Green);

            // Complete the message
            await QueueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static async Task DontProcessTextMessageAsync(Message message, CancellationToken token)
        {
            // Deserialize the message body.
            var messageBodyText = Encoding.UTF8.GetString(message.Body);

            WriteLine("Received: ", ConsoleColor.Yellow);
            Write($"{ messageBodyText } \n", ConsoleColor.Green);

            throw new Exception("Error;");
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            WriteLine(exceptionReceivedEventArgs.Exception.Message, ConsoleColor.Red);
            return Task.CompletedTask;
        }

        static async Task StopReceivingAsync()
        {
            // Close the client, which will stop the message pump.
            await QueueClient.CloseAsync();
        }

        private static void WriteLine(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = tempColor;
        }

        private static void Write(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = tempColor;
        }
    }
}
