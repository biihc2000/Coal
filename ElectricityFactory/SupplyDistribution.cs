using System;
using System.Collections.Generic;

namespace ElectricityFactory
{
    public class SupplyDistribution
    {
        private static Random random = new Random();
        public static float TotalAmount { get; set; }
        public static int TotalVendorCount { get; set; }
        public static List<Vendor> VendorList;
        public static float[] ActiveDistribute { get; set; }
        public static float PriceLimit { get; set; }
        public static float QLimit { get; set; }
        public static float VLimit { get; set; }
        public static float SLimit { get; set; }
        public static int CandidateThreshold { get; set; }
        public static List<PurchaseCandidate> CandidateList = new List<PurchaseCandidate>();
        public static int Timeout = 3;

        public static bool StopComputing { get; set; }
        public static DateTime StartTime { get; set; }

        public static void Distribute(float total, List<Vendor> vendorList)
        {
            if (StopComputing)
                return;

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

            possibleDistributes = ShuffleList(possibleDistributes);

            if (vendorList.Count == 1)
            {
                ActiveDistribute[TotalVendorCount - vendorList.Count] = Convert.ToSingle(Math.Ceiling(total/currentVendor.PurchaseUnit) * currentVendor.PurchaseUnit);
                float totalPrice = 0;
                float totalQ = 0;
                float totalVad = 0;
                float totalSad = 0;
                float totalAmount = 0;
                for (int i = 0; i < TotalVendorCount; i++)
                {
                    totalAmount += ActiveDistribute[i];
                    totalPrice += VendorList[i].PricePerUnit * ActiveDistribute[i];
                    totalQ += VendorList[i].QnetPerUnit * ActiveDistribute[i];
                    totalVad += VendorList[i].VadPerUnit * ActiveDistribute[i];
                    totalSad += VendorList[i].SadPerUnit * ActiveDistribute[i];
                }

                var averagePrice = totalPrice / totalAmount;
                var averageQ = totalQ / totalAmount;
                var averageVad = totalVad / totalAmount;
                var averageSad = totalSad / totalAmount;

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
                    PurchaseCandidate newCandidate = new PurchaseCandidate();
                    newCandidate.PurchaseAmountList = copy;
                    newCandidate.ActualAveragePrice = averagePrice;
                    newCandidate.ActualAverageQ = averageQ;
                    newCandidate.ActualAverageV = averageVad;
                    newCandidate.ActualAverageS = averageSad;
                    newCandidate.ActualTotalAmount = totalAmount;
                    CandidateList.Add(newCandidate);
                    if (CandidateList.Count >= CandidateThreshold)
                    {
                        StopComputing = true;
                    }
                }

                TimeSpan elipsedTime = DateTime.Now - StartTime;
                Timeout = 3;
                if (elipsedTime.Seconds > Timeout)
                    StopComputing = true;
            }
            else
            {
                int step = 1; //possibleDistributes.Count/5;
                for (int index = 0; index < possibleDistributes.Count; index+=step)
                {
                    if(StopComputing)
                        return;

                    float currentDistribute = possibleDistributes[index];
                    ActiveDistribute[TotalVendorCount - vendorList.Count] = currentDistribute;
                    Distribute(total - currentDistribute, vendorList.GetRange(1, vendorList.Count - 1));
                }
            }
        }

        public static List<T> ShuffleList<T>(List<T> oldList)
        {
            List<T> newList = new List<T>();
            foreach (T item in oldList)
            {
                newList.Insert(random.Next(newList.Count), item);
            }
            return newList;
        }
    }

    public class PurchaseCandidate
    {
        public List<float> PurchaseAmountList { get; set; }
        public float ActualAveragePrice { get; set; }
        public float ActualAverageQ { get; set; }
        public float ActualAverageV { get; set; }
        public float ActualAverageS { get; set; }
        public float ActualTotalAmount { get; set; }
    }
}
