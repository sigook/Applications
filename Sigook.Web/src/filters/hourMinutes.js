export default function(date){
    let hour = date.split(":");
    return hour[0] + ':' + hour[1]
}