<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Create PayStub
      </h2>
    </div>
    <div>
      <div class="container-flex">
        <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
          <b-field label="Worker" :type="errors.has('worker') ? 'is-danger' : ''"
            :message="errors.has('worker') ? errors.first('worker') : 'Type at least 3 characters to search'">
            <b-autocomplete v-model="workerSelected" :data="workers" placeholder="Worker" name="worker"
              :loading="isLoadingList" @typing="onWorkerInput" v-validate="'required'" @select="selectWorker">
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
          <b-field label="Position" :type="errors.has('position') ? 'is-danger' : ''"
            :message="errors.has('position') ? errors.first('position') : ''">
            <b-input v-model="payStub.position" name="position" v-validate="'required'"></b-input>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
          <b-field label="Dates of work" :type="errors.has('datesOfWork') ? 'is-danger' : ''"
            :message="errors.has('datesOfWork') ? errors.first('datesOfWork') : ''">
            <b-datepicker v-model="datesSelected" name="datesOfWork" range @input="onDatesSelected"
              v-validate="'required'" />
          </b-field>
        </div>
        <div class="col-12 col-padding">
          <b-field label="Pay Vacations">
            <b-switch v-model="payStub.payVacations">
              {{ payStub.payVacations ? 'Yes' : 'No' }}
            </b-switch>
          </b-field>
        </div>
      </div>
      <div class="container-flex">
        <!-- Items Section -->
        <div class="col-12 col-padding">
          <h3 class="fz1 fw-600 mb-3">Items</h3>
          <div class="expandable-section-container mb-5">
            <div class="expandable-section-header" @click="addItem"
              :class="{ 'is-disabled': availableItemTypes.length === 0 }">
              <h3 class="expandable-section-title fz1 fw-600 mb-2 text-center">
                <b-icon icon="plus-circle" class="mr-2"></b-icon>
                Add Item
              </h3>
              <p class="fz-1 color-gray mb-0 text-center">Click here to add an item to the pay stub</p>
            </div>
            <div class="expandable-section-list" v-if="items.length > 0">
              <div class="container-flex" v-for="(item, i) in items" :key="i">
                <div class="col-sm-6 col-md-3 col-lg-3 col-padding">
                  <b-field label="Type" expanded :type="errors.has('type' + i) ? 'is-danger' : ''"
                    :message="errors.has('type' + i) ? errors.first('type' + i) : ''">
                    <b-select v-model="item.type" :name="'type' + i" v-validate="'required'" expanded
                      placeholder="Select a type" @input="onItemTypeChanged(item)">
                      <option v-for="opt in getAvailableTypesForItem(item)" :key="opt.type" :value="opt.type"
                        :disabled="opt.disabled">
                        {{ opt.label }}{{ opt.disabled ? ' (auto-calculated)' : '' }}
                      </option>
                    </b-select>
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Description" expanded>
                    <b-input v-model="item.description" placeholder="Optional"></b-input>
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-2 col-lg-2 col-padding">
                  <b-field label="Quantity" expanded :type="errors.has('quantity' + i) ? 'is-danger' : ''"
                    :message="errors.has('quantity' + i) ? errors.first('quantity' + i) : ''">
                    <b-numberinput v-model="item.quantity" :min="1" :max="1000000" :step="0.01" :controls="false"
                      :name="'quantity' + i" v-validate="'required|min_value:0.01'" @input="updateItem(item)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-2 col-lg-2 col-padding">
                  <b-field label="Price" expanded :type="errors.has('unitPrice' + i) ? 'is-danger' : ''"
                    :message="errors.has('unitPrice' + i) ? errors.first('unitPrice' + i) : ''">
                    <b-numberinput v-model="item.unitPrice" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'unitPrice' + i" v-validate="'required|min_value:0.01'" @input="updateItem(item)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-2 col-lg-2 col-padding">
                  <b-field label="Total" expanded>
                    <b-input v-model="item.total" disabled></b-input>
                  </b-field>
                </div>
                <div class="col-sm-2 col-md-1 col-lg-1 col-padding">
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
        <div class="col-12 col-padding">
          <h3 class="fz1 fw-600 mb-3">Discount</h3>
          <div class="expandable-section-container">
            <div class="expandable-section-header" @click="addDiscount" :class="{ 'is-disabled': discount }"
              :disabled="!!discount">
              <h3 class="expandable-section-title fz1 fw-600 mb-2 text-center">
                <b-icon icon="minus-circle" class="mr-2"></b-icon>
                Add Discount
              </h3>
              <p class="fz-1 color-gray mb-0 text-center">Click here to add a discount to the pay stub</p>
            </div>
            <div class="expandable-section-list" v-if="discount">
              <div class="container-flex">
                <div class="col-sm-4 col-md-4 col-lg-5 col-padding">
                  <b-field label="Description" expanded :type="errors.has('discountDescription') ? 'is-danger' : ''"
                    :message="errors.has('discountDescription') ? errors.first('discountDescription') : ''">
                    <b-input v-model="discount.description" name="discountDescription" v-validate="'required'"
                      placeholder="Enter discount description">
                    </b-input>
                  </b-field>
                </div>
                <div class="col-sm-4 col-md-4 col-lg-5 col-padding">
                  <b-field label="Amount" expanded :type="errors.has('discountAmount') ? 'is-danger' : ''"
                    :message="errors.has('discountAmount') ? errors.first('discountAmount') : ''">
                    <b-numberinput v-model="discount.amount" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      name="discountAmount" v-validate="'required|min_value:0.01'" expanded>
                    </b-numberinput>
                  </b-field>
                </div>
                <div class="col-sm-4 col-md-4 col-lg-2 col-padding">
                  <b-field label="Delete" expanded>
                    <b-button type="is-danger" outlined rounded icon-right="delete" @click="removeDiscount">
                    </b-button>
                  </b-field>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-12 col-padding mt-4">
          <b-button type="is-primary" @click="createPayStub">Create PayStub</b-button>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import dayjs from 'dayjs';
import { getAgencyWorkersDropdown } from "@/api/agencyWorkerApi";
import { createAgencyPayStub } from "@/api/agencyPayStubApi";

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
  Reimbursement: 10
};

const itemTypeOptions = [
  { type: PayStubItemType.Regular, label: 'Regular Hours' },
  { type: PayStubItemType.Overtime, label: 'Overtime Hours' },
  { type: PayStubItemType.Missing, label: 'Missing Hours' },
  { type: PayStubItemType.MissingOvertime, label: 'Missing Overtime Hours' },
  { type: PayStubItemType.StatutoryHoliday, label: 'Statutory Holiday Pay' },
  { type: PayStubItemType.Vacations, label: 'Vacations' },
  { type: PayStubItemType.Other, label: 'Bonus/Others' },
  { type: PayStubItemType.Reimbursement, label: 'Reimbursement' }
];

const multipleAllowedTypes = [PayStubItemType.Other, PayStubItemType.Reimbursement];

export default {
  data() {
    return {
      isLoading: false,
      isLoadingList: false,
      payStub: {},
      workers: [],
      workerSelected: null,
      datesSelected: [],
      items: [],
      discount: null
    }
  },
  created() {
    const startOfWeek = dayjs().startOf('week').day(0);
    const endOfWeek = startOfWeek.add(6, 'day');
    this.datesSelected = [startOfWeek.toDate(), endOfWeek.toDate()];
    this.onDatesSelected();
  },
  methods: {
    onWorkerInput(text) {
      if (text.length >= 3) {
        this.searchWorkers(text);
      } else {
        this.workers = [];
      }
    },
    onDatesSelected() {
      this.payStub.workBegins = dayjs(this.datesSelected[0]).format('YYYY-MM-DD');
      this.payStub.workEnd = dayjs(this.datesSelected[1]).format('YYYY-MM-DD');
    },
    searchWorkers(text) {
      this.isLoadingList = true;
      getAgencyWorkersDropdown({ searchTerm: text })
        .then(response => {
          this.isLoadingList = false;
          this.workers = response;
        })
        .catch(error => {
          this.isLoadingList = false;
          this.showAlertError(error);
        });
    },
    selectWorker(worker) {
      if (worker) {
        this.payStub.workerProfileId = worker.workerProfileId;
      } else {
        this.payStub.workerProfileId = null;
      }
    },
    addItem() {
      if (this.availableItemTypes.length === 0) return;
      this.items.push({
        type: null,
        description: '',
        quantity: 1,
        unitPrice: 0,
        total: 0
      });
    },
    onItemTypeChanged(item) {
      if (item.type === PayStubItemType.Vacations && this.payStub.payVacations) {
        item.type = null;
      }
    },
    updateItem(item) {
      item.total = item.quantity * item.unitPrice;
    },
    removeItem(item) {
      this.items = this.items.filter(i => i !== item);
    },
    addDiscount() {
      this.discount = {
        description: '',
        amount: 0
      };
    },
    removeDiscount() {
      this.discount = null;
    },
    getAvailableTypesForItem(currentItem) {
      const selectedTypes = this.items
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
          disabled: opt.type === PayStubItemType.Vacations && this.payStub.payVacations
        }));
    },
    async createPayStub() {
      const validation = await this.$validator.validateAll();
      if (validation) {
        this.isLoading = true;
        const payload = {
          workerProfileId: this.payStub.workerProfileId,
          position: this.payStub.position,
          workBegins: this.payStub.workBegins,
          workEnd: this.payStub.workEnd,
          payVacations: this.payStub.payVacations,
          items: this.items
            .filter(i => i.type !== null)
            .map(i => ({
              type: i.type,
              quantity: i.quantity,
              unitPrice: i.unitPrice,
              description: i.description || null
            })),
          otherDeductions: this.discount ? this.discount.amount : 0,
          otherDeductionsDescription: this.discount ? this.discount.description : null
        };
        createAgencyPayStub(payload)
          .then(() => {
            this.isLoading = false;
            this.showAlertSuccess('PayStub created successfully');
            this.$router.push('/accounting/paystubs');
          })
          .catch(error => {
            this.isLoading = false;
            this.showAlertError(error);
          });
      }
    }
  },
  watch: {
    'payStub.payVacations'(value) {
      if (value) {
        this.items = this.items.filter(i => i.type !== PayStubItemType.Vacations);
      }
    }
  },
  computed: {
    availableItemTypes() {
      const selectedTypes = this.items.filter(i => i.type !== null).map(i => i.type);
      return itemTypeOptions.filter(opt =>
        multipleAllowedTypes.includes(opt.type) || !selectedTypes.includes(opt.type)
      );
    }
  }
}
</script>

<style scoped>
.expandable-section-header.is-disabled,
.expandable-section-header[disabled] {
  opacity: 0.6 !important;
  cursor: not-allowed !important;
  pointer-events: none !important;
}

::v-deep select option:disabled {
  color: #b5b5b5;
  font-style: italic;
}
</style>
