<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="validateForm">
      <div class="col-12 col-padding">
        <div>
          <h2 class="main-title">
            {{ isUpdate ? `Update Candidate Request (${request.numberId})` : $t("CreateCandidateRequest") }}
          </h2>
          <span class="line-orange"></span>
        </div>
      </div>
      <div class="container-flex">
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field>
            <b-checkbox v-model="directHiring">Direct Hiring?</b-checkbox>
            <b-checkbox v-model="request.isAsap" :disabled="isUpdate">Is Asap?</b-checkbox>
            <b-checkbox v-model="sameBillingTitle" @input="onSameBillingChecked">Job title same for billing
              title?</b-checkbox>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('job title') ? 'is-danger' : ''" :label="$t('JobTitle')"
            :message="errors.has('job title') ? errors.first('job title') : ''">
            <b-input v-model="request.jobTitle" name="job title" v-validate="'required|max:100|min:1'"></b-input>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('billingTitle') ? 'is-danger' : ''" label="Billing Title"
            :message="errors.has('billingTitle') ? errors.first('billingTitle') : ''">
            <b-input v-model="request.billingTitle" name="billingTitle" v-validate="'required|max:100|min:1'"
              :disabled="sameBillingTitle"></b-input>
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
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="!directHiring">
          <b-field :type="errors.has('job type') ? 'is-danger' : ''" :label="$t('RequestJobType')"
            :message="errors.has('job type') ? errors.first('job type') : ''">
            <b-autocomplete :data="filteredCompanyJobPositions" placeholder="Role" v-model="jobPosition" field="value"
              open-on-focus name="job type" v-validate="'required'" selectable-footer @select="onJobPositionSelected"
              @select-footer="() => showRolesModal = true">
              <template #footer>
                <a>
                  <span v-if="isPayrollManager">Add new...</span>
                  <span v-else>Request new...</span>
                </a>
              </template>
              <template #empty>You don't have any roles created</template>
            </b-autocomplete>
          </b-field>
          <b-tag v-if="request.rate">Rate for this position: {{ request.rate }}</b-tag>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
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
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="isPayrollManager">
          <b-field label="Sales Representative">
            <b-autocomplete :data="filteredSalesRepresentative" placeholder="Sales Rep." v-model="salesRepresentative"
              open-on-focus :custom-formatter="(option) => `${option.name} - ${option.email}`"
              @select="onSalesRepresentativeSelected">
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="companyUsers.length > 0">
          <b-field label="Company User">
            <b-taginput v-model="companyUsersSelected" autocomplete :data="companyUsers" open-on-focus icon="label"
              ref="companyUserTagInput" :placeholder="$t('Select')">
              <template v-slot="props">
                {{ props.option.name }} {{ props.option.lastName }} - {{ props.option.email }}
              </template>
              <template #selected="props">
                <b-tag v-for="(tag, index) in props.tags" :key="index" tabstop ellipsis closable
                  @close="$refs.companyUserTagInput.removeTag(index, $event)">
                  {{ tag.name }} {{ tag.lastName }} - {{ tag.email }}
                </b-tag>
              </template>
            </b-taginput>
          </b-field>
        </div>
        <br />
        <div class="container-flex">
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field :label="$t('Description')" :message="errors.first('description')"
              :type="errors.has('description') ? 'is-danger' : ''">
              <div class="vue-trix-editor">
                <vue-editor id="description-input" v-model="request.description" :name="'description'"
                  v-validate="'required|max:5000|min:10'"></vue-editor>
              </div>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field label="Responsibilities">
              <div class="vue-trix-editor">
                <div>
                  <vue-editor id="responsibilities-input" v-model="request.responsibilities"></vue-editor>
                </div>
              </div>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field :label="$t('Requirements')" :message="errors.first('requirements')"
              :type="errors.has('requirements') ? 'is-danger' : ''">
              <div class="vue-trix-editor">
                <vue-editor id="requirements-input" v-model="request.requirements" :name="'requirements'"
                  v-validate="'required|max:5000|min:10'"></vue-editor>
              </div>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field label="Internal Requirements">
              <div class="vue-trix-editor">
                <div>
                  <vue-editor id="internalRequirements-input" v-model="request.internalRequirements"></vue-editor>
                </div>
              </div>
            </b-field>
          </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" disabled="!directHiring">
          <b-field :type="errors.has('incentive') ? 'is-danger' : ''" :label="$t('Incentive')"
            :message="errors.has('incentive') ? errors.first('incentive') : ''">
            <b-numberinput controls-alignment="right" v-model="request.incentive" name="incentive"
              v-validate="'decimal:2'" step="0.01" />
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
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="!directHiring">
          <b-field label="Duration Break">
            <b-timepicker v-model="request.durationBreak" :max-time="maxBreak" hour-format="24"
              :disabled="!request.breakIsPaid">
            </b-timepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding" v-if="!directHiring">
          <b-field :label="$t('PunchCardIsVisibleInApp')">
            <b-switch v-model="request.punchCardOptionEnabled" :true-value="true" :false-value="false">
              {{ request.punchCardOptionEnabled ? $t("Yes") : $t("No") }}
            </b-switch>
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
          <b-field label="Start" :message="errors.has('from') ? errors.first('from') : ''">
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
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" native-type="submit">
          {{ isUpdate ? 'Update' : $t("Create") }}
        </b-button>
      </div>
    </form>

    <b-modal v-model="showRolesModal" @close="showRolesModal = false" width="850px">
      <position-form v-if="isPayrollManager" :profile-id="companyProfileId"
        @updateContent="onUpdateRolesModal"></position-form>
      <request-position-form v-else :profile-id="companyProfileId" @closeModal="() => showRolesModal = false" />
    </b-modal>

    <b-modal v-model="showLocationModal" @close="showLocationModal = false" width="500px">
      <location-form :profile-id="companyProfileId" @updateContent="onUpdateLocationModal"></location-form>
    </b-modal>
  </div>
</template>

<script>
import dayjs from "dayjs";
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  components: {
    AddressComponent: () => import("@/components/Address"),
    PositionForm: () => import("@/components/agency_company/JobPositionForm"),
    RequestPositionForm: () => import("@/components/agency_company/RequestJobPositionForm"),
    LocationForm: () => import("@/components/agency_company/LocationForm"),
  },
  mixins: [billingAdminMixin],
  name: "AgencyCreateRequest",
  data() {
    const maxBreak = new Date();
    maxBreak.setHours(1);
    maxBreak.setMinutes(0);
    let timeZero = new Date(dayjs().subtract(14, "days"));
    timeZero.setHours(0);
    timeZero.setMinutes(0);
    return {
      maxBreak: maxBreak,
      timeZero: timeZero,
      isLoading: false,
      companyProfileId: this.$route.params.companyProfileId,
      companyJobPositions: [],
      jobPosition: '',
      locations: [],
      jobLocation: '',
      salesRepresentatives: [],
      salesRepresentative: '',
      companyUsers: [],
      companyUsersSelected: [],
      request: {
        durationBreak: dayjs().startOf('day').toDate(),
      },
      directHiring: false,
      sameBillingTitle: true,
      totalHours: 0,
      errorMessage: this.$t("PleaseVerifyThatTheFieldsAreCorrect"),
      showRolesModal: false,
      showLocationModal: false,
      isUpdate: false,
    };
  },
  methods: {
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
    onSalesRepresentativeSelected(option) {
      if (option) {
        this.request.salesRepresentativeId = option.id;
      } else {
        this.request.salesRepresentativeId = null;
      }
    },
    validateForm() {
      this.$validator.validateAll().then((result) => {
        if (result) {
          if (this.isUpdate) {
            this.updateRequest();
          } else {
            this.createRequest();
          }
          return;
        }
        this.showAlertError(this.errorMessage);
      });
    },
    createRequest() {
      this.isLoading = true;
      this.$store.dispatch("agency/postAgencyRequest", {
        ...this.request,
        durationBreak: dayjs(this.request.durationBreak).format("HH:mm"),
      })
        .then((response) => {
          this.showAlertSuccess(this.$t("RequestCreated"));
          this.$router.push("/agency-request/" + response.id);
          this.isLoading = false;
        })
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    async onUpdateRolesModal() {
      this.showRolesModal = false;
      this.companyJobPositions = await this.$store.dispatch("agency/getAgencyCompanyJobPositions", this.companyProfileId);
    },
    async onUpdateLocationModal() {
      this.showLocationModal = false;
      this.locations = await this.$store.dispatch("agency/getCompanyLocation", this.companyProfileId);
    },
    updateRequest() {
      this.isLoading = true;
      this.$store
        .dispatch("agency/updateAgencyRequest", { requestId: this.request.id, model: {
          ...this.request,
          durationBreak: dayjs(this.request.durationBreak).format("HH:mm"),
        } })
        .then((response) => {
          this.isLoading = false;
          this.showAlertSuccess("Request updated");
          this.$router.push("/agency-request/" + response.id);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    onSameBillingChecked(value) {
      if (!value) {
        this.request.billingTitle = "";
      }
    }
  },
  async created() {
    this.request.employmentType =
      this.request.durationTerm !== this.$longTerm
        ? this.$partTime
        : this.$fullTime;
    const agencyRequest = this.$route.meta.agencyRequest;
    this.request.companyProfileId = this.companyProfileId;
    if (agencyRequest) {
      this.companyJobPositions = this.$route.meta.companyJobPositions;
      this.locations = this.$route.meta.companyLocations;
      this.salesRepresentatives = this.$route.meta.agencyPersonnel;
      this.companyUsers = this.$route.meta.companyUsers;
      this.isUpdate = true;
      this.request = {
        ...agencyRequest,
        durationBreak: agencyRequest.breakIsPaid ? dayjs().add(agencyRequest.durationBreak, "hours").toDate() : dayjs().startOf('day').toDate(),
        jobPositionRateId: agencyRequest.jobPositionId,
        rate: agencyRequest.workerRate,
        startAt: new Date(agencyRequest.startAt),
        finishAt: new Date(agencyRequest.finishAt || ""),
      };
      this.directHiring = agencyRequest.workerSalary ? true : false;
      this.sameBillingTitle = agencyRequest.jobTitle === agencyRequest.billingTitle;
      this.companyUsersSelected = this.companyUsers.filter(cu => agencyRequest.companyUserIds.some(ar => cu.id == ar));
      let record = this.companyJobPositions.find(cjp => cjp.id === agencyRequest.jobPositionId);
      if (record) {
        this.jobPosition = record.value
      }
      record = this.locations.find(l => l.address === agencyRequest.jobLocation.address);
      if (record) {
        this.request.locationId = record.id;
      }
      record = this.locations.find(l => l.id === this.request.locationId);
      if (record) {
        this.jobLocation = record.formattedAddress;
      }
      record = this.salesRepresentatives.find((sr) => sr.id === agencyRequest.salesRepresentativeId);
      if (record) {
        this.salesRepresentative = `${record.name} - ${record.email}`;
      }
    } else {
      this.companyJobPositions = await this.$store.dispatch("agency/getAgencyCompanyJobPositions", this.companyProfileId);
      this.locations = await this.$store.dispatch("agency/getCompanyLocation", this.companyProfileId);
      this.salesRepresentatives = await this.$store.dispatch("agency/getAgencyPersonnel")
      this.companyUsers = await this.$store.dispatch("agency/getCompanyUsers", this.companyProfileId);
    }
    this.isLoading = false;
  },
  computed: {
    finishDate() {
      return dayjs(this.request.startAt).add(1, "year").toDate();
    },
    filteredCompanyJobPositions() {
      const jobPositions = this.companyJobPositions
        .filter(cjp => cjp.value.toLowerCase().includes(this.jobPosition.toLowerCase()));
      return jobPositions;
    },
    filteredLocations() {
      const locations = this.locations
        .filter(l => l.formattedAddress.toLowerCase().includes(this.jobLocation.toLowerCase()));
      return locations;
    },
    filteredSalesRepresentative() {
      const salesRepresentatives = this.salesRepresentatives
        .filter(sr => `${sr.name} - ${sr.email}`.toLowerCase().includes(this.salesRepresentative.toLowerCase()));
      return salesRepresentatives;
    }
  },
  watch: {
    directHiring: function (val) {
      if (val) {
        this.$validator.detach("job type");
      } else {
        this.request.workerSalary = null;
      }
    },
    sameBillingTitle: function (val) {
      if (val) {
        this.request.billingTitle = this.request.jobTitle;
      }
    },
    companyUser: function (val) {
      if (val) {
        this.request.companyUserId = val.id;
      }
    },
    companyUsersSelected: function (val) {
      this.request.companyUserIds = val.map(cus => cus.id);
    },
    "request.jobTitle": function (val) {
      if (this.sameBillingTitle) {
        this.request.billingTitle = val;
      }
    }
  },
};
</script>
<style>
#description,
#requirements,
#internalRequirements {
  height: 250px;
}
</style>
