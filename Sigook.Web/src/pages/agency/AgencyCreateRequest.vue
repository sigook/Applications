<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="onSubmit">
      <div class="">
        <div>
          <h2 class="main-title">
            {{ isUpdate ? `Update Candidate Request (${request.numberId})` : "Create Candidate Request" }}
          </h2>
          <span class="line-orange"></span>
        </div>
      </div>
      <div class="columns is-multiline">
        <div class="column is-12">
          <b-field>
            <b-checkbox v-model="directHiring">Direct Hiring?</b-checkbox>
            <b-checkbox v-model="request.isAsap" :disabled="isUpdate">Is Asap?</b-checkbox>
            <b-checkbox v-model="request.usesRunners">Uses Runners?</b-checkbox>
            <b-checkbox v-model="sameBillingTitle" @update:modelValue="onSameBillingChecked">Job title same for billing
              title?</b-checkbox>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop">
          <b-field :type="errors.jobTitle ? 'is-danger' : ''" :label="`${'Job title'} *`"
            :message="errors.jobTitle || ''">
            <b-input v-model="jobTitle" name="job title"></b-input>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop">
          <b-field :type="errors.billingTitle ? 'is-danger' : ''" label="Billing Title *"
            :message="errors.billingTitle || ''">
            <b-input v-model="billingTitle" name="billingTitle" :disabled="sameBillingTitle"></b-input>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop" v-if="isAdmin">
          <b-field :type="errors.jobCosting ? 'is-danger' : ''" label="Job Costing"
            :message="errors.jobCosting || ''">
            <b-input v-model="jobCosting" name="jobCosting"></b-input>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop" v-if="directHiring">
          <b-field :type="errors.workerSalary ? 'is-danger' : ''" label="Worker Salary *"
            :message="errors.workerSalary || ''">
            <b-numberinput v-model="workerSalary" name="workerSalary" controls-alignment="right"></b-numberinput>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop">
          <b-field :type="errors.workersQuantity ? 'is-danger' : ''" :label="`${'Workers Quantity'} *`"
            :message="errors.workersQuantity || ''">
            <b-numberinput v-model="workersQuantity" name="worker quantity" controls-alignment="right" expanded></b-numberinput>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop" v-if="!directHiring">
          <b-field :type="errors.jobPosition ? 'is-danger' : ''" :label="`${'Rol'} *`"
            :message="errors.jobPosition || ''">
            <b-autocomplete :data="filteredCompanyJobPositions" placeholder="Role" v-model="jobPosition" field="jobPosition"
              open-on-focus name="rol" selectable-footer @select="onJobPositionSelected"
              @select-footer="() => showRolesModal = true">
              <template #footer>
                <a>
                  <span v-if="isAdmin">Add new...</span>
                  <span v-else>Request new...</span>
                </a>
              </template>
              <template #empty>You don't have any roles created</template>
            </b-autocomplete>
          </b-field>
          <b-tag v-if="request.rate">Rate for this position: {{ request.rate }}</b-tag>
        </div>
        <div class="column is-6 is-4-desktop">
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
        <div class="column is-6 is-4-desktop" v-if="isAdmin">
          <b-field label="Sales Representative">
            <b-autocomplete :data="filteredSalesRepresentative" placeholder="Sales Rep." v-model="salesRepresentative"
              open-on-focus :custom-formatter="(option) => `${option.name} - ${option.email}`"
              @select="onSalesRepresentativeSelected">
            </b-autocomplete>
          </b-field>
        </div>
        <div class="column is-6 is-4-desktop" v-if="companyUsers.length > 0">
          <b-field label="Company User">
            <b-taginput v-model="companyUsersSelected" autocomplete :data="companyUsers" open-on-focus icon="label"
              ref="companyUserTagInput" :placeholder="'Select'">
              <template v-slot="props">
                {{ props.option.name }} {{ props.option.lastName }} - {{ props.option.email }}
              </template>
              <template #selected="props">
                <b-tag v-for="(tag, index) in props.tags" :key="index" tabstop ellipsis closable
                  @close="companyUserTagInput?.removeTag(index, $event)">
                  {{ tag.name }} {{ tag.lastName }} - {{ tag.email }}
                </b-tag>
              </template>
            </b-taginput>
          </b-field>
        </div>
        <br />
        <div class="column is-12">
          <div class="columns is-multiline">
            <div class="column is-12 is-6-desktop">
              <b-field :label="`${'Description'} *`" :message="errors.description || ''"
              :type="errors.description ? 'is-danger' : ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="description" />
              </div>
            </b-field>
          </div>
          <div class="column is-12 is-6-desktop">
            <b-field label="Responsibilities">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="request.responsibilities" />
              </div>
            </b-field>
          </div>
          <div class="column is-12 is-6-desktop">
            <b-field :label="`${'Requirements'} *`" :message="errors.requirements || ''"
              :type="errors.requirements ? 'is-danger' : ''">
              <div class="vue-trix-editor">
                <QuillEditor theme="snow" content-type="html" v-model:content="requirements" />
              </div>
            </b-field>
          </div>
            <div class="column is-12 is-6-desktop">
              <b-field label="Internal Requirements">
                <div class="vue-trix-editor">
                  <QuillEditor theme="snow" content-type="html" v-model:content="request.internalRequirements" />
                </div>
              </b-field>
            </div>
          </div>
        </div>
        <div class="column is-6 is-3-desktop" disabled="!directHiring">
          <b-field :type="errors.incentive ? 'is-danger' : ''" :label="'Incentive'"
            :message="errors.incentive || ''">
            <b-numberinput controls-alignment="right" v-model="incentive" name="incentive" step="0.01" />
          </b-field>
        </div>
        <div class="column is-6 is-9-desktop">
          <b-field :type="errors.incentiveDescription ? 'is-danger' : ''" :label="'Incentive Description'"
            :message="errors.incentiveDescription || ''">
            <b-input v-model="incentiveDescription" name="incentiveDes" :disabled="!incentive" />
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop" v-if="!directHiring">
          <b-field :label="'Duration break is paid'">
            <b-switch v-model="request.breakIsPaid" :true-value="true" :false-value="false">
              {{ request.breakIsPaid ? "Yes" : "No" }}
            </b-switch>
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop" v-if="!directHiring">
          <b-field label="Duration Break">
            <b-timepicker v-model="request.durationBreak" :max-time="maxBreak" hour-format="24"
              :disabled="!request.breakIsPaid">
            </b-timepicker>
          </b-field>
        </div>
        <div class="column is-6" v-if="!directHiring">
          <b-field :label="'Punch card is visible in app'">
            <b-switch v-model="request.punchCardOptionEnabled" :true-value="true" :false-value="false">
              {{ request.punchCardOptionEnabled ? "Yes" : "No" }}
            </b-switch>
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop">
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
        <div class="column is-6 is-3-desktop">
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
        <div class="column is-6 is-3-desktop">
          <b-field label="Start *" :type="errors.startAt ? 'is-danger' : ''"
            :message="errors.startAt || ''">
            <b-datepicker v-model="startAt" name="from" :min-date="timeZero">
            </b-datepicker>
          </b-field>
        </div>
        <div class="column is-6 is-3-desktop" v-if="request.durationTerm === DurationTerm.ShortTerm">
          <b-field label="Finish">
            <b-datepicker v-model="request.finishAt" name="from" :min-date="startAt" :max-date="finishDate">
            </b-datepicker>
          </b-field>
        </div>
      </div>
      <div class="mt-5">
        <b-button type="is-primary" native-type="submit">
          {{ isUpdate ? 'Update' : "Create" }}
        </b-button>
      </div>
    </form>

    <b-modal custom-content-class="card" v-model="showRolesModal" @close="showRolesModal = false" width="850px">
      <position-form v-if="isAdmin" :profile-id="companyProfileId"
        @updateContent="onUpdateRolesModal"></position-form>
      <request-position-form v-else :profile-id="companyProfileId" @closeModal="() => showRolesModal = false" />
    </b-modal>

    <b-modal custom-content-class="card" v-model="showLocationModal" @close="showLocationModal = false" width="500px">
      <location-form :profile-id="companyProfileId" @updateContent="onUpdateLocationModal"></location-form>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import * as yup from 'yup';
import dayjs from 'dayjs';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import { useAdmin } from '@/composables/useAdmin';
import { useModuleBase } from '@/composables/useModuleBase';
import { getAgencyPersonnel } from '@/api/agencyApi';
import { getAgencyCompanyJobPositions, getAgencyCompanyLocation, getCompanyUsers } from '@/api/agencyCompanyApi';
import { postAgencyRequest, updateAgencyRequest } from '@/api/agencyRequestApi';
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentType,
  EmploymentTypeLabels,
} from '@/constants/enums';
import PositionForm from '@/components/agency_company/JobPositionForm.vue';
import RequestPositionForm from '@/components/agency_company/RequestJobPositionForm.vue';
import LocationForm from '@/components/agency_company/LocationForm.vue';

const route = useRoute();
const router = useRouter();
const { requestBase } = useModuleBase();
const { isAdmin } = useAdmin();

const directHiring = ref(false);

const validationSchema = computed(() => {
  const shape: Record<string, any> = {
    jobTitle: yup.string().required('Job title is required').min(1).max(100, 'Max 100 characters'),
    billingTitle: yup.string().required('Billing title is required').min(1).max(100, 'Max 100 characters'),
    jobCosting: yup.string().nullable().max(100, 'Max 100 characters'),
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
    shape.jobPosition = yup.string().required('Rol is required');
  }
  return yup.object(shape);
});

const form = useStickyForm({
  schema: validationSchema,
  initialValues: {
    jobTitle: '',
    billingTitle: '',
    jobCosting: '',
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

const {
  jobTitle, billingTitle, jobCosting, workersQuantity, workerSalary, jobPosition,
  branchOffice, description, requirements, incentive, incentiveDescription, startAt,
} = form.fields;
const errors = form.errors;

const maxBreak = new Date();
maxBreak.setHours(1);
maxBreak.setMinutes(0);
const timeZero = dayjs().subtract(14, 'days').toDate();
timeZero.setHours(0);
timeZero.setMinutes(0);

const isLoading = ref(false);
const companyProfileId = ref<any>(route.params.companyProfileId);
const companyJobPositions = ref<any[]>([]);
const jobPositionSelected = ref<any>(null);
const locations = ref<any[]>([]);
const locationSelected = ref<any>(null);
const salesRepresentatives = ref<any[]>([]);
const salesRepresentative = ref('');
const salesRepresentativeSelected = ref<any>(null);
const companyUsers = ref<any[]>([]);
const companyUsersSelected = ref<any[]>([]);
const request = ref<any>({
  durationBreak: dayjs().startOf('day').toDate(),
  durationTerm: DurationTerm.LongTerm,
  employmentType: EmploymentType.FullTime,
  usesRunners: true,
});
const sameBillingTitle = ref(true);
const errorMessage = 'Please make sure all required fields are filled out correctly';
const showRolesModal = ref(false);
const showLocationModal = ref(false);
const isUpdate = ref(false);
const companyUserTagInput = ref<any>(null);

const finishDate = computed(() => dayjs(startAt.value).add(1, 'year').toDate());

const filteredCompanyJobPositions = computed(() => {
  const search = (jobPosition.value || '').toLowerCase();
  return companyJobPositions.value.filter((cjp) => (cjp.jobPosition || '').toLowerCase().includes(search));
});

const filteredLocations = computed(() => {
  const search = (branchOffice.value || '').toLowerCase();
  return locations.value.filter((l) => l.formattedAddress.toLowerCase().includes(search));
});

const filteredSalesRepresentative = computed(() =>
  salesRepresentatives.value
    .filter((sr) => `${sr.name} - ${sr.email}`.toLowerCase().includes(salesRepresentative.value.toLowerCase())),
);

watch(jobPosition, (newVal) => {
  if (jobPositionSelected.value && jobPositionSelected.value.jobPosition !== newVal) {
    jobPositionSelected.value = null;
    request.value.shift = null;
    request.value.rate = null;
    request.value.jobPositionRateId = null;
  }
});

watch(branchOffice, (newVal) => {
  if (locationSelected.value && locationSelected.value.formattedAddress !== newVal) {
    locationSelected.value = null;
    request.value.locationId = null;
  }
});

watch(salesRepresentative, (newVal) => {
  if (salesRepresentativeSelected.value) {
    const formatted = `${salesRepresentativeSelected.value.name} - ${salesRepresentativeSelected.value.email}`;
    if (formatted !== newVal) {
      salesRepresentativeSelected.value = null;
      request.value.salesRepresentativeId = null;
    }
  }
});

watch(directHiring, (val) => {
  if (!val) {
    form.setFieldValue('workerSalary', null);
  }
});

watch(sameBillingTitle, (val) => {
  if (val) {
    form.setFieldValue('billingTitle', jobTitle.value);
  }
});

watch(companyUsersSelected, (val) => {
  request.value.companyUserIds = val.map((cus: any) => cus.id);
});

watch(jobTitle, (val) => {
  if (sameBillingTitle.value) {
    form.setFieldValue('billingTitle', val);
  }
});

(async () => {
  const agencyRequest = (route.meta as any).agencyRequest;
  request.value.companyProfileId = companyProfileId.value;
  if (agencyRequest) {
    companyJobPositions.value = route.meta.companyJobPositions as unknown[];
    locations.value = route.meta.companyLocations as unknown[];
    salesRepresentatives.value = route.meta.agencyPersonnel as unknown[];
    companyUsers.value = route.meta.companyUsers as unknown[];
    isUpdate.value = true;
    request.value = {
      ...agencyRequest,
      durationBreak: agencyRequest.breakIsPaid ? dayjs(`${dayjs().format('YYYY-MM-DD')}T${agencyRequest.durationBreak}`).toDate() : dayjs().startOf('day').toDate(),
      jobPositionRateId: agencyRequest.jobPositionId,
      rate: agencyRequest.workerRate,
      finishAt: agencyRequest.finishAt ? new Date(agencyRequest.finishAt) : null,
    };
    directHiring.value = agencyRequest.workerSalary ? true : false;
    sameBillingTitle.value = agencyRequest.jobTitle === agencyRequest.billingTitle;
    companyUsersSelected.value = companyUsers.value.filter((cu) => agencyRequest.companyUserIds.some((ar: any) => cu.id == ar));

    let record: any = companyJobPositions.value.find((cjp) => cjp.id === agencyRequest.jobPositionId);
    if (record) {
      form.setFieldValue('jobPosition', record.jobPosition);
      jobPositionSelected.value = record;
    }
    record = locations.value.find((l) => l.address === agencyRequest.jobLocation.address);
    if (record) {
      request.value.locationId = record.id;
    }
    record = locations.value.find((l) => l.id === request.value.locationId);
    if (record) {
      form.setFieldValue('branchOffice', record.formattedAddress);
      locationSelected.value = record;
    }
    record = salesRepresentatives.value.find((sr) => sr.id === agencyRequest.salesRepresentativeId);
    if (record) {
      salesRepresentative.value = `${record.name} - ${record.email}`;
      salesRepresentativeSelected.value = record;
    }

    form.hydrate({
      jobTitle: agencyRequest.jobTitle,
      billingTitle: agencyRequest.billingTitle,
      jobCosting: agencyRequest.jobCosting,
      workersQuantity: agencyRequest.workersQuantity,
      workerSalary: agencyRequest.workerSalary,
      jobPosition: jobPosition.value,
      branchOffice: branchOffice.value,
      description: agencyRequest.description,
      requirements: agencyRequest.requirements,
      incentive: agencyRequest.incentive,
      incentiveDescription: agencyRequest.incentiveDescription,
      startAt: new Date(agencyRequest.startAt),
    });
  } else {
    companyJobPositions.value = await getAgencyCompanyJobPositions(companyProfileId.value);
    locations.value = await getAgencyCompanyLocation(companyProfileId.value);
    salesRepresentatives.value = await getAgencyPersonnel();
    companyUsers.value = await getCompanyUsers(companyProfileId.value);
  }
  isLoading.value = false;
})();

function onJobPositionSelected(option: any) {
  jobPositionSelected.value = option;
  if (option) {
    request.value.shift = option.shift;
    request.value.rate = option.workerRate;
    request.value.jobPositionRateId = option.id;
  } else {
    request.value.shift = null;
    request.value.rate = null;
    request.value.jobPositionRateId = null;
  }
}

function onLocationSelected(option: any) {
  locationSelected.value = option;
  if (option) {
    request.value.locationId = option.id;
  } else {
    request.value.locationId = null;
  }
}

function onSalesRepresentativeSelected(option: any) {
  salesRepresentativeSelected.value = option;
  if (option) {
    request.value.salesRepresentativeId = option.id;
  } else {
    request.value.salesRepresentativeId = null;
  }
}

function validateAutocompleteSelections(): boolean {
  let valid = true;
  if (!directHiring.value && jobPosition.value && (!jobPositionSelected.value || jobPositionSelected.value.jobPosition.toLowerCase() !== jobPosition.value.toLowerCase())) {
    form.setFieldError('jobPosition', 'Please select a role from the list');
    valid = false;
  }
  if (branchOffice.value && (!locationSelected.value || locationSelected.value.formattedAddress.toLowerCase() !== branchOffice.value.toLowerCase())) {
    form.setFieldError('branchOffice', 'Please select a location from the list');
    valid = false;
  }
  return valid;
}

function onSubmit() {
  form.markInteracted([
    'jobTitle', 'billingTitle', 'jobCosting', 'workersQuantity', 'workerSalary',
    'jobPosition', 'branchOffice', 'description', 'requirements',
    'incentive', 'incentiveDescription', 'startAt',
  ]);
  form.handleSubmit((values) => {
    if (!validateAutocompleteSelections()) {
      showAlertError(errorMessage);
      return;
    }
    const payload: any = {
      ...request.value,
      jobTitle: values.jobTitle,
      billingTitle: values.billingTitle,
      jobCosting: isAdmin.value ? values.jobCosting : request.value.jobCosting,
      workersQuantity: values.workersQuantity,
      workerSalary: directHiring.value ? values.workerSalary : null,
      description: values.description,
      requirements: values.requirements,
      incentive: values.incentive,
      incentiveDescription: values.incentiveDescription,
      startAt: values.startAt,
      durationBreak: dayjs(request.value.durationBreak).format('HH:mm'),
    };
    if (isUpdate.value) {
      updateRequest(payload);
    } else {
      createRequest(payload);
    }
  }, () => {
    showAlertError(errorMessage);
  })();
}

function createRequest(payload: any) {
  isLoading.value = true;
  postAgencyRequest(payload)
    .then((response: any) => {
      showAlertSuccess('Request created');
      router.push(requestBase.value + '/' + response.id);
      isLoading.value = false;
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

async function onUpdateRolesModal() {
  showRolesModal.value = false;
  companyJobPositions.value = await getAgencyCompanyJobPositions(companyProfileId.value);
}

async function onUpdateLocationModal() {
  showLocationModal.value = false;
  locations.value = await getAgencyCompanyLocation(companyProfileId.value);
}

function updateRequest(payload: any) {
  isLoading.value = true;
  updateAgencyRequest(request.value.id, payload)
    .then((response: any) => {
      isLoading.value = false;
      showAlertSuccess('Request updated');
      router.push(requestBase.value + '/' + response.id);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onSameBillingChecked(value: boolean) {
  if (value) {
    form.setFieldValue('billingTitle', jobTitle.value);
  } else {
    form.setFieldValue('billingTitle', '');
  }
}
</script>
