import Vue from 'vue';

export default function(value: number | null): string {
  if (!value) return '';
  const type = Vue.prototype.$agencyTypes.find(t => t.value === value);
  return type ? type.label : '';
}
