using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using StrokeUnitsService.Models;
using StrokeUnitsService.Services;
using System;
using System.Collections.Generic;
using StrokeUnitsService.Extensions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrokeUnitsService.Api.V2
{
    [Produces("application/json")]
    [Route("api/v2/[controller]")]
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

        [HttpGet("list")]
        public ActionResult<PagedSearch<StrokeUnit>> Get(CancellationToken cancellationToken,
            int page = 1,
            int pageSize = 20)
        {
            var allStrokeUnits = _options.DataFiles.SelectMany(dataFile => _strokeUnitsService.GetRecordsAsync(dataFile, cancellationToken).GetAwaiter().GetResult());

            var result = allStrokeUnits.Select(x => new StrokeUnit
            {
                City = x.City,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
                Location = x.Location,
                PostCode = x.PostCode,
                Street = x.Street,
                DistanceKm = 0
            });

            return result.Page(page, pageSize);
        }

        [HttpGet("with-location")]
        public async Task<ActionResult<PagedSearch<StrokeUnit>>> Get(
            CancellationToken cancellationToken,
            [BindRequired, FromQuery]string address,
            int page = 1,
            int pageSize = 20)
        {
            var decodedAddress = Encoding.UTF8.GetString(Convert.FromBase64String(address));
            var location = await _locationService.GetLocationAsync(decodedAddress, cancellationToken);
            var searchlocation = new GeoCoordinate(location.Latitude, location.Longitude);

            var allStrokeUnits = _options.DataFiles.SelectMany(
                dataFile => _strokeUnitsService.GetRecordsWithLocationAsync(dataFile, cancellationToken)
                .GetAwaiter()
                .GetResult()
                ).ToArray();

            var result = allStrokeUnits.Select(x => new StrokeUnit
            {
                City = x.City,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
                Location = x.Location,
                PostCode = x.PostCode,
                Street = x.Street,
                DistanceKm = new GeoCoordinate(x.Latitude, x.Longitude).GetDistanceTo(searchlocation) / 1000
            });

            return result.OrderBy(x => x.DistanceKm).Page(page, pageSize);
        }
    }
}
