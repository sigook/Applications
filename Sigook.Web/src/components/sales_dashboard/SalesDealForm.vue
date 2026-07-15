<template>
  <form class="sd-form" @submit.prevent>
    <b-field label="Deal name">
      <b-input v-model="name" placeholder="e.g. Warehouse staffing – 40 FTE"></b-input>
    </b-field>

    <b-field label="Client">
      <b-select v-model="clientId" expanded placeholder="Select client…">
        <option v-for="client in clients" :key="client.id" :value="client.id">
          {{ client.name }}
        </option>
      </b-select>
    </b-field>

    <div class="sd-form__row">
      <b-field label="Value" class="sd-form__col">
        <b-input v-model="value" type="number" placeholder="$"></b-input>
      </b-field>

      <b-field label="Stage" class="sd-form__col">
        <b-select v-model="stage" expanded placeholder="Select stage…">
          <option v-for="option in SALES_STAGE_ORDER" :key="option" :value="option">
            {{ option }}
          </option>
        </b-select>
      </b-field>
    </div>

    <b-field label="Expected close">
      <b-datepicker v-model="expectedClose" placeholder="Pick a date"></b-datepicker>
    </b-field>

    <b-field label="Notes">
      <b-input v-model="notes" type="textarea" placeholder="Context, next steps…"></b-input>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { SalesClient, SalesDealStage } from '@/types/salesDashboard';
import { SALES_STAGE_ORDER } from '@/types/salesDashboard';

defineProps<{ clients: readonly SalesClient[] }>();

const name = ref('');
const clientId = ref<string | null>(null);
const value = ref<number | null>(null);
const stage = ref<SalesDealStage | null>(null);
const expectedClose = ref<Date | null>(null);
const notes = ref('');
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

  :deep(.textarea) {
    min-height: 5.5rem;
  }
}

.sd-form__row {
  display: flex;
  gap: 0.7rem;
}

.sd-form__col {
  flex: 1;
  min-width: 0;
}
</style>
