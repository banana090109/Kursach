namespace BuildStore.Models
{
    public class ElectricalTool : Product
    {
        public int PowerWatts { get; set; }

        public bool BatteryPowered { get; set; }

        public int WarrantyMonths { get; set; }
    }
}