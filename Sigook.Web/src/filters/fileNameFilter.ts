export default function(name: string | null): string | null {
    if (!name) return name;
    const fileName = name.split(/[ ._]+/);
    const joinName = fileName[0] + "." + fileName[fileName.length-1]

    return joinName.toUpperCase();
}