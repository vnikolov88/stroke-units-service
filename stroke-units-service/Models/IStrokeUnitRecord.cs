using StrokeUnitsService.Services;
using System.Threading;
using System.Threading.Tasks;

namespace StrokeUnitsService.Models
{
    public interface IStrokeUnitRecord
    {
        string Name { get; set; }
        string Street { get; set; }
        string PostCode { get; set; }
        string City { get; set; }

        #region Location part of the interface
        Task<IStrokeUnitRecord> WithLocationAsync(ILocationService locationService, CancellationToken cancellationtoken);
        string Location { get; }
        double Latitude { get; }
        double Longitude { get; }
        #endregion Location part of the interface
    }
}
