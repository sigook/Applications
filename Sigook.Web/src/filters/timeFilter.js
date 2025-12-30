import dayjs from 'dayjs';
export default function(date){
    if(!date) return "";
    return dayjs(date).format('HH:mm');
}