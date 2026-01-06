import Cleave from 'cleave.js';

export default {
  name: 'cleave',
  bind(el, binding) {
    let input;
    if (el.tagName.toLowerCase() === 'input') {
      input = el;
    } else {
      input = el.querySelector('input');
    }
    input._vCleave = new Cleave(input, binding.value);
  },
  unbind(el) {
    let input;
    if (el.tagName.toLowerCase() === 'input') {
      input = el;
    } else {
      input = el.querySelector('input');
    }
    input._vCleave.destroy();
  }
}