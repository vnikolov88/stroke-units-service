using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using StrokeUnitsService.Models;
using StrokeUnitsService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrokeUnitsService.Api.V1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StrokeUnitsController : ControllerBase
    {
        private readonly StartupOptions _options;
        private readonly ILocationService _locationService;
        private readonly IStrokeUnitService _strokeUnitsService;

        public StrokeUnitsController(
            IOptions<StartupOptions> options,
            ILocationService locationService,
            IStrokeUnitService strokeUnitsService
            )
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _strokeUnitsService = strokeUnitsService ?? throw new ArgumentNullException(nameof(strokeUnitsService));
        }

        [HttpGet("cannary")]
        public IActionResult Cannary() => Ok();

        [HttpGet]
        public IEnumerable<IStrokeUnitRecord> Get(CancellationToken cancellationToken)
        {
            return _options.DataFiles.SelectMany(dataFile => _strokeUnitsService.GetRecordsAsync(dataFile, cancellationToken).GetAwaiter().GetResult()).ToArray();
        }

        [HttpGet("with-location")]
        public async Task<IEnumerable<IStrokeUnitRecord>> Get(
            [BindRequired, FromQuery]string address,
            CancellationToken cancellationToken)
        {
            var decodedAddress = Encoding.UTF8.GetString(Convert.FromBase64String(address));
            var location = await _locationService.GetLocationAsync(decodedAddress, cancellationToken);
            var searchlocation = new GeoCoordinate(location.Latitude, location.Longitude);

            var allStrokeUnits = _options.DataFiles.SelectMany(
                dataFile => _strokeUnitsService.GetRecordsWithLocationAsync(dataFile, cancellationToken)
                .GetAwaiter()
                .GetResult()
                ).ToArray();

            return allStrokeUnits.OrderBy(x => new GeoCoordinate(x.Latitude, x.Longitude).GetDistanceTo(searchlocation)).ToArray();
        }
    }
}
