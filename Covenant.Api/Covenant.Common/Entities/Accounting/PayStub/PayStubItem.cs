using Covenant.Common.Enums;
using Covenant.Common.Functionals;

namespace Covenant.Common.Entities.Accounting.PayStub
{
    public class PayStubItem
    {
        public const string DescriptionIsRequired = "Description is required";
        public const string UnitPriceMustBeGreaterThanZero = "Unit price must be greater than zero";
        public const string QuantityMustBeGraterThanZero = "Quantity must be greater than zero";
        public const string RegularHoursLabel = "Regular hours";
        public const string OtherRegularHoursLabel = "Other Regular hours";
        public const string OvertimeHoursLabel = "Overtime hours";
        public const string HolidayPremiumPayHoursLabel = "Holiday premium pay hours";
        public const string NightShiftHoursLabel = "Night Shift hours";
        public const string MissingHoursLabel = "Missing hours";
        public const string MissingOvertimeHoursLabel = "Missing overtime hours";
        public const string BonusOthersLabel = "Bonus/Others";
        public const string ReimbursementLabel = "Reimbursement";

        protected PayStubItem() 
        { 
        }

        private PayStubItem(string description, double quantity, decimal unitPrice, PayStubItemType type, Guid id = default)
        {
            Id = id == default ? Guid.NewGuid() : id;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Total = decimal.Multiply(new decimal(quantity), unitPrice);
            Type = type;
        }

        public Guid Id { get; protected set; }
        public string Description { get; protected set; }
        public double Quantity { get; protected set; }
        public decimal UnitPrice { get; protected set; }
        public decimal Total { get; protected set; }
        public PayStubItemType Type { get; protected set; }
        public Guid PayStubId { get; private set; }
        public PayStub PayStub { get; private set; }

        public static Result<PayStubItem> CreateItem(string description, double quantity, decimal unitPrice, PayStubItemType type, Guid id = default)
        {
            if (string.IsNullOrEmpty(description)) return Result.Fail<PayStubItem>(DescriptionIsRequired);
            if (quantity <= 0) return Result.Fail<PayStubItem>($"{description} {QuantityMustBeGraterThanZero}");
            if (unitPrice <= 0) return Result.Fail<PayStubItem>($"{description} {UnitPriceMustBeGreaterThanZero}");
            return Result.Ok(new PayStubItem(description, quantity, unitPrice, type, id));
        }

        public static Result<PayStubItem> CreateRegular(double quantity, decimal unitPrice) =>
            CreateItem(RegularHoursLabel, quantity, unitPrice, PayStubItemType.Regular);

        public static Result<PayStubItem> CreateOtherRegular(double quantity, decimal unitPrice) =>
            CreateItem(OtherRegularHoursLabel, quantity, unitPrice, PayStubItemType.OtherRegular);

        public static Result<PayStubItem> CreateOvertime(double quantity, decimal unitPrice) =>
            CreateItem(OvertimeHoursLabel, quantity, unitPrice, PayStubItemType.Overtime);

        public static Result<PayStubItem> CreateHolidayPremiumPay(double quantity, decimal unitPrice) =>
            CreateItem(HolidayPremiumPayHoursLabel, quantity, unitPrice, PayStubItemType.HolidayPremiumPay);

        public static Result<PayStubItem> CreateNightShift(double quantity, decimal unitPrice) =>
            CreateItem(NightShiftHoursLabel, quantity, unitPrice, PayStubItemType.NightShift);

        public static Result<PayStubItem> CreateMissing(double quantity, decimal unitPrice) =>
            CreateItem(MissingHoursLabel, quantity, unitPrice, PayStubItemType.Missing);

        public static Result<PayStubItem> CreateMissingOvertime(double quantity, decimal unitPrice) =>
            CreateItem(MissingOvertimeHoursLabel, quantity, unitPrice, PayStubItemType.MissingOvertime);

        public static Result<PayStubItem> CreateBonusOthersItem(double quantity, decimal unitPrice, string description) =>
            CreateItem(description ?? BonusOthersLabel, quantity, unitPrice, PayStubItemType.Other);

        public static Result<PayStubItem> CreateBonusOthersItem(decimal total, string description) =>
            total <= 0 
            ? Result.Fail<PayStubItem>("Bonus/Others Total must be greater than zero") 
            : Result.Ok(new PayStubItem(string.IsNullOrEmpty(description) ? BonusOthersLabel : description, 1, total, PayStubItemType.Other));

        public static Result<PayStubItem> CreateReimbursements(decimal total, string description) =>
            total <= 0 
            ? Result.Fail<PayStubItem>("Reimbursement must be greater than zero") 
            : Result.Ok(new PayStubItem(string.IsNullOrEmpty(description) ? ReimbursementLabel : description, 1, total, PayStubItemType.Reimbursement));

        public void AssignTo(PayStub payStub)
        {
            PayStub = payStub ?? throw new ArgumentNullException(nameof(payStub));
            PayStubId = payStub.Id;
        }
    }
}