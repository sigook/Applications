<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="onSubmit">
      <div class="col-12 col-padding">
        <div>
          <h2 class="main-title">
            {{ isUpdate ? `Update Candidate Request (${request.numberId})` : "Create Candidate Request" }}
          </h2>
          <span class="line-orange"></span>
        </div>
      </div>
      <div class="container-flex">
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field>
            <b-checkbox v-model="directHiring">Direct Hiring?</b-checkbox>
            <b-checkbox v-model="request.isAsap" :disabled="isUpdate">Is Asap?</b-checkbox>
            <b-checkbox v-model="sameBillingTitle" @update:modelValue="onSameBillingChecked">Job title same for billing
              title?</b-checkbox>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.jobTitle ? 'is-danger' : ''" :label="`${'Job title'} *`"
            :message="errors.jobTitle || ''">
            <b-input v-model="jobTitle" name="job title"></b-input>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.billingTitle ? 'is-danger' : ''" label="Billing Title *"
            :message="errors.billingTitle || ''">
            <b-input v-model="billingTitle" name="billingTitle" :disabled="sameBillingTitle"></b-input>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="directHiring">
          <b-field :type="errors.workerSalary ? 'is-danger' : ''" label="Worker Salary *"
            :message="errors.workerSalary || ''">
            <b-numberinput v-model="workerSalary" name="workerSalary" controls-alignment="right"></b-numberinput>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.workersQuantity ? 'is-danger' : ''" :label="`${'Workers Quantity'} *`"
            :message="errors.workersQuantity || ''">
            <b-numberinput v-model="workersQuantity" name="worker quantity" controls-alignment="right" expanded></b-numberinput>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding" v-if="!directHiring">
          <b-field :type="errors.jobPosition ? 'is-danger' : ''" :label="`${'Job type'} *`"
            :message="errors.jobPosition || ''">
            <b-autocomplete :data="filteredCompanyJobPositions" placeholder="Role" v-model="jobPosition" field="value"
              open-on-focus name="job type" selectable-footer @select="onJobPositionSelected"
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
          <b-field :label="`${'Branch office'} *`" :message="errors.branchOffice || ''"
            :type="errors.branchOffice ? 'is-danger' : ''">
            <b-autocomplete :data="filteredLocations" placeholder="Location" v-model="branchOffice" open-on-focus
              name="branchOffice" selectable-footer field="formattedAddress" @select="onLocationSelected"
              @select-footer="() => showLocationModal = true">
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
              ref="companyUserTagInput" :placeholder="'Select'">
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
            <b-field :label="`${'Description'} *`" :message="errors.description || ''"
              :type="errors.description ? 'is-danger' : ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="description" />
              </div>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field label="Responsibilities">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="request.responsibilities" />
              </div>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field :label="`${'Requirements'} *`" :message="errors.requirements || ''"
              :type="errors.requirements ? 'is-danger' : ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="requirements" />
              </div>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-12 col-lg-6 col-padding">
            <b-field label="Internal Requirements">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="request.internalRequirements" />
              </div>
            </b-field>
          </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" disabled="!directHiring">
          <b-field :type="errors.incentive ? 'is-danger' : ''" :label="'Incentive'"
            :message="errors.incentive || ''">
            <b-numberinput controls-alignment="right" v-model="incentive" name="incentive" step="0.01" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-9 col-padding">
          <b-field :type="errors.incentiveDescription ? 'is-danger' : ''" :label="'Incentive Description'"
            :message="errors.incentiveDescription || ''">
            <b-input v-model="incentiveDescription" name="incentiveDes" :disabled="!incentive" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="!directHiring">
          <b-field :label="'Duration break is paid'">
            <b-switch v-model="request.breakIsPaid" :true-value="true" :false-value="false">
              {{ request.breakIsPaid ? "Yes" : "No" }}
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
          <b-field :label="'Punch card is visible in app'">
            <b-switch v-model="request.punchCardOptionEnabled" :true-value="true" :false-value="false">
              {{ request.punchCardOptionEnabled ? "Yes" : "No" }}
            </b-switch>
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
          <b-field label="Start *" :type="errors.startAt ? 'is-danger' : ''"
            :message="errors.startAt || ''">
            <b-datepicker v-model="startAt" name="from" :min-date="timeZero">
            </b-datepicker>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-3 col-padding" v-if="request.durationTerm === DurationTerm.ShortTerm">
          <b-field label="Finish">
            <b-datepicker v-model="request.finishAt" name="from" :min-date="startAt" :max-date="finishDate">
            </b-datepicker>
          </b-field>
        </div>
      </div>
      <div class="col-12 mt-5">
        <b-button type="is-primary" native-type="submit">
          {{ isUpdate ? 'Update' : "Create" }}
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

<script lang="ts">
import { defineAsyncComponent, ref, computed } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import dayjs from "dayjs";
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { getAgencyPersonnel } from "@/api/agencyApi";
import { getAgencyCompanyJobPositions, getAgencyCompanyLocation, getCompanyUsers } from "@/api/agencyCompanyApi";
import { postAgencyRequest, updateAgencyRequest } from "@/api/agencyRequestApi";
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentType,
  EmploymentTypeLabels
} from "@/constants/enums";

export default {
  setup() {
    const directHiring = ref(false);

    const validationSchema = computed(() => {
      const shape: Record<string, any> = {
        jobTitle: yup.string().required('Job title is required').min(1).max(100, 'Max 100 characters'),
        billingTitle: yup.string().required('Billing title is required').min(1).max(100, 'Max 100 characters'),
        workersQuantity: yup
          .number()
          .typeError('Workers quantity is required')
          .required('Workers quantity is required')
          .min(1, 'Must be at least 1'),
        branchOffice: yup.string().required('Branch office is required'),
        description: yup.string().required('Description is required').min(10, 'Min 10 characters').max(5000, 'Max 5000 characters'),
        requirements: yup.string().required('Requirements are required').min(10, 'Min 10 characters').max(5000, 'Max 5000 characters'),
        incentive: yup
          .number()
          .nullable()
          .transform((v, o) => (o === '' || o === null || o === undefined ? null : v))
          .test('decimal2', 'Max 2 decimals', (v) => v == null || /^-?\d+(\.\d{1,2})?$/.test(String(v))),
        incentiveDescription: yup.string().nullable().max(5000, 'Max 5000 characters'),
        startAt: yup.mixed().required('Start date is required'),
      };
      if (directHiring.value) {
        shape.workerSalary = yup
          .number()
          .typeError('Worker salary is required')
          .required('Worker salary is required');
      } else {
        shape.jobPosition = yup.string().required('Job type is required');
      }
      return yup.object(shape);
    });

    const form = useStickyForm({
      schema: validationSchema,
      initialValues: {
        jobTitle: '',
        billingTitle: '',
        workersQuantity: 1,
        workerSalary: null as number | null,
        jobPosition: '',
        branchOffice: '',
        description: '',
        requirements: '',
        incentive: null as number | null,
        incentiveDescription: '',
        startAt: null as Date | null,
      },
    });

    return {
      ...useBillingAdmin(),
      directHiring,
      ...form.fields,
      errors: form.errors,
      handleSubmit: form.handleSubmit,
      setFieldValue: form.setFieldValue,
      setFieldError: form.setFieldError,
      resetForm: form.hydrate,
      markInteracted: form.markInteracted,
    };
  },
  components: {
    PositionForm: defineAsyncComponent(() => import("@/components/agency_company/JobPositionForm.vue")),
    RequestPositionForm: defineAsyncComponent(() => import("@/components/agency_company/RequestJobPositionForm.vue")),
    LocationForm: defineAsyncComponent(() => import("@/components/agency_company/LocationForm.vue"))
  },
  name: "AgencyCreateRequest",
  data() {
    const maxBreak = new Date();
    maxBreak.setHours(1);
    maxBreak.setMinutes(0);
    let timeZero = dayjs().subtract(14, "days").toDate();
    timeZero.setHours(0);
    timeZero.setMinutes(0);
    return {
      maxBreak: maxBreak,
      timeZero: timeZero,
      isLoading: false,
      companyProfileId: this.$route.params.companyProfileId,
      companyJobPositions: [],
      jobPositionSelected: null,
      locations: [],
      locationSelected: null,
      salesRepresentatives: [],
      salesRepresentative: '',
      salesRepresentativeSelected: null,
      companyUsers: [],
      companyUsersSelected: [],
      request: {
        durationBreak: dayjs().startOf('day').toDate(),
        durationTerm: DurationTerm.LongTerm,
        employmentType: EmploymentType.FullTime
      },
      sameBillingTitle: true,
      totalHours: 0,
      errorMessage: "Please make sure all required fields are filled out correctly",
      showRolesModal: false,
      showLocationModal: false,
      isUpdate: false
    };
  },
  methods: {
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
    onSalesRepresentativeSelected(option) {
      this.salesRepresentativeSelected = option;
      if (option) {
        this.request.salesRepresentativeId = option.id;
      } else {
        this.request.salesRepresentativeId = null;
      }
    },
    validateAutocompleteSelections() {
      let valid = true;

      if (!this.directHiring && this.jobPosition && (!this.jobPositionSelected || this.jobPositionSelected.value.toLowerCase() !== this.jobPosition.toLowerCase())) {
        this.setFieldError('jobPosition', 'Please select a role from the list');
        valid = false;
      }

      if (this.branchOffice && (!this.locationSelected || this.locationSelected.formattedAddress.toLowerCase() !== this.branchOffice.toLowerCase())) {
        this.setFieldError('branchOffice', 'Please select a location from the list');
        valid = false;
      }

      return valid;
    },
    onSubmit() {
      this.markInteracted([
        'jobTitle', 'billingTitle', 'workersQuantity', 'workerSalary',
        'jobPosition', 'branchOffice', 'description', 'requirements',
        'incentive', 'incentiveDescription', 'startAt'
      ]);
      this.handleSubmit((values) => {
        if (!this.validateAutocompleteSelections()) {
          showAlertError(this.errorMessage);
          return;
        }
        const payload = {
          ...this.request,
          jobTitle: values.jobTitle,
          billingTitle: values.billingTitle,
          workersQuantity: values.workersQuantity,
          workerSalary: this.directHiring ? values.workerSalary : null,
          description: values.description,
          requirements: values.requirements,
          incentive: values.incentive,
          incentiveDescription: values.incentiveDescription,
          startAt: values.startAt,
          durationBreak: dayjs(this.request.durationBreak).format("HH:mm"),
        };
        if (this.isUpdate) {
          this.updateRequest(payload);
        } else {
          this.createRequest(payload);
        }
      }, () => {
        showAlertError(this.errorMessage);
      })();
    },
    createRequest(payload) {
      this.isLoading = true;
      postAgencyRequest(payload)
        .then((response) => {
          showAlertSuccess("Request created");
          this.$router.push("/agency-request/" + response.id);
          this.isLoading = false;
        })
        .catch((error) => {
          showAlertError(error);
          this.isLoading = false;
        });
    },
    async onUpdateRolesModal() {
      this.showRolesModal = false;
      this.companyJobPositions = await getAgencyCompanyJobPositions(this.companyProfileId);
    },
    async onUpdateLocationModal() {
      this.showLocationModal = false;
      this.locations = await getAgencyCompanyLocation(this.companyProfileId);
    },
    updateRequest(payload) {
      this.isLoading = true;
      updateAgencyRequest(this.request.id, payload)
        .then((response) => {
          this.isLoading = false;
          showAlertSuccess("Request updated");
          this.$router.push("/agency-request/" + response.id);
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    },
    onSameBillingChecked(value) {
      if (value) {
        this.setFieldValue('billingTitle', this.jobTitle);
      } else {
        this.setFieldValue('billingTitle', '');
      }
    }
  },
  async created() {
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
        finishAt: new Date(agencyRequest.finishAt || "")
      };
      this.directHiring = agencyRequest.workerSalary ? true : false;
      this.sameBillingTitle = agencyRequest.jobTitle === agencyRequest.billingTitle;
      this.companyUsersSelected = this.companyUsers.filter(cu => agencyRequest.companyUserIds.some(ar => cu.id == ar));

      let record = this.companyJobPositions.find(cjp => cjp.id === agencyRequest.jobPositionId);
      if (record) {
        this.setFieldValue('jobPosition', record.value);
        this.jobPositionSelected = record;
      }
      record = this.locations.find(l => l.address === agencyRequest.jobLocation.address);
      if (record) {
        this.request.locationId = record.id;
      }
      record = this.locations.find(l => l.id === this.request.locationId);
      if (record) {
        this.setFieldValue('branchOffice', record.formattedAddress);
        this.locationSelected = record;
      }
      record = this.salesRepresentatives.find((sr) => sr.id === agencyRequest.salesRepresentativeId);
      if (record) {
        this.salesRepresentative = `${record.name} - ${record.email}`;
        this.salesRepresentativeSelected = record;
      }

      this.resetForm({
        jobTitle: agencyRequest.jobTitle,
        billingTitle: agencyRequest.billingTitle,
        workersQuantity: agencyRequest.workersQuantity,
        workerSalary: agencyRequest.workerSalary,
        jobPosition: this.jobPosition,
        branchOffice: this.branchOffice,
        description: agencyRequest.description,
        requirements: agencyRequest.requirements,
        incentive: agencyRequest.incentive,
        incentiveDescription: agencyRequest.incentiveDescription,
        startAt: new Date(agencyRequest.startAt),
      });
    } else {
      this.companyJobPositions = await getAgencyCompanyJobPositions(this.companyProfileId);
      this.locations = await getAgencyCompanyLocation(this.companyProfileId);
      this.salesRepresentatives = await getAgencyPersonnel();
      this.companyUsers = await getCompanyUsers(this.companyProfileId);
    }
    this.isLoading = false;
  },
  computed: {
    DurationTerm: () => DurationTerm,
    DurationTermLabels: () => DurationTermLabels,
    EmploymentType: () => EmploymentType,
    EmploymentTypeLabels: () => EmploymentTypeLabels,
    finishDate() {
      return dayjs(this.startAt).add(1, "year").toDate();
    },
    filteredCompanyJobPositions() {
      const search = (this.jobPosition || '').toLowerCase();
      return this.companyJobPositions.filter(cjp => cjp.value.toLowerCase().includes(search));
    },
    filteredLocations() {
      const search = (this.branchOffice || '').toLowerCase();
      return this.locations.filter(l => l.formattedAddress.toLowerCase().includes(search));
    },
    filteredSalesRepresentative() {
      return this.salesRepresentatives
        .filter(sr => `${sr.name} - ${sr.email}`.toLowerCase().includes(this.salesRepresentative.toLowerCase()));
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
    branchOffice(newVal) {
      if (this.locationSelected && this.locationSelected.formattedAddress !== newVal) {
        this.locationSelected = null;
        this.request.locationId = null;
      }
    },
    salesRepresentative(newVal) {
      if (this.salesRepresentativeSelected) {
        const formatted = `${this.salesRepresentativeSelected.name} - ${this.salesRepresentativeSelected.email}`;
        if (formatted !== newVal) {
          this.salesRepresentativeSelected = null;
          this.request.salesRepresentativeId = null;
        }
      }
    },
    directHiring(val) {
      if (!val) {
        this.setFieldValue('workerSalary', null);
      }
    },
    sameBillingTitle(val) {
      if (val) {
        this.setFieldValue('billingTitle', this.jobTitle);
      }
    },
    companyUsersSelected(val) {
      this.request.companyUserIds = val.map(cus => cus.id);
    },
    jobTitle(val) {
      if (this.sameBillingTitle) {
        this.setFieldValue('billingTitle', val);
      }
    }
  }
};
</script>
