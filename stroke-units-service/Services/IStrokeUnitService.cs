using Microsoft.Extensions.Primitives;
using stroke_units_service.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace stroke_units_service.Services
{
    public interface IStrokeUnitService
    {
        IChangeToken Watch(string fileName);
        Task<IEnumerable<IStrokeUnitRecord>> GetRecordsAsync(string filename, CancellationToken cancellationToken);
        Task<IEnumerable<IStrokeUnitRecord>> GetRecordsWithLocationAsync(string filename, CancellationToken cancellationToken);
    }
}
