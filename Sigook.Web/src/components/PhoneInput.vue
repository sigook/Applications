<template>
  <b-field :type="errorMessage ? 'is-danger' : ''" :message="errorMessage">
    <template #label>
      {{ model }} <span v-if="required" class="has-text-danger">*</span>
    </template>
    <b-input type="tel" title="phone" :name="model" :disabled="disabled" :placeholder="placeholder"
      v-model="value" v-cleave="mask" />
  </b-field>
</template>

<script lang="ts">
import { watch } from 'vue';
import { useField } from 'vee-validate';
import { phoneMask as mask } from '@/constants/phoneMask';

export default {
  props: {
    model: { type: String, required: true },
    defaultValue: { type: String, default: '' },
    disabled: Boolean,
    required: Boolean,
    placeholder: String,
  },
  emits: ['formattedPhone'],
  setup(props, { emit }) {
    const { value, errorMessage, validate } = useField<string>(
      () => props.model,
      undefined,
      { initialValue: props.defaultValue || '' }
    );

    watch(value, (val) => {
      emit('formattedPhone', val || null);
    });

    return { mask, value, errorMessage, validate };
  },
  methods: {
    async validatePhone(): Promise<boolean> {
      const result = await (this as any).validate();
      return result.valid;
    },
  },
};
</script>
