using Covenant.Common.Entities.Agency;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Location;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Mappers;

public static class AgencyExtensionsMapping
{
    public static Expression<Func<Agency, AgencyModel>> SelectAgency = a => new AgencyModel
    {
        Id = a.Id,
        FullName = a.FullName,
        HstNumber = a.HstNumber,
        BusinessNumber = a.BusinessNumber,
        WebPage = a.WebPage,
        PhonePrincipal = a.PhonePrincipal,
        PhonePrincipalExt = a.PhonePrincipalExt,
        AgencyType = a.AgencyType,
        Logo = a.Logo == null ? null : new CovenantFileModel
        {
            Id = a.Logo.Id,
            FileName = a.Logo.FileName,
            Description = a.Logo.Description
        },
        WsibGroup = a.WsibGroup.Select(w => new BaseModel<Guid>
        {
            Id = w.WsibGroup.Id,
            Value = w.WsibGroup.Value
        }),
        Locations = a.Locations.Select(l => new LocationDetailModel
        {
            Id = l.LocationId,
            Address = l.Location.Address,
            PostalCode = l.Location.PostalCode,
            City = new CityModel
            {
                Id = l.Location.City.Id,
                Value = l.Location.City.Value,
                Code = l.Location.City.Code,
                Province = new ProvinceModel
                {
                    Id = l.Location.City.Province.Id,
                    Value = l.Location.City.Province.Value,
                    Code = l.Location.City.Province.Code,
                    Country = new CountryModel
                    {
                        Id = l.Location.City.Province.Country.Id,
                        Value = l.Location.City.Province.Country.Value,
                        Code = l.Location.City.Province.Country.Code
                    }
                }
            }
        }),
        ContactInformation = a.ContactInformation.Select(c => new AgencyContactInformationModel
        {
            Id = c.Id,
            Title = c.Title,
            FirstName = c.FirstName,
            MiddleName = c.MiddleName,
            LastName = c.LastName,
            MobileNumber = c.MobileNumber,
            OfficeNumber = c.OfficeNumber,
            OfficeNumberExt = c.OfficeNumberExt,
            Email = c.Email,
            Position = c.Position
        })
    };

    public static Expression<Func<AgencyLocation, LocationDetailModel>> SelectLocation = s => new LocationDetailModel
    {
        Id = s.LocationId,
        City = new CityModel
        {
            Id = s.Location.CityId,
            Value = s.Location.City.Value,
            Code = s.Location.City.Code,
            Province = new ProvinceModel
            {
                Id = s.Location.City.Province.Id,
                Value = s.Location.City.Province.Value,
                Code = s.Location.City.Province.Code,
                Country = new CountryModel
                {
                    Id = s.Location.City.Province.Country.Id,
                    Value = s.Location.City.Province.Country.Value,
                    Code = s.Location.City.Province.Country.Code
                }
            }
        },
        Address = s.Location.Address,
        PostalCode = s.Location.PostalCode,
        IsBilling = s.IsBilling,
        Latitude = s.Location.Latitude,
        Longitude = s.Location.Longitude,
    };
}
