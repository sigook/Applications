using Covenant.Deductions.Entities;
using System;

namespace Covenant.Deductions.Models
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
