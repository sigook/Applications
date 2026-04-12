/**
 * Distributes a total of weekly hours across the given number of days,
 * respecting the maximum daily hours limit.
 * Returns an empty array if distribution is not possible (daily average exceeds max).
 */
export function distributeHours(
  totalDays: number,
  totalHoursWeek: number,
  maximumDailyHours: number
): number[] {
  const weekDays: number[] = [];
  weekDays.length = totalDays;

  const dailyHours = totalHoursWeek / totalDays;
  if (dailyHours > maximumDailyHours) return [];
  else if (totalHoursWeek < maximumDailyHours) weekDays[0] = totalHoursWeek;
  else {
    let day = 0;
    while (totalHoursWeek > 0) {
      let h: number;
      const pureHours = Math.trunc(dailyHours);
      if (pureHours === 0) h = dailyHours;
      else if (totalHoursWeek < dailyHours) h = totalHoursWeek;
      else if (pureHours === maximumDailyHours) h = pureHours;
      else if (pureHours < totalHoursWeek) h = pureHours + 1;
      else h = totalHoursWeek;
      totalHoursWeek -= h;
      weekDays[day] = h;
      day++;
    }
  }
  return weekDays;
}
