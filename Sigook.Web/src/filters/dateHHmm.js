import dayjs from 'dayjs';
export default function(date){
    return date ? dayjs(date).format('HH:mm') : date;
}