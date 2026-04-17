<template>
  <b-field :type="errors.phone ? 'is-danger' : ''" :message="errors.phone || ''">
    <template #label>
      {{ label }} <span v-if="required" class="has-text-danger">*</span>
    </template>
    <b-input type="tel" title="phone" :name="name" :disabled="disabled" :placeholder="placeholder"
      v-model="phone" maxlength="12" />
  </b-field>
</template>

<script lang="ts">
import { computed, ref, watch } from 'vue';
import { useForm } from 'vee-validate';
import { phoneSchema } from '@/utils/validation';

function formatPhone(raw: string): string {
  const digits = (raw || '').replace(/\D/g, '').slice(0, 10);
  const a = digits.slice(0, 3);
  const b = digits.slice(3, 6);
  const c = digits.slice(6, 10);
  if (digits.length <= 3) return a;
  if (digits.length <= 6) return `${a} ${b}`;
  return `${a} ${b}-${c}`;
}

export default {
  props: {
    name: { type: String, default: 'mobileNumber' },
    label: { type: String, default: 'Mobile Number' },
    model: { type: String, default: '' },
    defaultValue: { type: String, default: '' },
    disabled: Boolean,
    required: Boolean,
    placeholder: String,
  },
  emits: ['formattedPhone'],
  setup(props, { emit }) {
    const schema = computed(() => {
      return {
        phone: phoneSchema(props.required),
      };
    });

    const { errors: formErrors, defineField, setFieldValue, validate } = useForm({
      validationSchema: schema,
      initialValues: {
        phone: formatPhone(props.defaultValue || ''),
      },
    });

    const [phone] = defineField('phone');
    const interacted = ref(false);

    watch(phone, (val) => {
      interacted.value = true;
      const formatted = formatPhone(val || '');
      if (formatted !== val) {
        setFieldValue('phone', formatted);
        return;
      }
      emit('formattedPhone', formatted || null);
    });

    watch(() => props.defaultValue, (val) => {
      const formatted = formatPhone(val || '');
      if (formatted !== phone.value) {
        setFieldValue('phone', formatted);
      }
    });

    const errors = computed(() => ({
      phone: interacted.value ? (formErrors.value.phone || '') : '',
    }));

    function markInteracted() {
      interacted.value = true;
    }

    return { errors, phone, validate, markInteracted };
  },
  methods: {
    async validatePhone(): Promise<boolean> {
      (this as any).markInteracted();
      const result = await (this as any).validate();
      return result.valid;
    },
  },
};
</script>
