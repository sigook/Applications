import Cleave from 'cleave.js';
import type { Directive } from 'vue';

const cleaveDirective: Directive = {
  mounted(el: HTMLElement, binding: any) {
    const input: any = el.tagName.toLowerCase() === 'input' ? el : el.querySelector('input');
    if (input) {
      input._vCleave = new Cleave(input, binding.value);
    }
  },
  unmounted(el: HTMLElement) {
    const input: any = el.tagName.toLowerCase() === 'input' ? el : el.querySelector('input');
    if (input && input._vCleave) {
      input._vCleave.destroy();
    }
  },
};

export default cleaveDirective;
