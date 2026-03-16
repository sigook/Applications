export default function(value: string | null): string {
    if (value){
        let number = value.substr(value.length - 4);
        return "******" + number
    } else {
        return ""
    }
}