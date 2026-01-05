using System;

namespace Covenant.Deductions.Entities
{
	public interface ITax
	{
		Guid Id { get; }
		decimal From { get; }
		decimal To { get; }
		decimal? Cc0 { get; }
		decimal? Cc1 { get; }
		decimal? Cc2 { get; }
		decimal? Cc3 { get; }
		decimal? Cc4 { get; }
		decimal? Cc5 { get; }
		decimal? Cc6 { get; }
		decimal? Cc7 { get; }
		decimal? Cc8 { get; }
		decimal? Cc9 { get; }
		decimal? Cc10 { get; }
		int Year { get; }
	}
}