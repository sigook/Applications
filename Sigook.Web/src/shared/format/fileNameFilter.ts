export default function fileName(name: string): string;
export default function fileName(name: string | null | undefined): string | null;
export default function fileName(name: string | null | undefined): string | null {
    if (!name) return null;
    const fileName = name.split(/[ ._]+/);
    const joinName = fileName[0] + "." + fileName[fileName.length-1]

    return joinName.toUpperCase();
}