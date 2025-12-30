export default function(value){
    if(!value) return "S";
    return value.substring(0,2).toUpperCase();
}