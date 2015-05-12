using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityFactory
{
    public class PurchaseEngine
    {
        public List<Vendor> VendorList { get; set; }
        private readonly float _totalAmount;

        public PurchaseEngine(float totalAmount, List<Vendor> vendorList)
        {
            VendorList = vendorList;
            _totalAmount = totalAmount;
        }

        public List<List<float>> Run(float priceLimit, float qLimit, float vadLimit, float sadLimit)
        {
            SupplyDistribution.CandidateList = new List<List<float>>();
            SupplyDistribution.StopComputing = false;
            SupplyDistribution.TotalAmount = _totalAmount;
            SupplyDistribution.TotalVendorCount = VendorList.Count;
            SupplyDistribution.VendorList = VendorList;
            SupplyDistribution.PriceLimit = priceLimit;
            SupplyDistribution.QLimit = qLimit;
            SupplyDistribution.VLimit = vadLimit;
            SupplyDistribution.SLimit = sadLimit;
            SupplyDistribution.ActiveDistribute = new float[VendorList.Count];
            SupplyDistribution.CandidateThreshold = 100;

            List<Vendor> shuffledVendorList = new List<Vendor>();
            while (SupplyDistribution.CandidateList.Count == 0)
            {
                shuffledVendorList = SupplyDistribution.ShuffleList(VendorList);
                SupplyDistribution.StartTime = DateTime.Now;
                SupplyDistribution.StopComputing = false;
                SupplyDistribution.Distribute(_totalAmount, shuffledVendorList);
            }

            List<List<float>> unshuffledCandidateList = new List<List<float>>();

            List<int> shuffleReverseOrder = new List<int>();

            foreach (Vendor vendor in VendorList)
            {
                for (int n = 0; n < shuffledVendorList.Count; n++)
                {
                    if (vendor.Name == shuffledVendorList[n].Name)
                    {
                        shuffleReverseOrder.Add(n);
                        break;
                    }
                }
            }

            foreach (var candidate in SupplyDistribution.CandidateList)
            {
                List<float> unshuffledCandidate = shuffleReverseOrder.Select(originalIndex => candidate[originalIndex]).ToList();
                unshuffledCandidateList.Add(unshuffledCandidate);
            }
            return unshuffledCandidateList;
        }
    }
}
