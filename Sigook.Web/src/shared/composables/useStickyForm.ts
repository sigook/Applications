import { reactive, computed, watch, nextTick, Ref, MaybeRef } from 'vue';
import { useForm, useField, GenericObject, FormOptions } from 'vee-validate';
import type { AnyObjectSchema } from 'yup';

type FormValues<T extends GenericObject> = NonNullable<FormOptions<T>['initialValues']>;
type FormSchema<T extends GenericObject> = FormOptions<T>['validationSchema'];

export interface UseStickyFormOptions<T extends GenericObject> {
  schema: MaybeRef<AnyObjectSchema>;
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
    validationSchema: options.schema as unknown as FormSchema<T>,
    initialValues: options.initialValues as FormValues<T>,
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
    setValues({ ...options.initialValues, ...newValues } as FormValues<T>);
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
