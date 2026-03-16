export default function(name: string | null): string | null {
    if (!name) return name;
    var fileName = name.split(/[ ._]+/);
    var joinName = fileName[0] + "." + fileName[fileName.length-1]

    return joinName.toUpperCase();
}