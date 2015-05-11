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
        private readonly int _vendorCount;

        public PurchaseEngine(float totalAmount, List<Vendor> vendorList)
        {
            VendorList = vendorList;
            _totalAmount = totalAmount;
            _vendorCount = vendorList.Count;
        }

        public List<List<float>> Run(float priceLimit, float qLimit, float vadLimit, float sadLimit)
        {
            SupplyDistribution.TotalAmount = _totalAmount;
            SupplyDistribution.TotalVendorCount = VendorList.Count;
            SupplyDistribution.VendorList = VendorList;
            SupplyDistribution.PriceLimit = priceLimit;
            SupplyDistribution.QLimit = qLimit;
            SupplyDistribution.VLimit = vadLimit;
            SupplyDistribution.SLimit = sadLimit;
            SupplyDistribution.ActiveDistribute = new float[VendorList.Count];
            SupplyDistribution.Distribute(_totalAmount, VendorList);
            
            return SupplyDistribution.CandidateList;
        } 
    }
}
