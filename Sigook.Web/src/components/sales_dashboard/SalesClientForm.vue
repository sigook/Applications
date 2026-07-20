<template>
  <form class="sd-form" @submit.prevent>
    <b-field label="Business name">
      <b-input v-model="name" placeholder="e.g. Acme Logistics"></b-input>
    </b-field>

    <b-field label="Industry">
      <search-select v-model="industry" :options="industryOptions" clearable placeholder="Search industry…" />
    </b-field>

    <b-field label="Primary contact">
      <b-input v-model="contact" placeholder="Full name"></b-input>
    </b-field>

    <b-field label="Email">
      <b-input v-model="email" type="email" placeholder="name@company.com"></b-input>
    </b-field>

    <b-field label="Phone">
      <b-input v-model="phone" placeholder="(555) 000-0000"></b-input>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import SearchSelect from './SearchSelect.vue';

const INDUSTRY_OPTIONS: readonly string[] = [
  'Logistics',
  'Manufacturing',
  'Food Service',
  'Retail',
  'Construction',
  'Other',
];

const industryOptions = INDUSTRY_OPTIONS.map((o) => ({ value: o, label: o }));

const name = ref('');
const industry = ref<string | null>(null);
const contact = ref('');
const email = ref('');
const phone = ref('');
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-form {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;

  :deep(.label) {
    font-size: 0.75rem;
    font-weight: 600;
    color: #777;
    margin-bottom: 0.35rem;
  }

  :deep(.input),
  :deep(.textarea),
  :deep(.select select) {
    font-size: 0.82rem;
    border-color: $gray-border;
    box-shadow: none;
    color: #333;

    &:focus,
    &:active {
      border-color: $primary;
      box-shadow: 0 0 0 2px rgba(33, 183, 255, 0.15);
    }
  }
}
</style>
