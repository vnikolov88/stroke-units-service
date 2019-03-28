using System.Threading;
using System.Threading.Tasks;

namespace stroke_units_service.Services
{
    public interface ILocationService
    {
        Task<(double Latitude, double Longitude)> GetLocationAsync(string address, CancellationToken cancellationToken);
    }
}
