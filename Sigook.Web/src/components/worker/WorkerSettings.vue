<template>
  <div class="mt-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <b-checkbox class="col-2" v-model="worker.isContractor" @input="updateIsContractor">
        Is Contractor
      </b-checkbox>
      <b-checkbox class="col-2" v-model="worker.isSubcontractor" @input="updateIsSubContractor">
        Is Subcontractor
      </b-checkbox>
      <span class="line-gray"></span>
      <div class="col-2">
        <b-field label="Federal Category">
          <b-select v-model="worker.federalTaxCategory" @input="updateTaxCategory" expanded>
            <option :value="null">Select</option>
            <option v-for="taxCategory in taxCategories" :key="taxCategory.id" :value="taxCategory.id">
              {{ taxCategory.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="Provincial Category" class="mr-5">
          <b-select v-model="worker.provincialTaxCategory" @input="updateTaxCategory" expanded>
            <option :value="null">Select</option>
            <option v-for="taxCategory in taxCategories" :key="taxCategory.id" :value="taxCategory.id">
              {{ taxCategory.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="CPP" :type="errors.has('cpp') ? 'is-danger' : ''"
          :message="errors.has('cpp') ? errors.first('cpp') : ''">
          <b-numberinput v-model="localCpp" name="cpp" :step="0.01" :controls="false" expanded
            v-validate="'min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="EI" :type="errors.has('ei') ? 'is-danger' : ''"
          :message="errors.has('ei') ? errors.first('ei') : ''">
          <b-numberinput v-model="localEi" name="ei" :step="0.01" :controls="false" expanded
            v-validate="'min_value:0'">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label=" ">
          <b-button type="is-primary" @click="updateTaxRate" :loading="isLoading">Save</b-button>
        </b-field>
      </div>
      <span class="line-gray"></span>
      <div class="col-12">
        <b-button type="is-ghost" icon-left="plus" @click="addHoliday">Add Holiday</b-button>
        <b-field grouped>
          <b-field label="Holidays">
            <b-datepicker inline :selectable-dates="selectableDates" @input="onHolidaySelected"
              :unselectable-days-of-week="[0, 1, 2, 3, 4, 5, 6]">
            </b-datepicker>
          </b-field>
          <b-field v-if="workerHolidaySelected" label="Amount to pay">
            <b-input v-model="workerHolidaySelected.statPaidWorker"></b-input>
            <b-button type="is-primary is-light" @click="addUpdateWorkerHoliday">Save</b-button>
          </b-field>
        </b-field>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  props: ['worker'],
  data() {
    return {
      isLoading: false,
      taxCategories: [],
      workerHolidays: [],
      workerHolidaySelected: null,
      localCpp: null,
      localEi: null
    }
  },
  methods: {
    updateIsContractor() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyWorkerContractor", this.worker.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateIsSubContractor() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyWorkerSubContractor", this.worker.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateTaxCategory() {
      this.isLoading = true;
      this.$store.dispatch('agency/updateWorkerProfileTaxCategory', this.worker)
        .then(() => {
          this.isLoading = false;
        }).catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    updateTaxRate() {
      // Actualizar el worker con los valores locales, convirtiendo vacíos a null
      this.worker.cpp = this.localCpp === '' || this.localCpp === undefined ? null : this.localCpp;
      this.worker.ei = this.localEi === '' || this.localEi === undefined ? null : this.localEi;

      this.isLoading = true;
      this.$store.dispatch('agency/updateWorkerProfileTaxRate', this.worker)
        .then(() => {
          this.isLoading = false;
        }).catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    async addHoliday() {
      this.$buefy.dialog.prompt({
        message: `City`,
        inputAttrs: {
          type: 'date',
          placeholder: 'Date'
        },
        closeOnConfirm: false,
        confirmText: 'Add',
        onConfirm: async (value, dialog) => {
          await this.$store.dispatch('agency/addNewHoliday', { workerProfileId: this.worker.id, date: value });
          this.workerHolidays = await this.$store.dispatch('agency/getAgencyWorkerProfileHolidays', this.worker.id);
          dialog.close();
        }
      })
    },
    onHolidaySelected(date) {
      this.workerHolidaySelected = this.workerHolidays.find(wh => new Date(wh.date).getDate() === date.getDate());
    },
    async addUpdateWorkerHoliday() {
      this.isLoading = true;
      await this.$store.dispatch('agency/addUpdateAgencyWorkerProfileHolidays', {
        workerProfileId: this.worker.id,
        data: this.workerHolidaySelected
      });
      this.isLoading = false;
    }
  },
  async created() {
    this.taxCategories = await this.$store.dispatch('getTaxCategories');
    this.workerHolidays = await this.$store.dispatch('agency/getAgencyWorkerProfileHolidays', this.worker.id);
    // Inicializar valores locales
    this.localCpp = this.worker.cpp;
    this.localEi = this.worker.ei;
  },
  watch: {
    'worker.cpp'(newVal) {
      this.localCpp = newVal;
    },
    'worker.ei'(newVal) {
      this.localEi = newVal;
    }
  },
  computed: {
    selectableDates() {
      const holidays = this.workerHolidays.map(wh => new Date(wh.date));
      return holidays;
    }
  }
}

</script>