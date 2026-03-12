using Covenant.Common.Entities.Deductions;
using System;

namespace Covenant.Common.Models.Deductions
{
    public class CppModel : ICpp
    {
        public Guid Id { get; set; }

        public decimal From { get; set; }

        public decimal To { get; set; }

        public decimal Cpp { get; set; }

        public int Year { get; set; }
    }
}
