function addStatusClass(element, binding){
    let statusClass = 'status-' + binding.value.status.toLowerCase();
    element.classList.add(statusClass);
}
export default {
    bind: (element, binding) => {
        addStatusClass(element, binding);
    },

    update: (element, binding) => {
        addStatusClass(element, binding);
    }

}