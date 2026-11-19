import { afterEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import SearchSelect from '@/shared/ui/SearchSelect.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

interface Option {
  value: number;
  label: string;
}

const options: Option[] = [
  { value: 1, label: 'Toronto' },
  { value: 2, label: 'Ottawa' },
  { value: 3, label: 'Hamilton' },
];

interface Props {
  modelValue: number | null;
  options: Option[];
  remote?: boolean;
  minSearchLength?: number;
  placeholder?: string;
}

function setup(props: Partial<Props> = {}) {
  return mountWithPlugins(SearchSelect, { props: { modelValue: null, options, ...props } });
}

function autocomplete(wrapper: ReturnType<typeof setup>) {
  return wrapper.findComponent({ name: 'BAutocomplete' });
}

function text(wrapper: ReturnType<typeof setup>) {
  return autocomplete(wrapper).props('modelValue') as string;
}

function labels(wrapper: ReturnType<typeof setup>) {
  return (autocomplete(wrapper).props('data') as Option[]).map((o) => o.label);
}

async function emit(wrapper: ReturnType<typeof setup>, event: string, ...args: unknown[]) {
  autocomplete(wrapper).vm.$emit(event, ...args);
  await flushPromises();
}

afterEach(() => {
  vi.useRealTimers();
});

describe('SearchSelect', () => {
  it('shows the label of the selected value and the default placeholder', () => {
    const wrapper = setup({ modelValue: 2 });

    expect(text(wrapper)).toBe('Ottawa');
    expect(wrapper.find('input').attributes('placeholder')).toBe('Search…');
  });

  it('filters the options locally by the typed term', async () => {
    const wrapper = setup();
    await emit(wrapper, 'update:modelValue', ' ON ');

    expect(labels(wrapper)).toEqual(['Toronto', 'Hamilton']);
  });

  it('emits the value of the selected option', async () => {
    const wrapper = setup();
    await emit(wrapper, 'select', options[2]);
    await emit(wrapper, 'select', null);

    expect(wrapper.emitted('update:modelValue')).toEqual([[3]]);
  });

  it('clears the selection when the text is wiped', async () => {
    const wrapper = setup({ modelValue: 1 });
    await emit(wrapper, 'typing', 'Tor');
    expect(wrapper.emitted('update:modelValue')).toBeUndefined();

    await emit(wrapper, 'typing', '');
    expect(wrapper.emitted('update:modelValue')).toEqual([[null]]);
  });

  it('clears the text on focus and restores the label on blur', async () => {
    const wrapper = setup({ modelValue: 1 });
    await emit(wrapper, 'focus');
    expect(text(wrapper)).toBe('');
    expect(labels(wrapper)).toHaveLength(3);

    await emit(wrapper, 'blur');
    expect(text(wrapper)).toBe('Toronto');
    expect(wrapper.emitted('search')).toBeUndefined();
  });

  it('follows the model value', async () => {
    const wrapper = setup({ modelValue: 1 });
    await wrapper.setProps({ modelValue: 3 });
    expect(text(wrapper)).toBe('Hamilton');

    await wrapper.setProps({ modelValue: null });
    expect(text(wrapper)).toBe('');
  });

  it('picks up the label when the options arrive after the value', async () => {
    const wrapper = setup({ modelValue: 3, options: [] });
    expect(text(wrapper)).toBe('');

    await wrapper.setProps({ options });
    expect(text(wrapper)).toBe('Hamilton');
  });

  it('keeps the typed text when options change while focused', async () => {
    const wrapper = setup({ modelValue: 3 });
    await emit(wrapper, 'focus');
    await emit(wrapper, 'typing', 'Ham');
    await wrapper.setProps({ options: [...options] });

    expect(text(wrapper)).toBe('Ham');
  });

  describe('remote', () => {
    it('asks the parent for the full list on focus', async () => {
      const wrapper = setup({ remote: true });
      await emit(wrapper, 'focus');
      expect(wrapper.emitted('search')).toEqual([['']]);
    });

    it('debounces the search term', () => {
      vi.useFakeTimers();
      const wrapper = setup({ remote: true });
      autocomplete(wrapper).vm.$emit('typing', 'T');
      vi.advanceTimersByTime(100);
      autocomplete(wrapper).vm.$emit('typing', 'Tor');
      vi.advanceTimersByTime(299);
      expect(wrapper.emitted('search')).toBeUndefined();

      vi.advanceTimersByTime(1);
      expect(wrapper.emitted('search')).toEqual([['Tor']]);
    });

    it('cancels a pending search on blur', () => {
      vi.useFakeTimers();
      const wrapper = setup({ remote: true });
      autocomplete(wrapper).vm.$emit('typing', 'Tor');
      autocomplete(wrapper).vm.$emit('blur');
      vi.advanceTimersByTime(1000);

      expect(wrapper.emitted('search')).toBeUndefined();
    });

    it('shows the server results unfiltered', async () => {
      const wrapper = setup({ remote: true });
      await emit(wrapper, 'update:modelValue', 'zzz');
      expect(labels(wrapper)).toEqual(['Toronto', 'Ottawa', 'Hamilton']);
    });

    it('shows nothing for a partial term below the minimum length', async () => {
      const wrapper = setup({ remote: true, minSearchLength: 3 });
      await emit(wrapper, 'update:modelValue', 'To');
      expect(labels(wrapper)).toEqual([]);

      await emit(wrapper, 'update:modelValue', '');
      expect(labels(wrapper)).toHaveLength(3);

      await emit(wrapper, 'update:modelValue', 'Tor');
      expect(labels(wrapper)).toHaveLength(3);
    });
  });
});
