import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';

dayjs.extend(relativeTime);

dayjs.locale('en');

export default function (date) {
    if (!date) return "";

    return dayjs(date).fromNow();
}