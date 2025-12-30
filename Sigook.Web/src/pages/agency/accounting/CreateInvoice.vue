<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <div class="section-top-title container-flex mb-2">
      <h2 class="fz1 pt-3 col-6 col-md-5 col-sm-7">
        Create Invoice
      </h2>
    </div>
    <div>
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-field label="Direct Hiring?">
            <b-switch v-model="invoice.directHiring">
              {{ invoice.directHiring ? 'Yes' : 'No' }}
            </b-switch>
          </b-field>
        </div>

        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field label="Company" :type="errors.has('company') ? 'is-danger' : ''"
            :message="errors.has('company') ? errors.first('company') : ''">
            <b-autocomplete v-model="companySelected" :data="filteredCompanies" open-on-focus v-validate="'required'"
              field="fullName" name="company" placeholder="Company" @select="selectCompany">
            </b-autocomplete>
          </b-field>
        </div>

        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field label="Province/State">
            <b-select v-model="invoice.provinceId" name="province" expanded placeholder="Select Province/State"
              :disabled="!enabledProvince">
              <option v-for="province in provinces" :key="province.id" :value="province.id">
                {{ province.value }}
              </option>
            </b-select>
          </b-field>
        </div>

        <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
          <b-field label="Email (optional)" :type="errors.has('email') ? 'is-danger' : ''"
            :message="errors.has('email') ? errors.first('email') : 'Fill this input if the company needs a different email for this specific invoice'">
            <b-input type="email" v-model="invoice.email" name="email" v-validate="'email'" />
          </b-field>
        </div>

        <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
          <b-field label="Date (optional)"
            :message="'Fill this input if the company needs a different date for this specific invoice'">
            <b-datepicker v-model="invoice.invoiceDate" />
          </b-field>
        </div>

        <div class="col-sm-12 col-md-4 col-lg-4 col-padding">
          <b-field label="Dates (From - To) (optional)"
            :message="'Optional: Dates to filter timesheets. Leave empty to include all timesheets from the beginning.'">
            <b-datepicker v-model="dateSelected" placeholder="Select start date for filtering" range
              @input="onDatesSelected" />
          </b-field>
        </div>
      </div>

      <!-- Additional Items Section -->
      <div class="container-flex">
        <div class="col-12 col-padding">
          <hr class="my-4">
          <div class="expandable-section-container">
            <div class="expandable-section-header" @click="addItem">
              <h3 class="expandable-section-title fz1 fw-600 mb-2 text-center">
                <b-icon icon="plus-circle" class="mr-2"></b-icon>
                Add Additional Items
              </h3>
              <p class="fz-1 color-gray mb-0 text-center">Click here to add extra items or charges not included in
                regular timesheet billing</p>
            </div>

            <div class="expandable-section-list" v-if="additionalItems.length > 0">
              <div class="container-flex" v-for="(item, i) in additionalItems" :key="i">
                <div class="col-sm-6 col-md-3 col-lg-4 col-padding">
                  <b-field label="Description" expanded :type="errors.has('description' + i) ? 'is-danger' : ''"
                    :message="errors.has('description' + i) ? errors.first('description' + i) : ''">
                    <b-input v-model="item.description" v-validate="'required'" :name="'description' + i" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Quantity" expanded :type="errors.has('quantity' + i) ? 'is-danger' : ''"
                    :message="errors.has('quantity' + i) ? errors.first('quantity' + i) : ''">
                    <b-numberinput v-model="item.quantity" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'quantity' + i" v-validate="'required|min_value:0.01'"
                      @input="updateTotalAdditionalItems(item)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Price" expanded :type="errors.has('unitPrice' + i) ? 'is-danger' : ''"
                    :message="errors.has('unitPrice' + i) ? errors.first('unitPrice' + i) : ''">
                    <b-numberinput v-model="item.unitPrice" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'unitPrice' + i" v-validate="'required|min_value:0.01'"
                      @input="updateTotalAdditionalItems(item)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Total" expanded :type="errors.has('total' + i) ? 'is-danger' : ''"
                    :message="errors.has('total' + i) ? errors.first('total' + i) : ''">
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
      </div>

      <!-- Discounts Section -->
      <div class="container-flex">
        <div class="col-12 col-padding">
          <hr class="my-4">
          <div class="expandable-section-container">
            <div class="expandable-section-header" @click="addDiscount">
              <h3 class="expandable-section-title fz1 fw-600 mb-2 text-center">
                <b-icon icon="minus-circle" class="mr-2"></b-icon>
                Add Discounts
              </h3>
              <p class="fz-1 color-gray mb-0 text-center">Click here to add discounts or reductions to apply to this
                invoice</p>
            </div>

            <div class="expandable-section-list" v-if="discounts.length > 0">
              <div class="container-flex" v-for="(discount, i) in discounts" :key="i">
                <div class="col-sm-6 col-md-3 col-lg-4 col-padding">
                  <b-field label="Description" expanded :type="errors.has('discountDescription' + i) ? 'is-danger' : ''"
                    :message="errors.has('discountDescription' + i) ? errors.first('discountDescription' + i) : ''">
                    <b-input v-model="discount.description" v-validate="'required'" :name="'discountDescription' + i" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Quantity" expanded :type="errors.has('quantity' + i) ? 'is-danger' : ''"
                    :message="errors.has('quantity' + i) ? errors.first('quantity' + i) : ''">
                    <b-numberinput v-model="discount.quantity" :min="0.01" :max="1000000" :step="0.01" :controls="false"
                      :name="'quantity' + i" v-validate="'required|min_value:0.01'"
                      @input="updateTotalDiscounts(discount)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Price" expanded :type="errors.has('unitPrice' + i) ? 'is-danger' : ''"
                    :message="errors.has('unitPrice' + i) ? errors.first('unitPrice' + i) : ''">
                    <b-numberinput v-model="discount.unitPrice" :min="0.01" :max="1000000" :step="0.01"
                      :controls="false" :name="'unitPrice' + i" v-validate="'required|min_value:0.01'"
                      @input="updateTotalDiscounts(discount)" />
                  </b-field>
                </div>
                <div class="col-sm-6 col-md-3 col-lg-2 col-padding">
                  <b-field label="Total" expanded>
                    <b-input v-model="discount.total" disabled></b-input>
                  </b-field>
                </div>
                <div class="col-sm-2 col-md-3 col-lg-2 col-padding">
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

      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="loadPreview">
          Preview Invoice
        </b-button>
      </div>

      <!-- Preview Section -->
      <div v-if="previewData" class="col-12 mt-4">
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

<script>
import dayjs from 'dayjs';

export default {
  name: "CreateInvoice",
  components: {
    PreviewInvoice: () => import("@/components/agency_accounting/PreviewInvoice.vue")
  },
  data() {
    return {
      isLoading: false,
      invoice: {},
      companies: [],
      companySelected: '',
      provinces: [],
      additionalItems: [],
      discounts: [],
      previewData: null,
      dateSelected: []
    };
  },
  methods: {
    loadPreview() {
      this.validateForm();
    },
    async onDatesSelected() {
      this.invoice.from = dayjs(this.dateSelected[0]).format('YYYY-MM-DD');
      this.invoice.to = dayjs(this.dateSelected[1]).format('YYYY-MM-DD');
    },
    async getCompanies() {
      this.companies = await this.$store.dispatch('agency/getAgencyCompanyProfileWithRequests');
    },
    async selectCompany(company) {
      if (company) {
        this.invoice.companyId = company.companyId;
        this.invoice.companyProfileId = company.id;
        this.provinces = await this.$store.dispatch('agency/getCompanyProvinceWithTaxes', this.invoice.companyProfileId);
      } else {
        this.invoice = {};
        this.provinces = [];
      }
    },
    updateTotalAdditionalItems(item) {
      item.total = item.quantity * item.unitPrice;
    },
    updateTotalDiscounts(discount) {
      discount.total = discount.quantity * discount.unitPrice;
    },
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.isLoading = true;
          this.invoice.additionalItems = this.additionalItems;
          this.invoice.discounts = this.discounts;
          this.$store.dispatch('agency/previewInvoice', this.invoice)
            .then((response) => {
              this.isLoading = false;
              this.previewData = response;

              // Scroll to preview section
              this.$nextTick(() => {
                const previewElement = this.$el.querySelector('.box');
                if (previewElement) {
                  previewElement.scrollIntoView({ behavior: 'smooth' });
                }
              });
            }).catch((error) => {
              this.isLoading = false;
              this.showAlertError(error);
            });
        }
      });
    },
    addItem() {
      this.additionalItems.push({
        quantity: 0,
        unitPrice: 0,
        total: 0,
        description: ''
      });
    },
    removeItem(item) {
      this.additionalItems = this.additionalItems.filter(i => i !== item);
    },
    addDiscount() {
      this.discounts.push({});
    },
    removeDiscount(discount) {
      this.discounts = this.discounts.filter(d => d !== discount);
    },
    confirmInvoice() {
      this.isLoading = true;
      this.$store.dispatch('agency/createInvoice', this.invoice)
        .then(() => {
          this.isLoading = false;
          this.$router.push('/accounting/invoices');
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    cancelPreview() {
      this.previewData = null;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    },
  },
  computed: {
    filteredCompanies() {
      return this.companies.filter(company => company.fullName.toLowerCase().includes(this.companySelected.toLowerCase()));
    },
    enabledProvince() {
      return this.companySelected && this.provinces.length > 0;
    }
  },
  async created() {
    await this.getCompanies();
  }
};
</script>
