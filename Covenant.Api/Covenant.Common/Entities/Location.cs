using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities
{
    public class Location : ILocation<City>
    {
        public const double DefaultLatitude = 43.6020909;
        public const double DefaultLongitude = -79.7335027;

        public Location()
        {
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Address { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; }
        public string PostalCode { get; set; }
        public string Entrance { get; set; }
        public string MainIntersection { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public virtual string FormattedAddress => $"{Address} {City?.Value} {City?.Province?.Code} {PostalCode}";
        public bool IsUSA => City?.Province?.Country.Code == "USA";

        public void UpdatePostalCode(CvnPostalCode postalCode)
        {
            PostalCode = postalCode.PostalCode;
        }

        public void UpdateCoordinates(double? latitude, double? longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Result<Location> Create(Guid? cityId, string address, string postalCode, string entrance = null, string mainIntersection = null, double? latitud = null, double? longitude = null)
        {
            if (cityId is null || cityId == Guid.Empty) return Result.Fail<Location>("Invalid city");
            var rAddress = CvnAddress.Create(address);
            if (!rAddress) return Result.Fail<Location>(rAddress.Errors);

            var rPostalCode = CvnPostalCode.Create(postalCode);
            if (!rPostalCode) return Result.Fail<Location>(rPostalCode.Errors);

            return Result.Ok(new Location
            {
                CityId = cityId.GetValueOrDefault(),
                Address = rAddress.Value.Address,
                PostalCode = rPostalCode.Value.PostalCode,
                Latitude = latitud,
                Longitude = longitude,
                Entrance = entrance,
                MainIntersection = mainIntersection
            });
        }

        public Result Update(Location location)
        {
            if (location is null) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.Location));
            CityId = location.CityId;
            Address = location.Address;
            Entrance = location.Entrance;
            MainIntersection = location.MainIntersection;
            Latitude = location.Latitude;
            Longitude = location.Longitude;
            var rPostalCode = CvnPostalCode.Create(location.PostalCode);
            if (!rPostalCode) return Result.Fail(rPostalCode.Errors);
            UpdateCoordinates(location.Latitude, location.Longitude);
            UpdatePostalCode(rPostalCode.Value);
            return Result.Ok();
        }
    }
}
