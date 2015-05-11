using System;
using System.Collections.Generic;
using ElectricityFactory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class SupplyDistributionTest
    {
        [TestMethod]
        public void print_all_possible_supplies()
        {
            List<Vendor> vendorList = new List<Vendor>();
            for (int i = 0; i < 12; i++)
            {
                vendorList.Add(new Vendor(){MinPurchase = 12%5+2,MaxPurchase = 30});
            }

            var distributeList = SupplyDistribution.Distribute(60, vendorList);
            Console.WriteLine(distributeList.Count);
            foreach (float[] supply in distributeList)
            {
                string supplyString = "[";
                foreach (float supplyElement in supply)
                {
                    supplyString += supplyElement.ToString() + ',';
                }
                supplyString += "]";

                Console.WriteLine(supplyString);
            }
        }
    }
}
