<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="validateForm">
      <div class="col-12 col-padding">
        <div>
          <h2 class="main-title">{{ $t("CreateCandidateRequest") }}</h2>
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
          <b-field :label="$t('JobTitle')" :message="errors.first('job title')"
            :type="errors.has('job title') ? 'is-danger' : ''">
            <b-input v-model="request.jobTitle" name="job title" v-validate="'required'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="directHiring">
          <b-field :type="errors.has('workerSalary') ? 'is-danger' : ''" label="Worker Salary"
            :message="errors.has('workerSalary') ? errors.first('workerSalary') : ''">
            <b-numberinput v-model="request.workerSalary" name="workerSalary" v-validate="'required'"
              controls-alignment="right"></b-numberinput>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('worker quantity') ? 'is-danger' : ''" :label="$t('WorkersQuantity')"
            :message="errors.has('worker quantity') ? errors.first('worker quantity') : ''">
            <b-numberinput v-model="request.workersQuantity" name="worker quantity"
              v-validate="'required|min_value:1|numeric'" controls-alignment="right" expanded></b-numberinput>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!directHiring">
          <b-field :label="$t('RequestJobType')" :message="errors.first('job type')"
            :type="errors.has('job type') ? 'is-danger' : ''">
            <b-autocomplete :data="filteredCompanyJobPositions" placeholder="Role" v-model="jobPosition" field="value"
              open-on-focus name="job type" v-validate="'required'" @select="onJobPositionSelected">
              <template #empty>You don't have any roles created</template>
            </b-autocomplete>
          </b-field>
          <b-tag v-if="request.rate">Rate for this position: {{ request.rate }}</b-tag>
        </div>
        <div :class="[directHiring ? 'col-12 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding']">
          <b-field :label="$t('RequestBranchOffice')"
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
          <b-field :label="$t('Description')" :message="errors.first('description')"
            :type="errors.has('description') ? 'is-danger' : ''">
            <div class="vue-trix-editor">
              <vue-editor id="description-input" v-model="request.description" :name="'description'"
                v-validate="'required|max:50000|min:25'" />
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
          <b-field :label="$t('Requirements')" :message="errors.first('requirements')"
            :type="errors.has('requirements') ? 'is-danger' : ''">
            <div class="vue-trix-editor">
              <vue-editor id="requirements-input" v-model="request.requirements" :name="'requirements'"
                v-validate="'required|max:50000|min:25'" />
            </div>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" disabled="!directHiring">
          <b-field :type="errors.has('incentive') ? 'is-danger' : ''" :label="$t('Incentive')"
            :message="errors.has('incentive') ? errors.first('incentive') : ''">
            <b-numberinput controls-alignment="right" v-model="request.incentive" name="incentive"
              v-validate="'required|decimal:2'" step="0.01" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-9 col-padding">
          <b-field :type="errors.has('incentiveDes') ? 'is-danger' : ''" :label="$t('IncentiveDescription')"
            :message="errors.has('incentiveDes') ? errors.first('incentiveDes') : ''">
            <b-input v-model="request.incentiveDescription" name="incentiveDes" v-validate="'max:5000|min:0'"
              :disabled="!request.incentive" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="!directHiring">
          <b-field :label="$t('DurationBreakIsPaid')">
            <b-switch v-model="request.breakIsPaid" :true-value="true" :false-value="false">
              {{ request.breakIsPaid ? $t("Yes") : $t("No") }}
            </b-switch>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-9 col-padding" v-if="!directHiring">
          <b-field label="Duration Break">
            <b-timepicker v-model="request.durationBreakFormat" :max-time="maxBreak" hour-format="24"
              :disabled="!request.breakIsPaid">
            </b-timepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Duration Term">
            <b-select v-model="request.durationTerm" expanded>
              <option :value="this.$longTerm">
                {{ this.$longTerm | splitCapital }}
              </option>
              <option :value="this.$shortTerm">
                {{ this.$shortTerm | splitCapital }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Employment Type">
            <b-select v-model="request.employmentType" expanded>
              <option :value="this.$fullTime">
                {{ this.$fullTime | splitCapital }}
              </option>
              <option :value="this.$partTime">
                {{ this.$partTime | splitCapital }}
              </option>
              <option :value="this.$contractor">
                {{ this.$contractor | splitCapital }}
              </option>
              <option :value="this.$temporary">
                {{ this.$temporary | splitCapital }}
              </option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
          <b-field label="Start" :type="errors.has('from') ? 'is-danger' : ''"
            :message="errors.has('from') ? errors.first('from') : ''">
            <b-datepicker v-model="request.startAt" name="from" :min-date="timeZero" v-validate="'required'">
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="request.durationTerm === this.$shortTerm">
          <b-field label="Finish">
            <b-datepicker v-model="request.finishAt" name="from" :min-date="request.startAt" :max-date="finishDate">
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button type="is-primary" native-type="submit">
            {{ $t("Create") }}
          </b-button>
        </div>
      </div>
    </form>

    <b-modal v-model="showLocationModal" @close="showLocationModal = false" width="500px">
      <location-form @updateContent="onUpdateLocationModal"></location-form>
    </b-modal>
  </div>
</template>

<script>
import dayjs from "dayjs";
import confirmationAlert from "@/mixins/confirmationAlert";

export default {
  components: {
    LocationForm: () => import("@/components/agency_company/LocationForm"),
  },
  data() {
    let breakDate = new Date();
    breakDate.setHours(0);
    breakDate.setMinutes(0);

    const maxBreak = new Date();
    maxBreak.setHours(1);
    maxBreak.setMinutes(0);

    let timeZero = new Date(dayjs().subtract(14, "days"));
    timeZero.setHours(0);
    timeZero.setMinutes(0);
    return {
      isLoading: true,
      locations: [],
      companyJobPositions: [],
      timeZero: timeZero,
      maxBreak: maxBreak,
      modalValidation: false,
      contact: {},
      request: {},
      errorMessage: this.$t("PleaseVerifyThatTheFieldsAreCorrect"),
      directHiring: false,
      jobPosition: '',
      jobLocation: '',
      showLocationModal: false,
    };
  },
  mixins: [confirmationAlert],
  async created() {
    this.locations = await this.$store.dispatch("company/getLocations");
    this.companyJobPositions = await this.$store.dispatch("company/getCompanyJobPositions");
    this.isLoading = false;
  },
  methods: {
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          this.createRequest();
          return;
        }
        this.showAlertError(this.errorMessage);
      });
    },
    createRequest() {
      this.isLoading = true;
      this.request.durationBreak = this.directHiring
        ? `0:00:00`
        : `${this.request.durationBreakFormat.getHours()}:${this.request.durationBreakFormat.getMinutes()}:00`;
      this.$store.dispatch("company/createRequest", this.request)
        .then(() => {
          this.showAlertSuccess(this.$t("RequestCreated"));
          this.$router.push("company-requests");
          this.isLoading = false;
        })
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    sendMessage() {
      this.$validator.validate("message").then((result) => {
        if (result) {
          this.isLoading = true;
          this.$store.dispatch("company/requestNewPosition", this.contact)
            .then(() => {
              this.isLoading = false;
              this.showAlertSuccess(this.$t("Sent"));
              this.modalValidation = false;
            })
            .catch((error) => {
              this.isLoading = false;
              this.showAlertError(error);
              this.modalValidation = false;
            });
        }
      });
    },
    getCompanyJobPositionById(id) {
      this.isLoading = true;
      this.request.shift = null;
      this.$store.dispatch("company/getCompanyJobPositionById", id)
        .then((response) => {
          this.request.shift = response.shift || {};
          this.request.rate = response.workerRate;
          this.isLoading = false;
        })
    },
    onJobPositionSelected(option) {
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
      if (option) {
        this.request.locationId = option.id;
      } else {
        this.request.locationId = null;
      }
    },
    async onUpdateLocationModal() {
      this.showLocationModal = false;
      this.locations = await this.$store.dispatch("company/getLocations");
    },
  },
  computed: {
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
    },
  },
  watch: {
    directHiring: function (val) {
      if (val) {
        this.$validator.detach("job type");
      } else {
        this.request.workerSalary = null;
      }
    },
  },
};
</script>
