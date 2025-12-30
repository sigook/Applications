using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Xunit;

namespace Covenant.Tests.Common
{
    public class LocationTest
    {
        [Fact]
        public void Create()
        {
            var cityId = Guid.NewGuid();
            const string address = "419 dundas west";
            const string postalCode = "M1P1M1";
            const string entrance = "Main entrance";
            const string mainIntersection = "Dundas and bloor";
            Result<Location> result = Location.Create(cityId, address, postalCode, entrance, mainIntersection);
            Assert.True(result);
            Assert.Equal(cityId, result.Value.CityId);
            Assert.Equal(address, result.Value.Address);
            Assert.Equal(postalCode, result.Value.PostalCode);
            Assert.Equal(entrance, result.Value.Entrance);
            Assert.Equal(mainIntersection, result.Value.MainIntersection);
        }

        [Fact]
        public void Update()
        {
            Location location = FakeLocation;

            Location newLocation = Location.Create(Guid.NewGuid(), "270 Orenda Rd",
                "L6T4X6", "Back door", "Orenda and dixie").Value;

            location.Update(newLocation);
            Assert.Equal(newLocation.CityId, location.CityId);
            Assert.Equal(newLocation.Address, location.Address);
            Assert.Equal(newLocation.PostalCode, location.PostalCode);
            Assert.Equal(newLocation.Entrance, location.Entrance);
            Assert.Equal(newLocation.MainIntersection, location.MainIntersection);
        }

        [Fact]
        public void UpdatePostalCode()
        {
            Location location = FakeLocation;
            location.UpdateCoordinates(Location.DefaultLatitude, Location.DefaultLongitude);

            CvnPostalCode newPostalCode = CvnPostalCode.Create("M2P2M2").Value;

            location.UpdatePostalCode(newPostalCode);

            Assert.Equal(newPostalCode.PostalCode, location.PostalCode);
        }

        [Fact]
        public void UpdateCoordinates()
        {
            Location location = FakeLocation;
            Assert.Null(location.Latitude);
            Assert.Null(location.Longitude);

            const double latitude = Location.DefaultLatitude;
            const double longitude = Location.DefaultLongitude;
            location.UpdateCoordinates(latitude, longitude);
            Assert.Equal(latitude, location.Latitude);
            Assert.Equal(longitude, location.Longitude);
        }

        private static Location FakeLocation => Location.Create(Guid.NewGuid(), "555 dundas west",
            "M1P1M1", "Main entrance", "Dundas and bloor").Value;
    }
}