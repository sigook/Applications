<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <PageHeader title="Create PayStub" :crumbs="payStubsCrumbs" back-to="/accounting/paystubs" />
    <div>
      <div class="columns is-multiline">
        <div class="column is-4">
          <b-field label="Worker" :type="formErrors.worker ? 'is-danger' : ''"
            :message="formErrors.worker || 'Type at least 3 characters to search'">
            <b-autocomplete v-model="worker" :data="workers" placeholder="Worker" name="worker"
              :loading="isLoadingList" @typing="onWorkerInput" @select="selectWorker">
            </b-autocomplete>
          </b-field>
        </div>
        <div class="column is-4">
          <b-field label="Position" :type="formErrors.position ? 'is-danger' : ''"
            :message="formErrors.position">
            <b-input v-model="position" name="position"></b-input>
          </b-field>
        </div>
        <div class="column is-4">
          <b-field label="Dates of work" :type="formErrors.datesOfWork ? 'is-danger' : ''"
            :message="formErrors.datesOfWork">
            <b-datepicker v-model="datesOfWork" name="datesOfWork" range @update:modelValue="onDatesSelected" />
          </b-field>
        </div>
        <div class="column is-12">
          <b-field label="Pay Vacations">
            <b-switch v-model="payVacations">
              {{ payVacations ? 'Yes' : 'No' }}
            </b-switch>
          </b-field>
        </div>
      </div>
      <div class="columns is-multiline">
        <!-- Items Section -->
        <div class="column is-12">
          <h3 class="fz1 has-text-weight-semibold mb-3">Items</h3>
          <div class="expandable-section-container mb-5">
            <div class="expandable-section-header" @click="addItem"
              :class="{ 'is-disabled': availableItemTypes.length === 0 }">
              <h3 class="expandable-section-title fz1 has-text-weight-semibold mb-2 has-text-centered">
                <b-icon icon="plus-circle" class="mr-2"></b-icon>
                Add Item
              </h3>
              <p class="fz-1 color-gray mb-0 has-text-centered">Click here to add an item to the pay stub</p>
            </div>
            <div class="expandable-section-list" v-if="items.length > 0">
              <div class="columns is-multiline" v-for="(item, i) in items" :key="i">
                <div class="column is-6-mobile is-3">
                  <b-field label="Type" expanded :type="itemErrors['type' + i] ? 'is-danger' : ''"
                    :message="itemErrors['type' + i]">
                    <b-select v-model="item.type" :name="'type' + i" expanded
                      placeholder="Select a type" @update:modelValue="onItemTypeChanged(item)">
                      <option v-for="opt in getAvailableTypesForItem(item)" :key="opt.type" :value="opt.type"
                        :disabled="opt.disabled">
                        {{ opt.label }}{{ opt.disabled ? ' (auto-calculated)' : '' }}
                      </option>
                    </b-select>
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Description" expanded>
                    <b-input v-model="item.description" placeholder="Optional"></b-input>
                  </b-field>
                </div>
                <div class="column is-6-mobile is-2">
                  <b-field label="Quantity" expanded :type="itemErrors['qty' + i] ? 'is-danger' : ''"
                    :message="itemErrors['qty' + i]">
                    <b-numberinput v-model="item.quantity" :min="1" :max="1000000" :step="0.01" :controls="false"
                      :name="'quantity' + i" @update:modelValue="updateItem(item)" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-2">
                  <b-field label="Price" expanded :type="itemErrors['price' + i] ? 'is-danger' : ''"
                    :message="itemErrors['price' + i]">
                    <b-numberinput v-model="item.unitPrice" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'unitPrice' + i" @update:modelValue="updateItem(item)" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-2">
                  <b-field label="Total" expanded>
                    <b-input v-model="item.total" disabled></b-input>
                  </b-field>
                </div>
                <div class="column is-2-mobile is-1">
                  <b-field label="Delete" expanded>
                    <b-button type="is-danger" outlined rounded icon-right="delete"
                      @click="removeItem(item)"></b-button>
                  </b-field>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Discount Section -->
        <div class="column is-12">
          <h3 class="fz1 has-text-weight-semibold mb-3">Discount</h3>
          <div class="expandable-section-container">
            <div class="expandable-section-header" @click="addDiscount" :class="{ 'is-disabled': discount }">
              <h3 class="expandable-section-title fz1 has-text-weight-semibold mb-2 has-text-centered">
                <b-icon icon="minus-circle" class="mr-2"></b-icon>
                Add Discount
              </h3>
              <p class="fz-1 color-gray mb-0 has-text-centered">Click here to add a discount to the pay stub</p>
            </div>
            <div class="expandable-section-list" v-if="discount">
              <div class="columns is-multiline">
                <div class="column is-4-mobile is-4 is-5-desktop">
                  <b-field label="Description" expanded :type="itemErrors['discountDescription'] ? 'is-danger' : ''"
                    :message="itemErrors['discountDescription']">
                    <b-input v-model="discount.description" name="discountDescription"
                      placeholder="Enter discount description">
                    </b-input>
                  </b-field>
                </div>
                <div class="column is-4-mobile is-4 is-5-desktop">
                  <b-field label="Amount" expanded :type="itemErrors['discountAmount'] ? 'is-danger' : ''"
                    :message="itemErrors['discountAmount']">
                    <b-numberinput v-model="discount.amount" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      name="discountAmount" expanded>
                    </b-numberinput>
                  </b-field>
                </div>
                <div class="column is-4-mobile is-4 is-2-desktop">
                  <b-field label="Delete" expanded>
                    <b-button type="is-danger" outlined rounded icon-right="delete" @click="removeDiscount">
                    </b-button>
                  </b-field>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="column is-12 mt-4">
          <b-button type="is-primary" @click="createPayStub">Create PayStub</b-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import * as yup from 'yup';
import dayjs from 'dayjs';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { getAgencyWorkersDropdown } from '@/api/agencyWorkerApi';
import { createAgencyPayStub } from '@/api/agencyPayStubApi';
import { useStickyForm } from '@/composables/useStickyForm';
import PageHeader from '@/components/PageHeader.vue';
import { payStubsCrumbs } from '@/constants/breadcrumbs';

const PayStubItemType = {
  Regular: 0,
  OtherRegular: 1,
  Overtime: 2,
  StatutoryHoliday: 3,
  StatutoryWorkedHoliday: 4,
  NightShift: 5,
  Missing: 6,
  MissingOvertime: 7,
  Vacations: 8,
  Other: 9,
  Reimbursement: 10,
} as const;

const itemTypeOptions = [
  { type: PayStubItemType.Regular, label: 'Regular Hours' },
  { type: PayStubItemType.Overtime, label: 'Overtime Hours' },
  { type: PayStubItemType.Missing, label: 'Missing Hours' },
  { type: PayStubItemType.MissingOvertime, label: 'Missing Overtime Hours' },
  { type: PayStubItemType.StatutoryHoliday, label: 'Statutory Holiday Pay' },
  { type: PayStubItemType.Vacations, label: 'Vacations' },
  { type: PayStubItemType.Other, label: 'Bonus/Others' },
  { type: PayStubItemType.Reimbursement, label: 'Reimbursement' },
];

const multipleAllowedTypes: number[] = [PayStubItemType.Other, PayStubItemType.Reimbursement];

interface PayStubItem {
  type: number | null;
  description: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

interface Discount {
  description: string;
  amount: number;
}

const router = useRouter();

const schema = yup.object({
  worker: yup.string().required('Worker is required'),
  position: yup.string().required('Position is required'),
  datesOfWork: yup.array().of(yup.date()).min(2, 'Dates are required').required('Dates are required'),
});

const form = useStickyForm<{ worker: string; position: string; datesOfWork: Date[] }>({
  schema,
  initialValues: { worker: '', position: '', datesOfWork: [] },
});
const { worker, position, datesOfWork } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const isLoadingList = ref(false);
const payVacations = ref(false);
const workerProfileId = ref<string | null>(null);
const workBegins = ref<string | null>(null);
const workEnd = ref<string | null>(null);
const workers = ref<any[]>([]);
const items = ref<PayStubItem[]>([]);
const discount = ref<Discount | null>(null);
const itemErrors = reactive<Record<string, string>>({});

const startOfWeek = dayjs().startOf('week').day(0);
const endOfWeek = startOfWeek.add(6, 'day');
datesOfWork.value = [startOfWeek.toDate(), endOfWeek.toDate()];
onDatesSelected();

const availableItemTypes = computed(() => {
  const selectedTypes = items.value.filter(i => i.type !== null).map(i => i.type);
  return itemTypeOptions.filter(opt =>
    multipleAllowedTypes.includes(opt.type) || !selectedTypes.includes(opt.type)
  );
});

watch(payVacations, (value) => {
  if (value) {
    items.value = items.value.filter(i => i.type !== PayStubItemType.Vacations);
  }
});

function onWorkerInput(text: string) {
  if (text.length >= 3) searchWorkers(text);
  else workers.value = [];
}

function onDatesSelected() {
  const d = datesOfWork.value;
  if (d && d.length === 2) {
    workBegins.value = dayjs(d[0]).format('YYYY-MM-DD');
    workEnd.value = dayjs(d[1]).format('YYYY-MM-DD');
  }
}

function searchWorkers(text: string) {
  isLoadingList.value = true;
  getAgencyWorkersDropdown({ searchTerm: text })
    .then(response => {
      isLoadingList.value = false;
      workers.value = response;
    })
    .catch(error => {
      isLoadingList.value = false;
      showAlertError(error);
    });
}

function selectWorker(w: any) {
  workerProfileId.value = w ? w.workerProfileId : null;
}

function addItem() {
  if (availableItemTypes.value.length === 0) return;
  items.value.push({ type: null, description: '', quantity: 1, unitPrice: 0, total: 0 });
}

function onItemTypeChanged(item: PayStubItem) {
  if (item.type === PayStubItemType.Vacations && payVacations.value) {
    item.type = null;
  }
}

function updateItem(item: PayStubItem) {
  item.total = item.quantity * item.unitPrice;
}

function removeItem(item: PayStubItem) {
  items.value = items.value.filter(i => i !== item);
}

function addDiscount() {
  discount.value = { description: '', amount: 0 };
}

function removeDiscount() {
  discount.value = null;
}

function getAvailableTypesForItem(currentItem: PayStubItem) {
  const selectedTypes = items.value
    .filter(i => i !== currentItem && i.type !== null)
    .map(i => i.type);
  return itemTypeOptions
    .filter(opt =>
      multipleAllowedTypes.includes(opt.type) ||
      !selectedTypes.includes(opt.type) ||
      opt.type === currentItem.type
    )
    .map(opt => ({
      ...opt,
      disabled: opt.type === PayStubItemType.Vacations && payVacations.value,
    }));
}

function validateItems(): boolean {
  for (const k of Object.keys(itemErrors)) delete itemErrors[k];
  let ok = true;
  items.value.forEach((it, i) => {
    if (it.type === null || it.type === undefined) { itemErrors['type' + i] = 'Required'; ok = false; }
    if (!it.quantity || it.quantity < 0.01) { itemErrors['qty' + i] = 'Required'; ok = false; }
    if (!it.unitPrice || it.unitPrice < 0.01) { itemErrors['price' + i] = 'Required'; ok = false; }
  });
  if (discount.value) {
    if (!discount.value.description) { itemErrors['discountDescription'] = 'Required'; ok = false; }
    if (!discount.value.amount || discount.value.amount < 0.01) { itemErrors['discountAmount'] = 'Required'; ok = false; }
  }
  return ok;
}

async function createPayStub() {
  form.markInteracted();
  const { valid } = await form.validate();
  const itemsOk = validateItems();
  if (!valid || !itemsOk) return;
  isLoading.value = true;
  const payload = {
    workerProfileId: workerProfileId.value,
    position: position.value,
    workBegins: workBegins.value,
    workEnd: workEnd.value,
    payVacations: payVacations.value,
    items: items.value
      .filter(i => i.type !== null)
      .map(i => ({
        type: i.type,
        quantity: i.quantity,
        unitPrice: i.unitPrice,
        description: i.description || null,
      })),
    otherDeductions: discount.value ? discount.value.amount : 0,
    otherDeductionsDescription: discount.value ? discount.value.description : null,
  };
  createAgencyPayStub(payload)
    .then(() => {
      isLoading.value = false;
      showAlertSuccess('PayStub created successfully');
      router.push('/accounting/paystubs');
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>

<style scoped>
.expandable-section-header.is-disabled,
.expandable-section-header[disabled] {
  opacity: 0.6 !important;
  cursor: not-allowed !important;
  pointer-events: none !important;
}

:deep(select option:disabled) {
  color: #b5b5b5;
  font-style: italic;
}
</style>
