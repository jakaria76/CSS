namespace CSS.ViewModels
{
    public class BloodGroupVM
    {
        public string BloodGroup { get; set; } = default!;
        public int Total { get; set; }
        public int Available { get; set; }
        public int NotAvailable { get; set; }
    }
}
