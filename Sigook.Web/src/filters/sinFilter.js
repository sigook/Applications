export default function(string){
    if (string){
        let number = string.substr(string.length - 4);
        return "******" + number
    } else {
        return ""
    }
}