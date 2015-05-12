using System;
using System.Collections.Generic;
using ElectricityFactory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class PurchaseEngineTest
    {
        [TestMethod]
        public void print_all_candidates()
        {

            SupplyDistribution.minStep = 5;

            List<int> pricePerUnit = new List<int>(new []{512,489,501,503,432,479,513,600,405,528,543,459});
            List<int> qnetPerUnit = new List<int>(new []{5678 ,6081 ,5607 ,5642 ,5510 ,5776 ,4397 ,5204 ,4974 ,4793 ,5110 ,5012 });
            List<float> vadPerUnit = new List<float>(new []{11.50f ,21.86f ,15.82f ,7.91f ,28.35f ,27.06f ,21.33f ,11.23f ,26.27f ,26.84f ,30.00f ,29.50f });
            List<float> sadPerUnit = new List<float>(new []{0.49f ,1.52f ,2.15f ,1.11f ,0.60f ,0.61f ,0.51f ,0.53f ,0.56f ,0.57f ,1.78f ,1.42f });

            List<int> minLimit = new List<int>(new[] { 7, 4, 3, 1, 6, 2, 4, 3, 5, 3, 0, 1 });
            List<int> maxLimit = new List<int>(new[] { 30, 20, 35, 50, 60, 34, 42, 26, 55, 32, 51, 47});
            

            List<Vendor> vendorList = new List<Vendor>();
            for (int i = 0; i < pricePerUnit.Count; i++)
            {
                Vendor vendor = new Vendor();
                vendor.PricePerUnit = pricePerUnit[i];
                vendor.QnetPerUnit = qnetPerUnit[i];
                vendor.VadPerUnit = vadPerUnit[i];
                vendor.SadPerUnit = sadPerUnit[i];
                vendor.MinPurchase = minLimit[i];
                vendor.MaxPurchase = maxLimit[i];
                vendorList.Add(vendor);
            }
            
            PurchaseEngine engine = new PurchaseEngine(60,vendorList);
            var list = engine.RunAsync(500, 5000, 23, 1.2f);
            Console.WriteLine(list.Count);
            foreach (List<int> candidate in list)
            {
                string candidateString = "[";
                foreach (var amount in candidate)
                {
                    candidateString += amount.ToString() + ',';
                }
                candidateString += "]";
                Console.WriteLine(candidateString);
            }
        }
    }
}
