namespace CSS.Services
{
    public class AamarPaySettings
    {
        public string StoreId { get; set; } = "";
        public string SignatureKey { get; set; } = "";
        public bool UseSandbox { get; set; } = true;

        // Sandbox URLs
        public string SandboxInitUrl { get; set; } = "";
        public string SandboxTrxCheckUrl { get; set; } = "";

        // Live URLs
        public string LiveInitUrl { get; set; } = "";
        public string LiveTrxCheckUrl { get; set; } = "";
    }
}
