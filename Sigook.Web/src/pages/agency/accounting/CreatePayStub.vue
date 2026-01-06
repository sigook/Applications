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
          <b-field label="Type of work" :type="errors.has('typeOfWork') ? 'is-danger' : ''"
            :message="errors.has('typeOfWork') ? errors.first('typeOfWork') : ''">
            <b-input v-model="payStub.typeOfWork" name="typeOfWork" v-validate="'required'"></b-input>
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
        <!-- Regular Items Section -->
        <div class="col-12 col-padding">
          <h3 class="fz1 fw-600 mb-3">Regular Items</h3>
          <div class="expandable-section-container mb-5">
            <div class="expandable-section-header" @click="addItem">
              <h3 class="expandable-section-title fz1 fw-600 mb-2 text-center">
                <b-icon icon="plus-circle" class="mr-2"></b-icon>
                Add Regular Item
              </h3>
              <p class="fz-1 color-gray mb-0 text-center">Click here to add a regular item to the pay stub</p>
            </div>
            <div class="expandable-section-list" v-if="items.length > 0">
              <div class="container-flex" v-for="(item, i) in items" :key="i">
                <div class="col-sm-6 col-md-3 col-lg-4 col-padding">
                  <b-field label="Description" expanded :type="errors.has('description' + i) ? 'is-danger' : ''"
                    :message="errors.has('description' + i) ? errors.first('description' + i) : ''">
                    <b-autocomplete :data="filteredDescriptions" v-model="item.description" :name="'description' + i"
                      :loading="isLoadingList" v-validate="'required'" autocomplete append-to-body open-on-focus>
                    </b-autocomplete>
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Quantity" expanded :type="errors.has('quantity' + i) ? 'is-danger' : ''"
                    :message="errors.has('quantity' + i) ? errors.first('quantity' + i) : ''">
                    <b-numberinput v-model="item.quantity" :min="1" :max="1000000" :step="1" :controls="false"
                      :name="'quantity' + i" v-validate="'required|min_value:1'" @input="updateItem(item)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Price" expanded :type="errors.has('unitPrice' + i) ? 'is-danger' : ''"
                    :message="errors.has('unitPrice' + i) ? errors.first('unitPrice' + i) : ''">
                    <b-numberinput v-model="item.unitPrice" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'unitPrice' + i" v-validate="'required|min_value:0.01'" @input="updateItem(item)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Total" expanded>
                    <b-input v-model="item.total" disabled></b-input>
                  </b-field>
                </div>
                <div class="col-sm-2 col-md-3 col-lg-2 col-padding">
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
<script>
import dayjs from 'dayjs';

export default {
  data() {
    const descriptions = [
      { value: 'Regular Hours' },
      { value: 'Overtime Hours' },
      { value: 'Missing Hours' },
      { value: 'Missing Overtime Hours' },
      { value: 'Holiday Premium Pay Hours' }
    ];
    return {
      isLoading: false,
      isLoadingList: false,
      payStub: {},
      workers: [],
      workerSelected: null,
      datesSelected: [],
      descriptions: descriptions,
      descriptionsSelected: [],
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
        this.getAllWorkers(text);
      } else {
        this.workers = [];
      }
    },
    onDatesSelected() {
      this.payStub.workBegins = dayjs(this.datesSelected[0]).format('YYYY-MM-DD');
      this.payStub.workEnd = dayjs(this.datesSelected[1]).format('YYYY-MM-DD');
    },
    getAllWorkers(text) {
      this.isLoadingList = true;
      this.$store.dispatch("agency/getAllWorkers", { searchTerm: text })
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
      if (this.items.length >= 8) {
        this.showAlertError('You can only add 8 items');
        return;
      }
      this.items.push({
        description: '',
        quantity: 1,
        unitPrice: 0,
        total: 0
      });
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
    async createPayStub() {
      const validation = await this.$validator.validateAll();
      if (validation) {
        this.isLoading = true;
        const regularHours = this.items.find(i => i.description === 'Regular Hours');
        const overtimeHours = this.items.find(i => i.description === 'Overtime Hours');
        const missingHours = this.items.find(i => i.description === 'Missing Hours');
        const missingOvertimeHours = this.items.find(i => i.description === 'Missing Overtime Hours');
        const holidayPremiumPayHours = this.items.find(i => i.description === 'Holiday Premium Pay Hours');
        if (regularHours) {
          this.payStub.regularHours = regularHours.quantity;
          this.payStub.unitPriceRegularHours = regularHours.unitPrice;
        }
        if (overtimeHours) {
          this.payStub.overtimeHours = overtimeHours.quantity;
          this.payStub.unitPriceOvertimeHours = overtimeHours.unitPrice;
        }
        if (missingHours) {
          this.payStub.missingHours = missingHours.quantity;
          this.payStub.unitPriceMissingHours = missingHours.unitPrice;
        }
        if (missingOvertimeHours) {
          this.payStub.missingOvertimeHours = missingOvertimeHours.quantity;
          this.payStub.unitPriceMissingOvertimeHours = missingOvertimeHours.unitPrice;
        }
        if (holidayPremiumPayHours) {
          this.payStub.holidayPremiumPayHours = holidayPremiumPayHours.quantity;
          this.payStub.unitPriceHolidayPremiumPayHours = holidayPremiumPayHours.unitPrice;
        }
        if (this.discount) {
          this.payStub.otherDeductions = this.discount.amount;
          this.payStub.otherDeductionsDescription = this.discount.description;
        }
        this.items.filter(i => !this.descriptions.some(d => d.value === i.description))
          .forEach((i, index) => {
            console.log(i);
            switch (index) {
              case 0:
                this.payStub.other = i.quantity;
                this.payStub.unitPriceOther = i.unitPrice;
                this.payStub.bonusOthersDescription = i.description;
                break;
              case 1:
                this.payStub.other2 = i.quantity;
                this.payStub.unitPriceOther2 = i.unitPrice;
                this.payStub.bonusOthersDescription2 = i.description;
                break;
              case 2:
                this.payStub.other3 = i.quantity;
                this.payStub.unitPriceOther3 = i.unitPrice;
                this.payStub.bonusOthersDescription3 = i.description;
                break;
            }
          });
        this.$store.dispatch("agency/createPayStub", this.payStub)
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
  computed: {
    filteredDescriptions() {
      return this.descriptions.filter(d => !this.items.map(i => i.description).includes(d.value));
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
</style>