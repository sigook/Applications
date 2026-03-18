import { DirectiveOptions } from 'vue';

function addStatusClass(element: HTMLElement, binding: any): void {
    const statusClass = 'status-' + binding.value.status.toLowerCase();
    element.classList.add(statusClass);
}

export default {
    bind: (element, binding) => {
        addStatusClass(element as HTMLElement, binding);
    },
    update: (element, binding) => {
        addStatusClass(element as HTMLElement, binding);
    }
} as DirectiveOptions;
