<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center fz1 mb-4">Assign Recruiters</h2>
    <p class="text-center mb-4">
      {{ requestCount }} order(s) selected
    </p>
    <b-message type="is-warning" size="is-small" :closable="false">
      This replaces every recruiter on the selected orders, including their Weekly Board day assignments and dispatch history.
      Leave the field empty and click "Unassign all" to remove them without assigning anyone.
    </b-message>
    <b-field label="Recruiters">
      <b-taginput
        v-model="selected"
        :data="filtered"
        autocomplete
        open-on-focus
        field="name"
        icon="account"
        placeholder="Type to search recruiters"
        @typing="onTyping"
        append-to-body>
      </b-taginput>
    </b-field>
    <div class="text-right">
      <b-button class="mr-2" @click="onCancel">Cancel</b-button>
      <b-button v-if="selected.length === 0" type="is-danger" @click="onSubmit">Unassign all</b-button>
      <b-button v-else type="is-primary" @click="onSubmit">Assign</b-button>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/utils/toast';
import { getAgencyPersonnel } from '@/api/agencyApi';
import type { AgencyPersonnelListItem } from '@/types/agency';

defineProps<{ requestCount: number }>();
const emit = defineEmits<{
  (e: 'submit', recruiterIds: string[]): void;
  (e: 'cancel'): void;
}>();

const isLoading = ref(false);
const personnel = ref<AgencyPersonnelListItem[]>([]);
const filtered = ref<AgencyPersonnelListItem[]>([]);
const selected = ref<AgencyPersonnelListItem[]>([]);

function loadPersonnel() {
  isLoading.value = true;
  getAgencyPersonnel()
    .then((response) => {
      personnel.value = response;
      filtered.value = response;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onTyping(text: string) {
  const term = (text || '').toLowerCase();
  const selectedIds = new Set(selected.value.map((s) => s.id));
  filtered.value = personnel.value.filter((p) =>
    !selectedIds.has(p.id) && p.name.toLowerCase().includes(term)
  );
}

function onSubmit() {
  emit('submit', selected.value.map((s) => s.id));
}

function onCancel() {
  emit('cancel');
}

loadPersonnel();
</script>
