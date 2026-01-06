import dayjs from 'dayjs';
export default function(date){
    return date ? dayjs(date).format('ddd DD MMMM, YYYY, HH:mm') : date;
}