import { reactive, computed, watch, nextTick, Ref } from 'vue';
import { useForm, useField, GenericObject } from 'vee-validate';

export interface UseStickyFormOptions<T extends GenericObject> {
  schema: any;
  initialValues: T;
}

export function useStickyForm<T extends GenericObject>(options: UseStickyFormOptions<T>) {
  const {
    errors: rawErrors,
    handleSubmit,
    setValues,
    setFieldValue,
    setFieldError,
    resetForm,
    values,
    validate,
    validateField,
  } = useForm<T>({
    validationSchema: options.schema,
    initialValues: options.initialValues as any,
  });

  const fieldNames = Object.keys(options.initialValues) as Array<keyof T & string>;
  const fields = {} as { [K in keyof T]: Ref<T[K]> };
  for (const name of fieldNames) {
    const { value } = useField<T[typeof name]>(name as string);
    (fields as Record<string, unknown>)[name] = value;
  }

  const interacted = reactive<Record<string, boolean>>({});
  let suppressTracking = false;

  for (const name of fieldNames) {
    watch((fields as Record<string, Ref<unknown>>)[name], () => {
      if (!suppressTracking) interacted[name as string] = true;
    });
  }

  const errors = computed(() => {
    const out: Record<string, string> = {};
    for (const key of Object.keys(rawErrors.value)) {
      out[key] = interacted[key] ? ((rawErrors.value as Record<string, string>)[key] || '') : '';
    }
    return out;
  });

  function markInteracted(names?: string[]) {
    const list = names || (fieldNames as string[]);
    for (const f of list) interacted[f] = true;
  }

  function clearInteracted() {
    for (const k of Object.keys(interacted)) delete interacted[k];
  }

  function hydrate(newValues: Partial<T>) {
    suppressTracking = true;
    setValues({ ...options.initialValues, ...newValues } as any);
    clearInteracted();
    nextTick(() => { suppressTracking = false; });
  }

  function resetAll() {
    suppressTracking = true;
    resetForm();
    clearInteracted();
    nextTick(() => { suppressTracking = false; });
  }

  return {
    fields,
    errors,
    rawErrors,
    values,
    handleSubmit,
    setFieldValue,
    setFieldError,
    validate,
    validateField,
    markInteracted,
    clearInteracted,
    hydrate,
    resetAll,
  };
}
