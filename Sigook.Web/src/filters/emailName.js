export default function(name){
    if(name == null) return "";
    return name.split("@")[0];
}