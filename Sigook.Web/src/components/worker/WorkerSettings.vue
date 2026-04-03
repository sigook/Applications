<template>
  <div class="mt-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-2">
        <b-field label="External ID">
          <b-input v-model="localWorker.externalId" placeholder="External ID" @keypress.enter.native="updateExternalId">
          </b-input>
        </b-field>
      </div>
      <b-checkbox class="col-2" v-model="localWorker.isContractor" @input="updateIsContractor">
        Is Contractor
      </b-checkbox>
      <b-checkbox class="col-2" v-model="localWorker.isSubcontractor" @input="updateIsSubContractor">
        Is Subcontractor
      </b-checkbox>
      <span class="line-gray"></span>
      <div class="col-2">
        <b-field label="Federal Category">
          <b-select v-model="localWorker.federalTaxCategory" @input="updateTaxCategory" expanded>
            <option :value="null">Select</option>
            <option v-for="taxCategory in taxCategories" :key="taxCategory.id" :value="taxCategory.id">
              {{ taxCategory.value }}
            </option>
          </b-select>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="Provincial Category" class="mr-5">
          <b-select v-model="localWorker.provincialTaxCategory" @input="updateTaxCategory" expanded>
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
          <b-numberinput v-model="localWorker.cpp" name="cpp" :step="0.01" :controls="false" expanded
            v-validate="'min_value:0'" @keypress.enter.native="updateTaxRate">
          </b-numberinput>
        </b-field>
      </div>
      <div class="col-2">
        <b-field label="EI" :type="errors.has('ei') ? 'is-danger' : ''"
          :message="errors.has('ei') ? errors.first('ei') : ''">
          <b-numberinput v-model="localWorker.ei" name="ei" :step="0.01" :controls="false" expanded
            v-validate="'min_value:0'" @keypress.enter.native="updateTaxRate">
          </b-numberinput>
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
<script lang="ts">
import { getTaxCategories } from "@/api/catalogApi";

export default {
  props: ['worker'],
  data() {
    return {
      isLoading: false,
      taxCategories: [],
      workerHolidays: [],
      workerHolidaySelected: null,
      localWorker: JSON.parse(JSON.stringify(this.worker))
    }
  },
  watch: {
    worker: {
      handler(newVal) {
        this.localWorker = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  methods: {
    updateExternalId() {
      this.isLoading = true;
      this.$store.dispatch('agency/updateWorkerProfileExternalId', this.localWorker)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        }).catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateIsContractor() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyWorkerContractor", this.localWorker.id)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateIsSubContractor() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyWorkerSubContractor", this.localWorker.id)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateTaxCategory() {
      this.isLoading = true;
      this.$store.dispatch('agency/updateWorkerProfileTaxCategory', this.localWorker)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
        }).catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    updateTaxRate() {
      this.isLoading = true;
      this.$store.dispatch('agency/updateWorkerProfileTaxRate', this.localWorker)
        .then(() => {
          this.isLoading = false;
          this.$emit('update:worker', this.localWorker);
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
          await this.$store.dispatch('agency/addNewHoliday', { workerProfileId: this.localWorker.id, date: value });
          this.workerHolidays = await this.$store.dispatch('agency/getAgencyWorkerProfileHolidays', this.localWorker.id);
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
        workerProfileId: this.localWorker.id,
        data: this.workerHolidaySelected
      });
      this.isLoading = false;
    }
  },
  async created() {
    this.taxCategories = await getTaxCategories();
    this.workerHolidays = await this.$store.dispatch('agency/getAgencyWorkerProfileHolidays', this.localWorker.id);
  },
  computed: {
    selectableDates() {
      const holidays = this.workerHolidays.map(wh => new Date(wh.date));
      return holidays;
    }
  }
}

</script>