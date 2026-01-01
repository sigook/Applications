import dayjs from "dayjs";
import duration from "dayjs/plugin/duration";

dayjs.extend(duration);

export default function (hours) {
  if (!hours || hours === 0 || hours === "0") {
    return 0;
  }

  if (typeof hours === "string" && hours.includes(":")) {
    const timeParts = hours.split(":");
    const hoursNum = parseInt(timeParts[0]) || 0;
    const minutesNum = parseInt(timeParts[1]) || 0;
    const secondsNum = parseInt(timeParts[2]) || 0;

    const totalHours = hoursNum + minutesNum / 60 + secondsNum / 3600;

    return Math.round(totalHours) === totalHours
      ? totalHours
      : totalHours.toFixed(2);
  }

  const numericHours = parseFloat(hours);
  if (!isNaN(numericHours)) {
    return Math.round(numericHours) === numericHours
      ? numericHours
      : numericHours.toFixed(2);
  }

  return hours;
}
