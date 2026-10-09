export default function(name: string | null): string {
    if(name == null) return "";
    return name.split("@")[0];
}