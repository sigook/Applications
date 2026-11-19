import { describe, expect, it } from 'vitest';
import Unauthorized from '@/app/pages/Unauthorized.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('Unauthorized', () => {
  it('explains the missing permissions', () => {
    expect(mountWithPlugins(Unauthorized).find('h1').text()).toBe('Sorry you don\'t have the permissions to access this page');
  });
});
