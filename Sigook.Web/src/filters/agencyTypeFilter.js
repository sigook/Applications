import Vue from 'vue';

export default function(value) {
  if (!value) return '';
  const type = Vue.prototype.$agencyTypes.find(t => t.value === value);
  return type ? type.label : '';
}
