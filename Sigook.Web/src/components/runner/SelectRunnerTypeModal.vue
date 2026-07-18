<template>
  <div class="modal-card" style="width: auto">
    <header class="modal-card-head">
      <p class="modal-card-title">Add Runner</p>
    </header>
    <form @submit.prevent="submit">
      <section class="modal-card-body" style="min-width: 380px">
        <p v-if="name" class="mb-4">{{ name }}</p>
        <b-field label="Type">
          <b-radio-button v-model="type" :native-value="RunnerType.Active" type="is-primary">Active</b-radio-button>
          <b-radio-button v-model="type" :native-value="RunnerType.Passive" type="is-primary">Passive</b-radio-button>
        </b-field>
      </section>
      <footer class="modal-card-foot">
        <b-button @click="emit('close')">Cancel</b-button>
        <b-button type="is-primary" native-type="submit">Add Runner</b-button>
      </footer>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { RunnerType } from '@/types/runner';

defineProps<{ name?: string }>();
const emit = defineEmits<{ (e: 'select', type: RunnerType): void; (e: 'close'): void }>();

const type = ref<RunnerType>(RunnerType.Active);

function submit() {
  emit('select', type.value);
}
</script>
