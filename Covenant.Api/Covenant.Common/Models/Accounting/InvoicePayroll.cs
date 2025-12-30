using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting
{
    public class InvoicePayroll
    {
        public string Position { get; set; }
        public string Email { get; set; }
        public EmailSettingName EmailSettingName { get; set; }

        public static InvoicePayroll Covenant => new InvoicePayroll
        {
            Position = "Payroll Covenant",
            Email = "payroll@covenantgroupl.com",
            EmailSettingName = EmailSettingName.PayrollCovenant
        };

        public static InvoicePayroll Sigook => new InvoicePayroll
        {
            Position = "Payroll Sigook",
            Email = "payroll@sigook.com",
            EmailSettingName = EmailSettingName.PayrollSigook
        };
    }
}
