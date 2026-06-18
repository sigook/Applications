<template>
  <div class="modal-card add-workers-modal">
    <header class="modal-card-head is-flex-direction-column is-align-items-start">
      <p class="modal-card-title">Add workers to order</p>
      <p class="has-text-grey is-size-7">
        Order #{{ assignment.numberId }} · {{ assignment.companyName }} — {{ workDateLabel }}
      </p>
    </header>
    <section class="modal-card-body">
      <div class="container-flex">
        <div class="col-12 col-padding">
          <div class="sent-so-far">
            <span>Sent so far</span>
            <strong>{{ assignment.dispatches.length }}</strong>
          </div>
        </div>

        <div class="col-12 col-padding">
          <b-field label="Search candidates">
            <b-input v-model="searchText" placeholder="Search by name or email" icon="magnify" :loading="isLoading"
              @update:model-value="onTyping"></b-input>
          </b-field>
          <p v-if="searchText.length > 0 && searchText.length < 3" class="hint">
            Type at least 3 characters to search.
          </p>
        </div>

        <div class="col-12 col-padding">
          <div v-if="candidates.length > 0" class="candidate-list">
            <label v-for="worker in candidates" :key="worker.workerProfileId" class="candidate-row">
              <div class="candidate-info">
                <p class="candidate-name">{{ worker.fullName }}</p>
                <p class="candidate-email">{{ worker.email }}</p>
              </div>
              <b-checkbox :model-value="isSelected(worker)" @update:model-value="toggle(worker)"></b-checkbox>
            </label>
          </div>
          <p v-else-if="searchText.length >= 3 && !isLoading" class="hint">No candidates found.</p>

          <div v-if="selected.length > 0" class="selected-count">
            {{ selected.length }} selected
          </div>
        </div>
      </div>
    </section>
    <footer class="modal-card-foot is-justify-content-flex-end">
      <b-button @click="emit('close')">Cancel</b-button>
      <b-button type="is-primary" icon-left="account-plus" :disabled="selected.length === 0" :loading="saving"
        @click="submit">
        Send workers
      </b-button>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import dayjs from 'dayjs';
import { showAlertError } from '@/utils/toast';
import { getAgencyWorkersDropdown } from '@/api/agencyWorkerApi';
import type { AgencyWorkerDropdownItem } from '@/types/agency';
import type { WeeklyBoardAssignment } from '@/types/weeklyBoard';

const props = defineProps<{
  assignment: WeeklyBoardAssignment;
  saving: boolean;
}>();

const emit = defineEmits<{
  (e: 'send', workerProfileIds: string[]): void;
  (e: 'close'): void;
}>();

const searchText = ref('');
const isLoading = ref(false);
const results = ref<AgencyWorkerDropdownItem[]>([]);
const selected = ref<AgencyWorkerDropdownItem[]>([]);

let searchTimer: ReturnType<typeof setTimeout> | null = null;

const workDateLabel = computed(() => dayjs(props.assignment.workDate).format('ddd MMM D'));

const alreadySent = computed(() => new Set(props.assignment.dispatches.map(d => d.workerProfileId)));

const candidates = computed(() =>
  results.value.filter(w => !alreadySent.value.has(w.workerProfileId))
);

function onTyping(value: string): void {
  if (searchTimer) clearTimeout(searchTimer);
  if (value.trim().length < 3) {
    results.value = [];
    return;
  }
  searchTimer = setTimeout(() => search(value.trim()), 300);
}

function search(term: string): void {
  isLoading.value = true;
  getAgencyWorkersDropdown({ searchTerm: term })
    .then(response => {
      results.value = response;
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isLoading.value = false;
    });
}

function isSelected(worker: AgencyWorkerDropdownItem): boolean {
  return selected.value.some(s => s.workerProfileId === worker.workerProfileId);
}

function toggle(worker: AgencyWorkerDropdownItem): void {
  const index = selected.value.findIndex(s => s.workerProfileId === worker.workerProfileId);
  if (index >= 0) selected.value.splice(index, 1);
  else selected.value.push(worker);
}

function submit(): void {
  if (selected.value.length === 0) return;
  emit('send', selected.value.map(s => s.workerProfileId));
}
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.add-workers-modal {
  width: 100%;

  .sent-so-far {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.6rem 0.85rem;
    margin-bottom: 1rem;
    background: $gray-bg;
    border-radius: 8px;
    font-size: 0.85rem;
    color: $grey-dark;

    strong {
      font-size: 1.1rem;
      color: $grey-font;
    }
  }

  .hint {
    font-size: 0.8rem;
    color: $grey-light;
    margin: 0.25rem 0 0.5rem;
  }

  .candidate-list {
    max-height: 280px;
    overflow-y: auto;
    border: 1px solid $gray-border;
    border-radius: 8px;
  }

  .candidate-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.55rem 0.75rem;
    border-bottom: 1px solid $gray-border;
    cursor: pointer;

    &:last-child {
      border-bottom: none;
    }

    &:hover {
      background: $gray-bg;
    }

    .candidate-name {
      font-weight: 600;
      font-size: 0.88rem;
    }

    .candidate-email {
      font-size: 0.78rem;
      color: $grey-light;
    }
  }

  .selected-count {
    margin-top: 0.75rem;
    font-size: 0.82rem;
    color: $primary;
    font-weight: 600;
  }
}
</style>
