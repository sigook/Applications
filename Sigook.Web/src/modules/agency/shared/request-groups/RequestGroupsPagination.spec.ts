import { describe, expect, it } from 'vitest';
import RequestGroupsPagination from '@/modules/agency/shared/request-groups/RequestGroupsPagination.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(total: number) {
  return mountWithPlugins(RequestGroupsPagination, { props: { page: 1, total, perPage: 10 } });
}

describe('RequestGroupsPagination', () => {
  it('is hidden when everything fits in one page', () => {
    expect(setup(10).find('.pagination').exists()).toBe(false);
  });

  it('emits the chosen page', () => {
    const wrapper = setup(25);
    expect(wrapper.find('.pagination').exists()).toBe(true);
    wrapper.findComponent({ name: 'BPagination' }).vm.$emit('update:modelValue', 2);
    expect(wrapper.emitted('change')).toEqual([[2]]);
  });
});
