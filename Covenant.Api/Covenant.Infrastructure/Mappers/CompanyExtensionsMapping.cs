using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using Covenant.Company.Models;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Mappers
{
    public static class CompanyExtensionsMapping
    {
        public static Expression<Func<CompanyUser, CompanyUserModel>> SelectCompanyUser = s => new CompanyUserModel
        {
            Id = s.Id,
            Name = s.Name,
            Lastname = s.Lastname,
            MobileNumber = s.MobileNumber,
            Position = s.Position,
            Email = s.User.Email,
            CreatedAt = s.CreatedAt
        };

        public static Expression<Func<CompanyProfileNote, NoteModel>> SelectNote = s => new NoteModel
        {
            Id = s.NoteId,
            Note = s.Note.Note,
            Color = s.Note.Color,
            CreatedAt = s.Note.CreatedAt,
            CreatedBy = s.Note.CreatedBy
        };

        public static Expression<Func<CompanyProfileContactPerson, CompanyProfileContactPersonModel>> SelectContactPerson = c => new CompanyProfileContactPersonModel
        {
            Id = c.Id,
            Title = c.Title,
            FirstName = c.FirstName,
            MiddleName = c.MiddleName,
            LastName = c.LastName,
            Position = c.Position,
            Email = c.Email,
            MobileNumber = c.MobileNumber,
            OfficeNumber = c.OfficeNumber,
            OfficeNumberExt = c.OfficeNumberExt
        };

        public static Expression<Func<CompanyProfileLocation, LocationDetailModel>> SelectCompanyProfileLocationDetail = (cpl) => new LocationDetailModel
        {
            Id = cpl.Location.Id,
            Address = cpl.Location.Address,
            PostalCode = cpl.Location.PostalCode,
            Entrance = cpl.Location.Entrance,
            MainIntersection = cpl.Location.MainIntersection,
            Latitude = cpl.Location.Latitude ?? Location.DefaultLatitude,
            Longitude = cpl.Location.Longitude ?? Location.DefaultLongitude,
            IsBilling = cpl.IsBilling,
            City = new CityModel
            {
                Id = cpl.Location.City.Id,
                Value = cpl.Location.City.Value,
                Code = cpl.Location.City.Code,
                Province = new ProvinceModel
                {
                    Id = cpl.Location.City.Province.Id,
                    Value = cpl.Location.City.Province.Value,
                    Code = cpl.Location.City.Province.Code,
                    Settings = cpl.Location.City.Province.ProvinceSetting == null ? null : new ProvinceSettingsModel
                    {
                        OvertimeStartsAfter = cpl.Location.City.Province.ProvinceSetting.OvertimeStartsAfter.HasValue
                            ? cpl.Location.City.Province.ProvinceSetting.OvertimeStartsAfter.Value.TotalHours
                            : (double?)null,
                        PaidHolidays = cpl.Location.City.Province.ProvinceSetting.PaidHolidays
                    },
                    Country = new CountryModel
                    {
                        Id = cpl.Location.City.Province.Country.Id,
                        Value = cpl.Location.City.Province.Country.Value,
                        Code = cpl.Location.City.Province.Country.Code
                    }
                }
            }
        };
    }
}
