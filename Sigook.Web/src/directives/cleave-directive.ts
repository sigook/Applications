import Cleave from 'cleave.js';

export default {
  name: 'cleave',
  bind(el: HTMLElement, binding: any) {
    let input: any;
    if (el.tagName.toLowerCase() === 'input') {
      input = el;
    } else {
      input = el.querySelector('input');
    }
    input._vCleave = new Cleave(input, binding.value);
  },
  unbind(el: HTMLElement) {
    let input: any;
    if (el.tagName.toLowerCase() === 'input') {
      input = el;
    } else {
      input = el.querySelector('input');
    }
    input._vCleave.destroy();
  }
}
