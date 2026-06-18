<template>
  <div class="modal-card assign-modal">
    <header class="modal-card-head is-flex-direction-column is-align-items-start">
      <p class="modal-card-title">Assign recruiter to an order</p>
      <p class="has-text-grey is-size-7">Pick the order, the work day, and who runs it.</p>
    </header>
    <section class="modal-card-body">
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-field label="Order">
            <b-autocomplete :data="orders" placeholder="RequestID, Position" :loading="isLoadingOrders"
              :custom-formatter="(option: AgencyRequestListItem) => `#${option.numberId} | ${option.jobTitle} | ${option.companyFullName}`"
              icon="magnify" append-to-body @typing="onOrderTyping" @select="onOrderSelected">
              <template v-slot="props">
                <small>#{{ props.option.numberId }} | {{ props.option.jobTitle }} | {{ props.option.companyFullName }}
                </small>
              </template>
            </b-autocomplete>
          </b-field>
        </div>

        <div class="col-12 col-padding">
          <b-field label="Work day">
            <div class="day-grid">
              <button v-for="day in days" :key="day.date" type="button" class="day-cell"
                :class="{ 'is-selected': selectedDays.includes(day.date) }" @click="toggleDay(day.date)">
                <span class="day-name">{{ day.weekday }}</span>
                <span class="day-num">{{ day.dayNum }}</span>
              </button>
            </div>
          </b-field>
        </div>

        <div class="col-12 col-padding">
          <b-field label="Recruiter(s)">
            <b-taginput v-model="selectedRecruiters" :data="filteredRecruiters" autocomplete open-on-focus field="name"
              icon="account" placeholder="Type to search recruiters" append-to-body @typing="onRecruiterTyping">
            </b-taginput>
          </b-field>
        </div>
      </div>
    </section>
    <footer class="modal-card-foot is-justify-content-flex-end">
      <b-button @click="emit('close')">Cancel</b-button>
      <b-button type="is-primary" icon-left="check" :disabled="!canAssign" :loading="saving" @click="submit">
        Assign
      </b-button>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { showAlertError } from '@/utils/toast';
import { getAllAgencyRequests } from '@/api/agencyRequestApi';
import { getAgencyPersonnel } from '@/api/agencyApi';
import { RequestStatus } from '@/constants/enums';
import type { AgencyRequestListItem, AgencyPersonnelListItem, AgencyRequestFilter } from '@/types/agency';
import type { AssignRecruitersPayload, WeekDay, AssignPreset } from '@/types/weeklyBoard';

const props = defineProps<{
  days: WeekDay[];
  preset?: AssignPreset;
  saving: boolean;
}>();

const emit = defineEmits<{
  (e: 'assign', payload: AssignRecruitersPayload): void;
  (e: 'close'): void;
}>();

const isLoadingOrders = ref(false);
const orders = ref<AgencyRequestListItem[]>([]);
const selectedOrder = ref<AgencyRequestListItem | null>(null);

const personnel = ref<AgencyPersonnelListItem[]>([]);
const filteredRecruiters = ref<AgencyPersonnelListItem[]>([]);
const selectedRecruiters = ref<AgencyPersonnelListItem[]>([]);

const selectedDays = ref<string[]>(props.preset?.workDate ? [props.preset.workDate] : []);

const canAssign = computed(
  () => !!selectedOrder.value && selectedDays.value.length > 0 && selectedRecruiters.value.length > 0
);

function onOrderTyping(value: string): void {
  const filter: AgencyRequestFilter = {
    filter: value,
    statuses: [RequestStatus.Open, RequestStatus.Filled],
  };
  isLoadingOrders.value = true;
  getAllAgencyRequests(filter)
    .then(response => {
      orders.value = response;
    })
    .catch(error => showAlertError(error))
    .finally(() => {
      isLoadingOrders.value = false;
    });
}

function onOrderSelected(option: AgencyRequestListItem | null): void {
  selectedOrder.value = option;
}

function loadPersonnel(): void {
  getAgencyPersonnel()
    .then(response => {
      personnel.value = response;
      filteredRecruiters.value = response;
      if (props.preset?.recruiterId) {
        selectedRecruiters.value = response.filter(r => r.id === props.preset?.recruiterId);
      }
    })
    .catch(error => showAlertError(error));
}

function onRecruiterTyping(text: string): void {
  const term = (text || '').toLowerCase();
  const selectedIds = new Set(selectedRecruiters.value.map(s => s.id));
  filteredRecruiters.value = personnel.value.filter(
    p => !selectedIds.has(p.id) && p.name.toLowerCase().includes(term)
  );
}

function toggleDay(date: string): void {
  const index = selectedDays.value.indexOf(date);
  if (index >= 0) selectedDays.value.splice(index, 1);
  else selectedDays.value.push(date);
}

function submit(): void {
  if (!canAssign.value || !selectedOrder.value) return;
  emit('assign', {
    requestId: selectedOrder.value.id,
    workDates: [...selectedDays.value],
    recruiterIds: selectedRecruiters.value.map(r => r.id),
  });
}

loadPersonnel();
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.assign-modal {
  width: 100%;

  .day-grid {
    display: grid;
    grid-template-columns: repeat(7, minmax(0, 1fr));
    gap: 0.5rem;
    width: 100%;
  }

  .day-cell {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.25rem;
    min-width: 0;
    padding: 0.6rem 0;
    border: 1px solid $border-input;
    border-radius: 8px;
    background: $white;
    cursor: pointer;
    transition: all 0.15s ease;

    .day-name {
      font-size: 0.7rem;
      font-weight: 600;
      color: $grey-dark;
    }

    .day-num {
      font-size: 1.1rem;
      font-weight: 700;
    }

    &.is-selected {
      border-color: $primary;
      background: rgba($primary, 0.12);
    }
  }
}
</style>
