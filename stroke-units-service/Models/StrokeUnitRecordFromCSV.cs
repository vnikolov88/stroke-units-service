using stroke_units_service.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace stroke_units_service.Models
{
    public class StrokeUnitRecordFromCSV : IStrokeUnitRecord
    {
        public StrokeUnitRecordFromCSV(StrokeUnitRecordFromCSV copy)
        {
            Name = new string(copy.Name);
            Street = new string(copy.Street);
            PostCode = new string(copy.PostCode);
            City = new string(copy.City);
            Latitude = copy.Latitude;
            Longitude = copy.Longitude;
        }

        public StrokeUnitRecordFromCSV(string lineFromCSV)
        {
            var components = lineFromCSV.Split(';');

            Action<string>[] mappingTableForAddress = {
                x => PostCode = x,
                x => City = x
            };

            Action<string>[] mappingTableForLine = {
                x => { },
                x => Name = x,
                x => Street = x,
                x => ExecuteMappingTable(mappingTableForAddress, x.Split()),
                x => { },
                x => { }
            };
            ExecuteMappingTable(mappingTableForLine, components);
        }

        private void ExecuteMappingTable(Action<string>[] table, string[] elements)
        {
            for (var i = 0; i < elements.Length && i < table.Length; ++i)
            {
                table[i](elements[i]);
            }
        }

        public async Task<IStrokeUnitRecord> WithLocationAsync(ILocationService locationService, CancellationToken cancellationtoken)
        {
            var result = new StrokeUnitRecordFromCSV(this);
            (result.Latitude, result.Longitude) = await locationService.GetLocationAsync(Location, cancellationtoken);
            return result;
        }

        public string Name { get; set; }
        public string Street { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Location { get => $"{City}, {PostCode} {Street}"; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
    }
}
