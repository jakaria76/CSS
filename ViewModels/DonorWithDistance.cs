using CSS.Models;

namespace CSS.ViewModels
{
    public class DonorWithDistance
    {
        public ApplicationUser User { get; set; } = default!;
        public double DistanceKm { get; set; }
    }
}
