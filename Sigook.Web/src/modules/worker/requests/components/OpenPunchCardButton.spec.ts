import { describe, expect, it } from 'vitest';
import OpenPunchCardButton from '@/modules/worker/requests/components/OpenPunchCardButton.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('OpenPunchCardButton', () => {
  it('links to the punch card tab of the request', () => {
    const wrapper = mountWithPlugins(OpenPunchCardButton, { props: { requestId: 'r1', expanded: true, size: 'is-small' } });
    const link = wrapper.find('a');
    expect(link.attributes('href')).toBe('/worker-requests/applied/r1?tab=punch');
    expect(link.text()).toBe('OPEN PUNCH CARD');
    expect(link.classes()).toEqual(expect.arrayContaining(['is-fullwidth', 'is-small', 'is-danger']));
  });

  it('does not let the click reach the parent', async () => {
    let parentClicks = 0;
    const host = document.createElement('div');
    host.addEventListener('click', () => { parentClicks++; });
    document.body.appendChild(host);
    const wrapper = mountWithPlugins(OpenPunchCardButton, { props: { requestId: 'r1' }, attachTo: host });
    await wrapper.find('a').trigger('click');
    expect(parentClicks).toBe(0);
  });
});
