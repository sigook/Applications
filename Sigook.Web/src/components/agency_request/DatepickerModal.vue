<template>
  <div class="p-3">
    <div class="columns is-multiline">
      <div class="column is-12">
        <b-field label="Start Working">
          <b-datepicker v-model="localStartWorking" name="start" inline :focused-date="today" :min-date="minDate">
          </b-datepicker>
        </b-field>
      </div>
      <div class="column is-12">
        <b-button type="is-primary" @click="bookWorker">Save</b-button>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import dayjs from "dayjs";

const props = defineProps<{ startWorking?: any }>();
const emit = defineEmits<{
  (e: 'update:startWorking', value: Date): void;
  (e: 'onSelectCalendar', value: Date): void;
}>();

const today = new Date();
const localStartWorking = ref<Date>(props.startWorking ? new Date(props.startWorking) : dayjs().toDate());

watch(() => props.startWorking, (newVal) => {
  localStartWorking.value = newVal ? new Date(newVal) : dayjs().toDate();
});

const minDate = computed(() => dayjs().subtract(1, 'month').toDate());

function bookWorker() {
  emit('update:startWorking', localStartWorking.value);
  emit('onSelectCalendar', localStartWorking.value);
}

if (!props.startWorking) {
  localStartWorking.value = dayjs().toDate();
}
</script>
