using System;

namespace Covenant.Deductions.Entities
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