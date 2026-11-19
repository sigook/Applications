import { afterEach, describe, expect, it, vi } from 'vitest';

const { configure } = vi.hoisted(() => ({ configure: vi.fn() }));

vi.mock('vee-validate', () => ({ configure }));

afterEach(() => {
  configure.mockReset();
});

describe('configureValidation', () => {
  it('validates fields while the user types', async () => {
    const { configureValidation } = await import('@/lang/validator');
    configureValidation();

    expect(configure).toHaveBeenCalledExactlyOnceWith({ validateOnInput: true });
  });
});
