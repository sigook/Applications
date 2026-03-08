using System;

namespace Covenant.Common.Entities.Deductions
{
    public interface ICpp
	{
		Guid Id { get; }
		decimal From { get; }
		decimal To { get; }
		decimal Cpp { get; }
		int Year { get; }
	}
}