<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title columns is-multiline mb-2">
      <h2 class="fz1 pt-3 column is-7-mobile is-5">
        Create Invoice
      </h2>
    </div>
    <div>
      <div class="columns is-multiline">
        <div class="column is-12">
          <b-field label="Direct Hiring?">
            <b-switch v-model="invoice.directHiring">
              {{ invoice.directHiring ? 'Yes' : 'No' }}
            </b-switch>
          </b-field>
        </div>

        <div class="column is-4">
          <b-field label="Company" :type="formErrors.company ? 'is-danger' : ''"
            :message="formErrors.company">
            <b-autocomplete v-model="company" :data="filteredCompanies" open-on-focus
              field="fullName" name="company" placeholder="Company" @select="selectCompany">
            </b-autocomplete>
          </b-field>
        </div>

        <div class="column is-4" v-if="invoice.directHiring">
          <b-field label="Tax (%)"
            :message="'Direct hiring invoices have no timesheets: enter the tax percentage to apply'">
            <b-numberinput v-model="invoice.taxPercentage" :min="0" :max="100" :step="0.01" :controls="false"
              name="taxPercentage" placeholder="e.g. 10 for 10%" />
          </b-field>
        </div>

        <div class="column is-4" v-else>
          <b-field label="Request (optional)"
            :message="'Optional: select one or more requests to invoice. Leave empty to invoice all the company\'s timesheets.'">
            <b-taginput v-model="selectedRequests" autocomplete :data="filteredRequests" open-on-focus
              field="numberId" icon="label" name="requests" placeholder="Request, Position"
              :disabled="!enabledRequests" :loading="isLoadingRequests"
              :custom-formatter="(option: any) => `${option.numberId} | ${option.jobTitle}`"
              @typing="filterRequests" @update:modelValue="onRequestsSelected" append-to-body>
              <template v-slot="props">
                <small>{{ props.option.numberId }} | {{ props.option.jobTitle }}</small>
              </template>
            </b-taginput>
          </b-field>
        </div>

        <div class="column is-4">
          <b-field label="Client's Site Address (optional)"
            :message="'Optional: Address of the client\'s job site to appear on the invoice'">
            <b-input v-model="invoice.clientSiteAddress" placeholder="Enter client's site address" />
          </b-field>
        </div>

        <div class="column is-4">
          <b-field label="Email (optional)" :type="formErrors.email ? 'is-danger' : ''"
            :message="formErrors.email || 'Fill this input if the company needs a different email for this specific invoice'">
            <b-input type="email" v-model="email" name="email" />
          </b-field>
        </div>

        <div class="column is-4">
          <b-field label="Date (optional)"
            :message="'Fill this input if the company needs a different date for this specific invoice'">
            <b-datepicker v-model="invoice.invoiceDate" />
          </b-field>
        </div>

        <div class="column is-4">
          <b-field label="Dates (From - To) (optional)"
            :message="'Optional: Dates to filter timesheets. Leave empty to include all timesheets from the beginning.'">
            <b-datepicker v-model="dateSelected" placeholder="Select start date for filtering" range
              @update:modelValue="onDatesSelected" />
          </b-field>
        </div>
      </div>

      <!-- Additional Items Section -->
      <div class="columns is-multiline">
        <div class="column is-12">
          <hr class="my-4">
          <div class="expandable-section-container">
            <div class="expandable-section-header" @click="addItem">
              <h3 class="expandable-section-title fz1 has-text-weight-semibold mb-2 has-text-centered">
                <b-icon icon="plus-circle" class="mr-2"></b-icon>
                Add Additional Items
              </h3>
              <p class="fz-1 color-gray mb-0 has-text-centered">Click here to add extra items or charges not included in
                regular timesheet billing</p>
            </div>

            <div class="expandable-section-list" v-if="additionalItems.length > 0">
              <div class="columns is-multiline" v-for="(item, i) in additionalItems" :key="i">
                <div class="column is-6-mobile is-3 is-4-desktop">
                  <b-field label="Description" expanded :type="itemErrors['aDesc' + i] ? 'is-danger' : ''"
                    :message="itemErrors['aDesc' + i]">
                    <b-input v-model="item.description" :name="'description' + i" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Quantity" expanded :type="itemErrors['aQty' + i] ? 'is-danger' : ''"
                    :message="itemErrors['aQty' + i]">
                    <b-numberinput v-model="item.quantity" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'quantity' + i"
                      @update:modelValue="updateTotalAdditionalItems(item)" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Price" expanded :type="itemErrors['aPrice' + i] ? 'is-danger' : ''"
                    :message="itemErrors['aPrice' + i]">
                    <b-numberinput v-model="item.unitPrice" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'unitPrice' + i"
                      @update:modelValue="updateTotalAdditionalItems(item)" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Total" expanded>
                    <b-input v-model="item.total" disabled></b-input>
                  </b-field>
                </div>
                <div class="column is-2-mobile is-3 is-2-desktop">
                  <b-field label="Delete" expanded>
                    <b-button type="is-danger" outlined rounded icon-right="delete"
                      @click="removeItem(item)"></b-button>
                  </b-field>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Discounts Section -->
      <div class="columns is-multiline">
        <div class="column is-12">
          <hr class="my-4">
          <div class="expandable-section-container">
            <div class="expandable-section-header" @click="addDiscount">
              <h3 class="expandable-section-title fz1 has-text-weight-semibold mb-2 has-text-centered">
                <b-icon icon="minus-circle" class="mr-2"></b-icon>
                Add Discounts
              </h3>
              <p class="fz-1 color-gray mb-0 has-text-centered">Click here to add discounts or reductions to apply to this
                invoice</p>
            </div>

            <div class="expandable-section-list" v-if="discounts.length > 0">
              <div class="columns is-multiline" v-for="(discount, i) in discounts" :key="i">
                <div class="column is-6-mobile is-3 is-4-desktop">
                  <b-field label="Description" expanded :type="itemErrors['dDesc' + i] ? 'is-danger' : ''"
                    :message="itemErrors['dDesc' + i]">
                    <b-input v-model="discount.description" :name="'discountDescription' + i" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Quantity" expanded :type="itemErrors['dQty' + i] ? 'is-danger' : ''"
                    :message="itemErrors['dQty' + i]">
                    <b-numberinput v-model="discount.quantity" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'dQuantity' + i"
                      @update:modelValue="updateTotalDiscounts(discount)" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Price" expanded :type="itemErrors['dPrice' + i] ? 'is-danger' : ''"
                    :message="itemErrors['dPrice' + i]">
                    <b-numberinput v-model="discount.unitPrice" :min="0.01" :max="1000000" :step="0.01"
                      :controls="false" :name="'dUnitPrice' + i"
                      @update:modelValue="updateTotalDiscounts(discount)" />
                  </b-field>
                </div>
                <div class="column is-6-mobile is-3 is-2-desktop">
                  <b-field label="Total" expanded>
                    <b-input v-model="discount.total" disabled></b-input>
                  </b-field>
                </div>
                <div class="column is-2-mobile is-3 is-2-desktop">
                  <b-field label="Delete" expanded>
                    <b-button type="is-danger" outlined rounded icon-right="delete"
                      @click="removeDiscount(discount)"></b-button>
                  </b-field>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="">
        <b-button type="is-primary" @click="loadPreview">
          Preview Invoice
        </b-button>
      </div>

      <!-- Preview Section -->
      <div v-if="previewData" class="mt-4" ref="previewContainer">
        <div class="box">
          <PreviewInvoice :preview="previewData" />
          <div class="buttons is-justify-content-flex-end mt-4">
            <b-button @click="cancelPreview" class="mr-2">
              Cancel
            </b-button>
            <b-button type="is-primary" @click="confirmInvoice">
              Confirm
            </b-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, nextTick, watch } from 'vue';
import { useRouter } from 'vue-router';
import * as yup from 'yup';
import dayjs from 'dayjs';
import { showAlertError } from '@/utils/toast';
import { getAgencyCompanyProfileWithRequests } from '@/api/agencyCompanyApi';
import { getAllAgencyRequests } from '@/api/agencyRequestApi';
import { previewAgencyInvoice, createAgencyInvoice } from '@/api/agencyInvoiceApi';
import { useStickyForm } from '@/composables/useStickyForm';
import PreviewInvoice from '@/components/agency_accounting/PreviewInvoice.vue';

const router = useRouter();

const schema = yup.object({
  company: yup.string().required('Company is required'),
  email: yup.string().email('Invalid email').nullable().transform((v) => v || null),
});

const form = useStickyForm<{ company: string; email: string }>({
  schema,
  initialValues: { company: '', email: '' },
});
const { company, email } = form.fields;
const formErrors = form.errors;

const itemErrors = reactive<Record<string, string>>({});
const isLoading = ref(false);
const invoice = ref<any>({});
const companies = ref<any[]>([]);
const requests = ref<any[]>([]);
const selectedRequests = ref<any[]>([]);
const requestSearch = ref('');
const isLoadingRequests = ref(false);
const additionalItems = ref<any[]>([]);
const discounts = ref<any[]>([]);
const previewData = ref<any>(null);
const dateSelected = ref<Date[]>([]);
const previewContainer = ref<HTMLElement | null>(null);

const filteredCompanies = computed(() =>
  companies.value.filter((c: any) => c.fullName.toLowerCase().includes((company.value || '').toLowerCase())),
);

const enabledRequests = computed(() => !!company.value && requests.value.length > 0);

const filteredRequests = computed(() => {
  const term = (requestSearch.value || '').toLowerCase();
  return requests.value.filter((r: any) =>
    !selectedRequests.value.some((s: any) => s.id === r.id) &&
    (`${r.numberId}`.includes(term) || (r.jobTitle || '').toLowerCase().includes(term)),
  );
});

(async () => {
  await loadCompanies();
})();

async function loadCompanyRequests(companyProfileId: string) {
  isLoadingRequests.value = true;
  try {
    requests.value = await getAllAgencyRequests({ companyProfileId, statuses: [1, 3], pageSize: 1000, pageIndex: 0 });
  } finally {
    isLoadingRequests.value = false;
  }
}

function loadPreview() {
  validateForm();
}

function onDatesSelected() {
  if (dateSelected.value && dateSelected.value.length === 2) {
    invoice.value.from = dayjs(dateSelected.value[0]).format('YYYY-MM-DD');
    invoice.value.to = dayjs(dateSelected.value[1]).format('YYYY-MM-DD');
  }
}

async function loadCompanies() {
  companies.value = await getAgencyCompanyProfileWithRequests();
}

function selectCompany(c: any) {
  resetRequests();
  if (c) {
    invoice.value.companyProfileId = c.id;
    loadCompanyRequests(c.id);
  } else {
    invoice.value = {};
    requests.value = [];
  }
}

function filterRequests(value: string) {
  requestSearch.value = value;
}

function onRequestsSelected() {
  invoice.value.requestIds = selectedRequests.value.map((r: any) => r.id);
}

function resetRequests() {
  selectedRequests.value = [];
  requestSearch.value = '';
  invoice.value.requestIds = [];
}

watch(() => invoice.value.directHiring, (isDirectHiring) => {
  if (isDirectHiring) {
    resetRequests();
  } else {
    invoice.value.taxPercentage = undefined;
  }
});

function updateTotalAdditionalItems(item: any) {
  item.total = item.quantity * item.unitPrice;
}

function updateTotalDiscounts(discount: any) {
  discount.total = discount.quantity * discount.unitPrice;
}

function validateItems(): boolean {
  for (const k of Object.keys(itemErrors)) delete itemErrors[k];
  let ok = true;
  additionalItems.value.forEach((it, i) => {
    if (!it.description) { itemErrors['aDesc' + i] = 'Required'; ok = false; }
    if (!it.quantity || it.quantity < 0.01) { itemErrors['aQty' + i] = 'Required'; ok = false; }
    if (!it.unitPrice || it.unitPrice < 0.01) { itemErrors['aPrice' + i] = 'Required'; ok = false; }
  });
  discounts.value.forEach((d, i) => {
    if (!d.description) { itemErrors['dDesc' + i] = 'Required'; ok = false; }
    if (!d.quantity || d.quantity < 0.01) { itemErrors['dQty' + i] = 'Required'; ok = false; }
    if (!d.unitPrice || d.unitPrice < 0.01) { itemErrors['dPrice' + i] = 'Required'; ok = false; }
  });
  return ok;
}

async function validateForm() {
  form.markInteracted();
  const { valid } = await form.validate();
  const itemsOk = validateItems();
  if (!valid || !itemsOk) return;
  isLoading.value = true;
  invoice.value.email = email.value || undefined;
  invoice.value.additionalItems = additionalItems.value;
  invoice.value.discounts = discounts.value;
  previewAgencyInvoice(invoice.value)
    .then((response) => {
      isLoading.value = false;
      previewData.value = response;
      nextTick(() => {
        const el = previewContainer.value?.querySelector('.box');
        if (el) {
          el.scrollIntoView({ behavior: 'smooth' });
        }
      });
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addItem() {
  additionalItems.value.push({
    quantity: 0,
    unitPrice: 0,
    total: 0,
    description: '',
  });
}

function removeItem(item: any) {
  additionalItems.value = additionalItems.value.filter((i) => i !== item);
}

function addDiscount() {
  discounts.value.push({});
}

function removeDiscount(discount: any) {
  discounts.value = discounts.value.filter((d) => d !== discount);
}

function confirmInvoice() {
  isLoading.value = true;
  createAgencyInvoice(invoice.value)
    .then(() => {
      isLoading.value = false;
      router.push('/accounting/invoices');
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function cancelPreview() {
  previewData.value = null;
  window.scrollTo({ top: 0, behavior: 'smooth' });
}
</script>
