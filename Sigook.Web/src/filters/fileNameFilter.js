export default function(name){
    if (!name) return name;
    var fileName = name.split(/[ ._]+/);
    var joinName = fileName[0] + "." + fileName[fileName.length-1]

    return joinName.toUpperCase();
}