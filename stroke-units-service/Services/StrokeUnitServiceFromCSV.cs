using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using stroke_units_service.Models;

namespace stroke_units_service.Services
{
    public class StrokeUnitServiceFromCSV : IStrokeUnitService
    {
        private readonly IMemoryCache _cache;
        private readonly IHostingEnvironment _env;
        private readonly ILocationService _locationService;

        public StrokeUnitServiceFromCSV(
            IMemoryCache cache,
            IHostingEnvironment env,
            ILocationService locationService)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        }

        public async Task<IEnumerable<IStrokeUnitRecord>> GetRecordsAsync(string fileName, CancellationToken cancellationToken)
        {
            return await _cache.GetOrCreateAsync(fileName, async entity => {
                entity.AddExpirationToken(Watch(fileName));

                var lines = await File.ReadAllLinesAsync(fileName, cancellationToken);

                return lines.Select(line => new StrokeUnitRecordFromCSV(line)).ToList<IStrokeUnitRecord>();
            });
        }

        public async Task<IEnumerable<IStrokeUnitRecord>> GetRecordsWithLocationAsync(string fileName, CancellationToken cancellationToken)
        {
            var records = await GetRecordsAsync(fileName, cancellationToken);
            return records?.Select(record => record.WithLocationAsync(_locationService, cancellationToken).GetAwaiter().GetResult()).Where(x => x != null);
        }

        public IChangeToken Watch(string fileName)
        {
            return _env.ContentRootFileProvider.Watch(fileName);
        }
    }
}
