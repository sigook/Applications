import type { Directive, DirectiveBinding } from 'vue';

function addStatusClass(element: HTMLElement, binding: DirectiveBinding<{ status: string }>): void {
  const statusClass = 'status-' + binding.value.status.toLowerCase();
  element.classList.add(statusClass);
}

const statusDirective: Directive = {
  mounted(element, binding) {
    addStatusClass(element as HTMLElement, binding);
  },
  updated(element, binding) {
    addStatusClass(element as HTMLElement, binding);
  },
};

export default statusDirective;
