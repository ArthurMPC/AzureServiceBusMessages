using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusMessages
{
    public class NutOrder
    {
        public int Id{ get; set; }
        public decimal Weight { get; set; }
        public NutType NutType { get; set; }
        public string Merchant { get; set; }
        public string DeliveryAddress { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id} | Merchant: {this.Merchant} \n DeliveryAddress: {this.DeliveryAddress} \n NutType: {NutType.ToString()} \n Weight: {Weight} Kg";
        }
    }
}
