using System;
using System.Collections.Generic;

namespace ElectricityFactory
{
    public class PurchaseEngine
    {
        public List<Vendor> VendorList { get; set; }
        private readonly float _totalAmount;
        public event EventHandler<MessageEventArgs> OnNotifitify;
        public event EventHandler<FinishEventArgs> OnFinished;

        public PurchaseEngine(float totalAmount, List<Vendor> vendorList)
        {
            VendorList = vendorList;
            _totalAmount = totalAmount;
        }

        public void RunAsync(float priceLimit, float qLimit, float vadLimit, float sadLimit)
        {
            SupplyDistribution.CandidateList = new List<PurchaseCandidate>();
            SupplyDistribution.StopComputing = false;
            SupplyDistribution.TotalAmount = _totalAmount;
            SupplyDistribution.TotalVendorCount = VendorList.Count;
            SupplyDistribution.PriceLimit = priceLimit;
            SupplyDistribution.QLimit = qLimit;
            SupplyDistribution.VLimit = vadLimit;
            SupplyDistribution.SLimit = sadLimit;
            SupplyDistribution.ActiveDistribute = new float[VendorList.Count];
            SupplyDistribution.CandidateThreshold = 50;

            List<Vendor> shuffledVendorList = new List<Vendor>();
            int times = 1;
            while (SupplyDistribution.CandidateList.Count == 0 && times <= 10)
            {
                OnNotifitify(this,new MessageEventArgs(@"尝试 "+times+" 次..."));
                shuffledVendorList = SupplyDistribution.ShuffleList(VendorList);
                SupplyDistribution.VendorList = shuffledVendorList;
                SupplyDistribution.StartTime = DateTime.Now;
                SupplyDistribution.StopComputing = false;
                SupplyDistribution.Distribute(_totalAmount, shuffledVendorList);
                times++;
            }

            var unshuffledCandidateList = AppendToCandidateList(shuffledVendorList);

            OnFinished(this, new FinishEventArgs(unshuffledCandidateList));
        }

        private List<PurchaseCandidate> AppendToCandidateList(List<Vendor> shuffledVendorList)
        {
            List<PurchaseCandidate> unshuffledCandidateList = new List<PurchaseCandidate>();

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
                List<float> unshuffledCandidates = new List<float>();
                foreach (int originalIndex in shuffleReverseOrder)
                {
                    unshuffledCandidates.Add(candidate.PurchaseAmountList[originalIndex]);
                }
                candidate.PurchaseAmountList = unshuffledCandidates;
                unshuffledCandidateList.Add(candidate);
            }
            return unshuffledCandidateList;
        }
    }

    public class FinishEventArgs : EventArgs
    {
        public List<PurchaseCandidate> Candidates { get; set; }

        public FinishEventArgs(List<PurchaseCandidate> candidates)
        {
            Candidates = candidates;
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string Msg { get; set; }
        public MessageEventArgs(string msg)
        {
            Msg = msg;
        }
    }
}
