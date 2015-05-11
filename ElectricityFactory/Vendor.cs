using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityFactory
{
    public class Vendor
    {
        public string Name { get; set; }
        public float PricePerUnit { get; set; }
        public float QnetPerUnit { get; set; }
        public float VadPerUnit { get; set; }
        public float SadPerUnit { get; set; }
        public int MinPurchase { get; set; }
        public int MaxPurchase { get; set; }
        public float PurchaseUnit { get; set; }
    }
}
