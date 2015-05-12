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
