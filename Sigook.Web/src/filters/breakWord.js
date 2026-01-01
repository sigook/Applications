export default function(value){
    return value ? value.split('|').join(" | ") : value;
}