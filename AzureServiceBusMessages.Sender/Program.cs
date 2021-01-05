using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus.Management;
using System.Collections.Generic;

namespace AzureServiceBusMessages.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            RecreateQueueAsync().Wait();

            Console.WriteLine("Welcome to Godoy's NutShop! Whats your order?");
            Console.WriteLine("Press enter to order Peanuts!"); 
            Console.ReadLine();

            //SendSingleOrder();
            SendMultipleOrder(1000);

        }

        static void SendMultipleOrder(int count)
        {
            int id = 0;
            var randomNut = new Random();

            List<NutOrder> orders = new List<NutOrder>();

            for(int i = 0; i < count; i++)
            {
                int nutType = randomNut.Next(0, 4);
                int weight = randomNut.Next(0, 5);

                NutOrder nutOrder = new NutOrder()
                { Id = id, Merchant = "Tungo", DeliveryAddress = "Rua Feliz 123", NutType = (NutType)nutType, Weight = weight };

                orders.Add(nutOrder);
            }

            SimpleOrderSenderAsync(orders).Wait();
                
        }

        static void SendSingleOrder()
        {
            int id = 0;
            var randomNut = new Random();

            do
            {
                int nutType = randomNut.Next(0, 4);
                int weight = randomNut.Next(0, 5);

                NutOrder nutOrder = new NutOrder()
                { Id = id, Merchant = "Tungo", DeliveryAddress = "Rua Feliz 123", NutType = (NutType)nutType, Weight = weight };

                SimpleOrderSenderAsync(nutOrder).Wait();
                id++;
            }
            while (Console.ReadLine().Equals("+"));
        }

        static async Task SimpleOrderSenderAsync(NutOrder nutOrder)
        {
            WriteLine("Sending NutOrder to Godoy's Shop", ConsoleColor.Cyan);
            WriteLine($"{nutOrder.ToString()}", ConsoleColor.DarkYellow);

            // Create a client
            var client = new QueueClient(Settings.ConnectionString, Settings.QueueName);

            try
            {
                WriteLine("Sending...", ConsoleColor.Green);

                string jsonNutOrder = JsonConvert.SerializeObject(nutOrder);

                var message = new Message(Encoding.UTF8.GetBytes(jsonNutOrder));
                await client.SendAsync(message);

                WriteLine("Done!", ConsoleColor.Green);

                Console.WriteLine();
            }
            finally
            {
                // Always close the client
                await client.CloseAsync();
            }            

        }

        static async Task SimpleOrderSenderAsync(List<NutOrder> nutOrder)
        {
            WriteLine("Sending NutOrder to Godoy's Shop", ConsoleColor.Cyan);
            //WriteLine($"{nutOrder.ToString()}", ConsoleColor.DarkYellow);

            // Create a client
            var client = new QueueClient(Settings.ConnectionString, Settings.QueueName);

            try
            {
                WriteLine("Sending...", ConsoleColor.Green);

                List<Message> messages = ObjectToMessages(nutOrder);

                await client.SendAsync(messages);

                WriteLine("Done!", ConsoleColor.Green);

                Console.WriteLine();
            }
            finally
            {
                // Always close the client
                await client.CloseAsync();
            }

        }

        static List<Message> ObjectToMessages<T>(List<T> objects)
        {
            List<Message> messages = new List<Message>();

            foreach(T obj in objects)
            {
                string jsonNutOrder = JsonConvert.SerializeObject(obj);

                var message = new Message(Encoding.UTF8.GetBytes(jsonNutOrder));

                messages.Add(message);
            }

            return messages;
        }

        static async Task RecreateQueueAsync()
        {
            var queueDescription = new QueueDescription(Settings.QueueName)
            {
                MaxDeliveryCount = 5,
                LockDuration = TimeSpan.FromMinutes(5)
            };

            var manager = new ManagementClient(Settings.ConnectionString);
            if (await manager.QueueExistsAsync(Settings.QueueName))
            {
                WriteLine($"Deleting queue: { Settings.QueueName }...", ConsoleColor.Magenta);
                await manager.DeleteQueueAsync(Settings.QueueName);
                WriteLine("Done!", ConsoleColor.Magenta);
            }

            WriteLine($"Creating queue: { Settings.QueueName }...", ConsoleColor.Magenta);
            await manager.CreateQueueAsync(queueDescription);
            WriteLine("Done!", ConsoleColor.Magenta);
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
