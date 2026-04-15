<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="validateForm">
      <div class="col-12 col-padding">
        <div>
          <h2 class="main-title">{{ "Create Candidate Request" }}</h2>
          <span class="line-orange"></span>
        </div>
      </div>
      <div class="container-flex">
        <div class="col-12 col-padding">
          <b-field>
            <b-checkbox v-model="directHiring">Direct Hiring</b-checkbox>
            <b-checkbox v-model="request.isAsap">Is Asap?</b-checkbox>
          </b-field>
        </div>
        <div
          :class="[directHiring ? 'col-sm-12 col-md-6 col-lg-4 col-padding' : 'col-sm-12 col-md-8 col-lg-8 col-padding']">
          <b-field :label="`${'Job title'} *`" :message="errors.first('job title')"
            :type="errors.has('job title') ? 'is-danger' : ''">
            <b-input v-model="request.jobTitle" name="job title" v-validate="'required|max:100|min:1'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="directHiring">
          <b-field :type="errors.has('workerSalary') ? 'is-danger' : ''" label="Worker Salary *"
            :message="errors.has('workerSalary') ? errors.first('workerSalary') : ''">
            <b-numberinput v-model="request.workerSalary" name="workerSalary" v-validate="'required'"
              controls-alignment="right"></b-numberinput>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('worker quantity') ? 'is-danger' : ''" :label="`${'Workers Quantity'} *`"
            :message="errors.has('worker quantity') ? errors.first('worker quantity') : ''">
            <b-numberinput v-model="request.workersQuantity" name="worker quantity"
              v-validate="'required|min_value:1|numeric'" controls-alignment="right" expanded></b-numberinput>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!directHiring">
          <b-field :label="`${'Job type'} *`" :message="errors.first('job type')"
            :type="errors.has('job type') ? 'is-danger' : ''">
            <b-autocomplete :data="filteredCompanyJobPositions" placeholder="Role" v-model="jobPosition" field="value"
              open-on-focus name="job type" v-validate="'required'" @select="onJobPositionSelected">
              <template #empty>You don't have any roles created</template>
            </b-autocomplete>
          </b-field>
          <b-tag v-if="request.rate">Rate for this position: {{ request.rate }}</b-tag>
        </div>
        <div :class="[directHiring ? 'col-12 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding']">
          <b-field :label="`${'Branch office'} *`"
            :message="errors.has('branchOffice') ? errors.first('branchOffice') : ''"
            :type="errors.has('branchOffice') ? 'is-danger' : ''">
            <b-autocomplete :data="filteredLocations" placeholder="Location" v-model="jobLocation" open-on-focus
              name="branchOffice" v-validate="'required'" selectable-footer field="formattedAddress"
              @select="onLocationSelected" @select-footer="() => showLocationModal = true">
              <template #footer>
                <a><span> Add new... </span></a>
              </template>
              <template #empty>You don't have any location created</template>
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-4 col-padding">
          <b-field :label="`${'Description'} *`" :message="errors.first('description')"
            :type="errors.has('description') ? 'is-danger' : ''">
            <div class="vue-trix-editor">
              <vue-editor id="description-input" v-model="request.description" :name="'description'"
                v-validate="'required|max:5000|min:10'" />
            </div>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-4 col-padding">
          <b-field label="Responsibilities">
            <div class="vue-trix-editor">
              <vue-editor id="responsibilities-input" v-model="request.responsibilities" />
            </div>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-4 col-padding">
          <b-field :label="`${'Requirements'} *`" :message="errors.first('requirements')"
            :type="errors.has('requirements') ? 'is-danger' : ''">
            <div class="vue-trix-editor">
              <vue-editor id="requirements-input" v-model="request.requirements" :name="'requirements'"
                v-validate="'required|max:5000|min:10'" />
            </div>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" disabled="!directHiring">
          <b-field :type="errors.has('incentive') ? 'is-danger' : ''" :label="'Incentive'"
            :message="errors.has('incentive') ? errors.first('incentive') : ''">
            <b-numberinput controls-alignment="right" v-model="request.incentive" name="incentive"
              v-validate="'decimal:2'" step="0.01" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-9 col-padding">
          <b-field :type="errors.has('incentiveDes') ? 'is-danger' : ''" :label="'Incentive Description'"
            :message="errors.has('incentiveDes') ? errors.first('incentiveDes') : ''">
            <b-input v-model="request.incentiveDescription" name="incentiveDes" v-validate="'max:5000|min:0'"
              :disabled="!request.incentive" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="!directHiring">
          <b-field :label="'Duration break is paid'">
            <b-switch v-model="request.breakIsPaid" :true-value="true" :false-value="false">
              {{ request.breakIsPaid ? "Yes" : "No" }}
            </b-switch>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-9 col-padding" v-if="!directHiring">
          <b-field label="Duration Break">
            <b-timepicker v-model="request.durationBreak" :max-time="maxBreak" hour-format="24"
              :disabled="!request.breakIsPaid">
            </b-timepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Duration Term">
            <b-select v-model="request.durationTerm" expanded>
              <option :value="DurationTerm.LongTerm">
                {{ DurationTermLabels[DurationTerm.LongTerm] }}
              </option>
              <option :value="DurationTerm.ShortTerm">
                {{ DurationTermLabels[DurationTerm.ShortTerm] }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Employment Type">
            <b-select v-model="request.employmentType" expanded>
              <option :value="EmploymentType.FullTime">
                {{ EmploymentTypeLabels[EmploymentType.FullTime] }}
              </option>
              <option :value="EmploymentType.PartTime">
                {{ EmploymentTypeLabels[EmploymentType.PartTime] }}
              </option>
              <option :value="EmploymentType.Contractor">
                {{ EmploymentTypeLabels[EmploymentType.Contractor] }}
              </option>
              <option :value="EmploymentType.Temporary">
                {{ EmploymentTypeLabels[EmploymentType.Temporary] }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Start *" :type="errors.has('from') ? 'is-danger' : ''"
            :message="errors.has('from') ? errors.first('from') : ''">
            <b-datepicker v-model="request.startAt" name="from" :min-date="timeZero" v-validate="'required'">
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="request.durationTerm === DurationTerm.ShortTerm">
          <b-field label="Finish">
            <b-datepicker v-model="request.finishAt" name="from" :min-date="request.startAt" :max-date="finishDate">
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button type="is-primary" native-type="submit">
            {{ "Create" }}
          </b-button>
        </div>
      </div>
    </form>

    <b-modal v-model="showLocationModal" @close="showLocationModal = false" width="500px">
      <location-form @updateContent="onUpdateLocationModal"></location-form>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import dayjs from "dayjs";
import { confirmationGuard } from '@/utils/confirmationGuard';
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentType,
  EmploymentTypeLabels
} from "@/constants/enums";
import { getLocations, getCompanyJobPositions, createRequest } from "@/api/companyApi";

export default {
  components: {
    LocationForm: defineAsyncComponent(() => import("@/components/agency_company/LocationForm.vue"))
  },
  data() {
    let breakDate = new Date();
    breakDate.setHours(0);
    breakDate.setMinutes(0);

    const maxBreak = new Date();
    maxBreak.setHours(1);
    maxBreak.setMinutes(0);

    let timeZero = dayjs().subtract(14, "days").toDate();
    timeZero.setHours(0);
    timeZero.setMinutes(0);
    return {
      isLoading: true,
      locations: [],
      companyJobPositions: [],
      timeZero: timeZero,
      maxBreak: maxBreak,
      request: {
        durationBreak: dayjs().startOf('day').toDate(),
        durationTerm: DurationTerm.LongTerm,
        employmentType: EmploymentType.FullTime
      },
      errorMessage: "Please make sure all required fields are filled out correctly",
      directHiring: false,
      jobPosition: '',
      jobPositionSelected: null,
      jobLocation: '',
      locationSelected: null,
      showLocationModal: false,
      unsavedChanges: false
    };
  },
  beforeRouteLeave: confirmationGuard,
  async created() {
    this.locations = await getLocations();
    this.companyJobPositions = await getCompanyJobPositions();
    this.isLoading = false;
  },
  methods: {
    validateAutocompleteSelections() {
      let valid = true;

      if (!this.directHiring && this.jobPosition && (!this.jobPositionSelected || this.jobPositionSelected.value.toLowerCase() !== this.jobPosition.toLowerCase())) {
        this.errors.add({ field: 'job type', msg: 'Please select a role from the list' });
        valid = false;
      }

      if (this.jobLocation && (!this.locationSelected || this.locationSelected.formattedAddress.toLowerCase() !== this.jobLocation.toLowerCase())) {
        this.errors.add({ field: 'branchOffice', msg: 'Please select a location from the list' });
        valid = false;
      }

      return valid;
    },
    validateForm() {
      this.$validator.validateAll().then((result) => {
        const selectionsValid = this.validateAutocompleteSelections();
        if (result && selectionsValid) {
          this.submitRequest();
          return;
        }
        showAlertError(this.errorMessage);
      });
    },
    submitRequest() {
      this.isLoading = true;
      createRequest({
        ...this.request,
        durationBreak: dayjs(this.request.durationBreak).format("HH:mm")
      })
        .then(() => {
          showAlertSuccess("Request created");
          this.$router.push("company-requests");
          this.isLoading = false;
        })
        .catch((error) => {
          showAlertError(error);
          this.isLoading = false;
        });
    },
    onJobPositionSelected(option) {
      this.jobPositionSelected = option;
      if (option) {
        this.request.shift = option.shift;
        this.request.rate = option.workerRate;
        this.request.jobPositionRateId = option.id;
      } else {
        this.request.shift = null;
        this.request.rate = null
        this.request.jobPositionRateId = null
      }
    },
    onLocationSelected(option) {
      this.locationSelected = option;
      if (option) {
        this.request.locationId = option.id;
      } else {
        this.request.locationId = null;
      }
    },
    async onUpdateLocationModal() {
      this.showLocationModal = false;
      this.locations = await getLocations();
    }
  },
  computed: {
    DurationTerm: () => DurationTerm,
    DurationTermLabels: () => DurationTermLabels,
    EmploymentType: () => EmploymentType,
    EmploymentTypeLabels: () => EmploymentTypeLabels,
    filteredCompanyJobPositions() {
      const jobPositions = this.companyJobPositions
        .filter(cjp => cjp.value.toLowerCase().includes(this.jobPosition.toLowerCase()));
      return jobPositions;
    },
    filteredLocations() {
      const locations = this.locations
        .filter(location => location.formattedAddress.toLowerCase().includes(this.jobLocation.toLowerCase()));
      return locations;
    },
    finishDate() {
      return dayjs(this.request.startAt).add(1, "year").toDate();
    }
  },
  watch: {
    jobPosition(newVal) {
      if (this.jobPositionSelected && this.jobPositionSelected.value !== newVal) {
        this.jobPositionSelected = null;
        this.request.shift = null;
        this.request.rate = null;
        this.request.jobPositionRateId = null;
      }
    },
    jobLocation(newVal) {
      if (this.locationSelected && this.locationSelected.formattedAddress !== newVal) {
        this.locationSelected = null;
        this.request.locationId = null;
      }
    },
    directHiring: function (val) {
      if (val) {
        this.$validator.detach("job type");
      } else {
        this.request.workerSalary = null;
      }
    }
  }
};
</script>
