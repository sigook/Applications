import dayjs from 'dayjs';
export default function(date){
    return date ? dayjs(date).format('DD-MMM-YYYY') : date;
}