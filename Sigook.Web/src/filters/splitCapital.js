export default function(value){
    return value ? value.split(/(?=[A-Z])/).join(" ") : value;
}