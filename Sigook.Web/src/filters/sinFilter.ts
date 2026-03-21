export default function(value: string | null): string {
    if (value){
        const number = value.substr(value.length - 4);
        return "******" + number
    } else {
        return ""
    }
}