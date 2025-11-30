using CSS.Services;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class AamarPayService
{
    private readonly AamarPaySettings _cfg;
    private readonly HttpClient _http;

    public AamarPayService(IOptions<AamarPaySettings> cfg, IHttpClientFactory httpFactory)
    {
        _cfg = cfg.Value;
        _http = httpFactory.CreateClient();
    }

    private string InitUrl => _cfg.UseSandbox ? _cfg.SandboxInitUrl : _cfg.LiveInitUrl;
    private string TrxCheckUrl => _cfg.UseSandbox ? _cfg.SandboxTrxCheckUrl : _cfg.LiveTrxCheckUrl;

    // Initiate payment -> returns object containing request_id/payment_url (depends on response)
    public async Task<JsonElement?> InitiatePaymentAsync(string tranId, decimal amount, string cusName,
        string cusPhone, string cusEmail, string desc, string successUrl, string failUrl, string cancelUrl)
    {
        var payload = new Dictionary<string, object?>
        {
            {"store_id", _cfg.StoreId},
            {"signature_key", _cfg.SignatureKey},
            {"tran_id", tranId},
            {"amount", amount.ToString("0.00")},
            {"currency", "BDT"},
            {"desc", desc},
            {"cus_name", cusName},
            {"cus_email", cusEmail},
            {"cus_phone", cusPhone},
            {"success_url", successUrl},
            {"fail_url", failUrl},
            {"cancel_url", cancelUrl},
            {"type", "json"}
        };

        var json = JsonSerializer.Serialize(payload);
        using var req = new HttpRequestMessage(HttpMethod.Post, InitUrl);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var res = await _http.SendAsync(req);
        var txt = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode) return null;

        try
        {
            var doc = JsonSerializer.Deserialize<JsonElement>(txt);
            return doc;
        }
        catch
        {
            return null;
        }
    }

    // Check transaction status by request_id or tran_id (docs use request_id param)
    public async Task<JsonElement?> CheckTransactionAsync(string requestId)
    {
        // Example: ?request_id=XXXX&store_id=XXXX&signature_key=XXXX&type=json
        var q = $"{TrxCheckUrl}?request_id={Uri.EscapeDataString(requestId)}&store_id={Uri.EscapeDataString(_cfg.StoreId)}&signature_key={Uri.EscapeDataString(_cfg.SignatureKey)}&type=json";
        using var res = await _http.GetAsync(q);
        var txt = await res.Content.ReadAsStringAsync();
        try
        {
            var doc = JsonSerializer.Deserialize<JsonElement>(txt);
            return doc;
        }
        catch
        {
            return null;
        }
    }
}
