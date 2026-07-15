<template>
  <form class="sd-form" @submit.prevent>
    <b-field label="Type">
      <div class="sd-choices">
        <button
          v-for="option in TYPE_OPTIONS"
          :key="option"
          type="button"
          class="sd-choice"
          :class="{ 'is-active': type === option }"
          @click="type = option"
        >
          {{ option }}
        </button>
      </div>
    </b-field>

    <b-field label="Client">
      <b-select v-model="clientId" expanded placeholder="Select client…">
        <option v-for="client in clients" :key="client.id" :value="client.id">
          {{ client.name }}
        </option>
      </b-select>
    </b-field>

    <b-field label="Subject">
      <b-input v-model="subject" placeholder="Short summary"></b-input>
    </b-field>

    <b-field label="Date & time">
      <b-datetimepicker v-model="occurredAt" placeholder="Pick a date and time"></b-datetimepicker>
    </b-field>

    <b-field label="Notes">
      <b-input v-model="notes" type="textarea" placeholder="What was discussed…"></b-input>
    </b-field>

    <b-field label="Follow-up date">
      <b-datepicker v-model="followUpAt" placeholder="Pick a date"></b-datepicker>
    </b-field>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { SalesClient, SalesInteractionType } from '@/types/salesDashboard';

defineProps<{ clients: readonly SalesClient[] }>();

const TYPE_OPTIONS: readonly SalesInteractionType[] = ['Call', 'Email', 'Meeting'];

const type = ref<SalesInteractionType>('Call');
const clientId = ref<string | null>(null);
const subject = ref('');
const occurredAt = ref<Date | null>(null);
const notes = ref('');
const followUpAt = ref<Date | null>(null);
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

.sd-choices {
  display: flex;
  gap: 0.4rem;
}

.sd-choice {
  border: 0;
  border-radius: 7px;
  padding: 0.4rem 0.85rem;
  font-size: 0.78rem;
  font-weight: 600;
  background: #eef0f3;
  color: #666;
  cursor: pointer;
  transition: background-color 0.15s ease, color 0.15s ease;

  &.is-active {
    background: $primary;
    color: $white;
  }

  &:focus-visible {
    outline: 2px solid rgba(33, 183, 255, 0.5);
    outline-offset: 1px;
  }
}
</style>
