export default function(name: string | null | undefined): string {
    if(name == null) return "";
    return name.split("@")[0];
}