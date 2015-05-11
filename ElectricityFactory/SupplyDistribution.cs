using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityFactory
{
    public class SupplyDistribution
    {
        public static float TotalAmount { get; set; }
        public static int TotalVendorCount { get; set; }
        public static List<Vendor> VendorList;
        public static float[] ActiveDistribute { get; set; }
        public static float PriceLimit { get; set; }
        public static float QLimit { get; set; }
        public static float VLimit { get; set; }
        public static float SLimit { get; set; }
        public static List<List<float>> CandidateList = new List<List<float>>();

        public static void Distribute(float total, List<Vendor> vendorList)
        {
            if (total <= 0)
                return;

            Vendor currentVendor = vendorList[0];
            List<float> possibleDistributes = new List<float>();
            var max = Math.Min(currentVendor.MaxPurchase, total);
            for (float start = currentVendor.MinPurchase; start <= max; start += currentVendor.PurchaseUnit)
            {
                possibleDistributes.Add(start);
                if (start + currentVendor.PurchaseUnit > currentVendor.MaxPurchase)
                    possibleDistributes.Add(currentVendor.MaxPurchase);
            }

            if (vendorList.Count == 1)
            {
                ActiveDistribute[TotalVendorCount - vendorList.Count] = Convert.ToSingle(Math.Ceiling(total/currentVendor.PurchaseUnit) * currentVendor.PurchaseUnit);
                float totalPrice = 0;
                float totalQ = 0;
                float totalVad = 0;
                float totalSad = 0;
                for (int i = 0; i < TotalVendorCount; i++)
                {
                    totalPrice += VendorList[i].PricePerUnit * ActiveDistribute[i];
                    totalQ += VendorList[i].QnetPerUnit * ActiveDistribute[i];
                    totalVad += VendorList[i].VadPerUnit * ActiveDistribute[i];
                    totalSad += VendorList[i].SadPerUnit * ActiveDistribute[i];
                }

                var averagePrice = totalPrice / TotalAmount;
                var averageQ = totalQ / TotalAmount;
                var averageVad = totalVad / TotalAmount;
                var averageSad = totalSad / TotalAmount;

                if (averagePrice <= PriceLimit * 1.03 && averagePrice >= PriceLimit * 0.97
                    && averageQ >= QLimit
                    && averageVad >= VLimit
                    && averageSad <= SLimit)
                {
                    List<float> copy = new List<float>();
                    foreach (float distribute in ActiveDistribute)
                    {
                        copy.Add(distribute);
                    }
                    CandidateList.Add(copy);
                }
            }
            else
            {
                int step = possibleDistributes.Count/5;
                for (int index = 0; index < possibleDistributes.Count; index+=step)
                {
                    float currentDistribute = possibleDistributes[index];
                    ActiveDistribute[TotalVendorCount - vendorList.Count] = currentDistribute;


                    Distribute(total - currentDistribute, vendorList.GetRange(1, vendorList.Count - 1));
                }
            }
        }
    }
}
