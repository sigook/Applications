<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <form class="page-form" @submit.prevent="onSubmit">
      <PageHeader title="Create Request" :crumbs="crumbs" back-to="/company-requests" />

      <div class="columns">
        <div class="column is-8-desktop">
          <div class="card section-card">
            <div class="card-header">
              <p class="card-header-title">Position</p>
            </div>
            <div class="card-content">
              <div class="columns is-multiline">
                <div class="column is-6 is-4-desktop">
                  <b-field :label="`${'Job title'} *`" :message="formErrors.jobTitle || ''"
                    :type="formErrors.jobTitle ? 'is-danger' : ''">
                    <b-input v-model="jobTitle" name="job title" />
                  </b-field>
                </div>
                <div class="column is-6 is-4-desktop" v-if="directHiring">
                  <b-field :type="formErrors.workerSalary ? 'is-danger' : ''" label="Worker Salary *"
                    :message="formErrors.workerSalary || ''">
                    <b-numberinput v-model="workerSalary" name="workerSalary" controls-alignment="right"></b-numberinput>
                  </b-field>
                </div>
                <div class="column is-6 is-4-desktop">
                  <b-field :type="formErrors.workersQuantity ? 'is-danger' : ''" :label="`${'Workers Quantity'} *`"
                    :message="formErrors.workersQuantity || ''">
                    <b-numberinput v-model="workersQuantity" name="worker quantity" controls-alignment="right" expanded></b-numberinput>
                  </b-field>
                </div>
                <div class="column is-6 is-4-desktop" v-if="!directHiring">
                  <b-field :label="`${'Rol'} *`" :message="formErrors.jobPosition || ''"
                    :type="formErrors.jobPosition ? 'is-danger' : ''">
                    <b-autocomplete :data="filteredCompanyJobPositions" placeholder="Role" v-model="jobPosition" field="jobPosition"
                      open-on-focus name="rol" @select="onJobPositionSelected">
                      <template #empty>You don't have any roles created</template>
                    </b-autocomplete>
                  </b-field>
                  <b-tag v-if="request.rate">Rate for this position: {{ request.rate }}</b-tag>
                </div>
                <div class="column is-6 is-4-desktop">
                  <b-field :label="`${'Branch office'} *`"
                    :message="formErrors.branchOffice || ''"
                    :type="formErrors.branchOffice ? 'is-danger' : ''">
                    <b-autocomplete :data="filteredLocations" placeholder="Location" v-model="branchOffice" open-on-focus
                      name="branchOffice" selectable-footer field="formattedAddress"
                      @select="onLocationSelected" @select-footer="() => showLocationModal = true">
                      <template #footer>
                        <a><span> Add new... </span></a>
                      </template>
                      <template #empty>You don't have any location created</template>
                    </b-autocomplete>
                  </b-field>
                </div>
              </div>
            </div>
          </div>

          <div class="card section-card">
            <div class="card-header">
              <p class="card-header-title">Content</p>
            </div>
            <div class="card-content">
              <div class="columns is-multiline">
                <div class="column is-12 is-6-desktop">
                  <b-field :label="`${'Description'} *`" :message="formErrors.description || ''"
                    :type="formErrors.description ? 'is-danger' : ''">
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
                  <b-field :label="`${'Requirements'} *`" :message="formErrors.requirements || ''"
                    :type="formErrors.requirements ? 'is-danger' : ''">
                    <div class="vue-trix-editor">
                      <QuillEditor theme="snow" content-type="html" v-model:content="requirements" />
                    </div>
                  </b-field>
                </div>
              </div>
            </div>
          </div>

          <div class="card section-card">
            <div class="card-header">
              <p class="card-header-title">Terms &amp; Schedule</p>
            </div>
            <div class="card-content">
              <div class="columns is-multiline">
                <div class="column is-6 is-3-desktop">
                  <b-field :type="formErrors.incentive ? 'is-danger' : ''" :label="'Incentive'"
                    :message="formErrors.incentive || ''">
                    <b-numberinput controls-alignment="right" v-model="incentive" name="incentive" step="0.01" />
                  </b-field>
                </div>
                <div class="column is-6 is-9-desktop">
                  <b-field :type="formErrors.incentiveDescription ? 'is-danger' : ''" :label="'Incentive Description'"
                    :message="formErrors.incentiveDescription || ''">
                    <b-input v-model="incentiveDescription" name="incentiveDes" :disabled="!incentive" />
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
                  <b-field label="Start *" :type="formErrors.startAt ? 'is-danger' : ''"
                    :message="formErrors.startAt || ''">
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
            </div>
          </div>
        </div>

        <div class="column is-4-desktop">
          <div class="card settings-card">
            <div class="card-header">
              <p class="card-header-title">Settings</p>
            </div>
            <div class="card-content">
              <div class="settings-list">
                <b-switch v-model="directHiring">Direct Hiring</b-switch>
                <b-switch v-model="request.isAsap">Is Asap</b-switch>
                <div class="setting-row" v-if="!directHiring">
                  <b-switch v-model="request.breakIsPaid">Duration break is paid</b-switch>
                  <div class="setting-row-control">
                    <b-timepicker v-model="request.durationBreak" :max-time="maxBreak" hour-format="24"
                      :disabled="!request.breakIsPaid">
                    </b-timepicker>
                  </div>
                </div>
              </div>
              <div class="settings-actions">
                <b-button type="is-primary" native-type="submit" expanded>
                  {{ "Create" }}
                </b-button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="mobile-submit is-hidden-desktop">
        <b-button type="is-primary" native-type="submit" expanded>
          Create
        </b-button>
      </div>
    </form>

    <b-modal custom-content-class="card" v-model="showLocationModal" @close="showLocationModal = false" width="500px">
      <LocationForm @updateContent="onUpdateLocationModal"></LocationForm>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import * as yup from 'yup';
import dayjs from 'dayjs';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import {
  DurationTerm,
  DurationTermLabels,
  EmploymentType,
  EmploymentTypeLabels,
} from '@/constants/enums';
import { getLocations, getCompanyJobPositions, createRequest } from '@/api/companyApi';
import LocationForm from '@/components/agency_company/LocationForm.vue';
import PageHeader from '@/components/PageHeader.vue';
import type { PageBreadcrumb } from '@/types/common';

const crumbs: PageBreadcrumb[] = [{ label: 'Requests', to: '/company-requests' }];

const router = useRouter();
const errorMessage = 'Please make sure all required fields are filled out correctly';

const directHiring = ref(false);

const validationSchema = computed(() => {
  const shape: Record<string, any> = {
    jobTitle: yup.string().required('Job title is required').min(1).max(100, 'Max 100 characters'),
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
  jobTitle, workersQuantity, workerSalary, jobPosition, branchOffice,
  description, requirements, incentive, incentiveDescription, startAt,
} = form.fields;
const formErrors = form.errors;

const maxBreak = new Date();
maxBreak.setHours(1);
maxBreak.setMinutes(0);

const timeZero = dayjs().subtract(14, 'days').toDate();
timeZero.setHours(0);
timeZero.setMinutes(0);

const isLoading = ref(true);
const locations = ref<any[]>([]);
const companyJobPositions = ref<any[]>([]);
const request = reactive<any>({
  durationBreak: dayjs().startOf('day').toDate(),
  durationTerm: DurationTerm.LongTerm,
  employmentType: EmploymentType.FullTime,
  punchCardOptionEnabled: true,
});
const jobPositionSelected = ref<any>(null);
const locationSelected = ref<any>(null);
const showLocationModal = ref(false);

(async () => {
  locations.value = await getLocations();
  companyJobPositions.value = await getCompanyJobPositions();
  isLoading.value = false;
})();

const filteredCompanyJobPositions = computed(() => {
  const search = (jobPosition.value || '').toLowerCase();
  return companyJobPositions.value.filter((cjp: any) => (cjp.jobPosition || '').toLowerCase().includes(search));
});

const filteredLocations = computed(() => {
  const search = (branchOffice.value || '').toLowerCase();
  return locations.value.filter((l: any) => l.formattedAddress.toLowerCase().includes(search));
});

const finishDate = computed(() => dayjs(startAt.value).add(1, 'year').toDate());

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

function submitRequest(payload: any) {
  isLoading.value = true;
  createRequest(payload)
    .then(() => {
      showAlertSuccess('Request created');
      router.push('company-requests');
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function onSubmit() {
  form.markInteracted([
    'jobTitle', 'workersQuantity', 'workerSalary', 'jobPosition',
    'branchOffice', 'description', 'requirements', 'incentive',
    'incentiveDescription', 'startAt',
  ]);
  form.handleSubmit((values) => {
    if (!validateAutocompleteSelections()) {
      showAlertError(errorMessage);
      return;
    }
    const payload: any = {
      ...request,
      jobTitle: values.jobTitle,
      workersQuantity: values.workersQuantity,
      workerSalary: directHiring.value ? values.workerSalary : null,
      description: values.description,
      requirements: values.requirements,
      incentive: values.incentive,
      incentiveDescription: values.incentiveDescription,
      startAt: values.startAt,
      durationBreak: dayjs(request.durationBreak).format('HH:mm'),
    };
    submitRequest(payload);
  }, () => {
    showAlertError(errorMessage);
  })();
}

function onJobPositionSelected(option: any) {
  jobPositionSelected.value = option;
  if (option) {
    request.shift = option.shift;
    request.rate = option.workerRate;
    request.jobPositionRateId = option.id;
  } else {
    request.shift = null;
    request.rate = null;
    request.jobPositionRateId = null;
  }
}

function onLocationSelected(option: any) {
  locationSelected.value = option;
  if (option) {
    request.locationId = option.id;
  } else {
    request.locationId = null;
  }
}

async function onUpdateLocationModal() {
  showLocationModal.value = false;
  locations.value = await getLocations();
}

watch(jobPosition, (newVal) => {
  if (jobPositionSelected.value && jobPositionSelected.value.jobPosition !== newVal) {
    jobPositionSelected.value = null;
    request.shift = null;
    request.rate = null;
    request.jobPositionRateId = null;
  }
});

watch(branchOffice, (newVal) => {
  if (locationSelected.value && locationSelected.value.formattedAddress !== newVal) {
    locationSelected.value = null;
    request.locationId = null;
  }
});

watch(directHiring, (val) => {
  if (!val) {
    form.setFieldValue('workerSalary', null);
  }
});
</script>
