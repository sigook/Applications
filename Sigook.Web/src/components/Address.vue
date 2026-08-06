<template>
  <div class="columns is-multiline">
    <b-loading v-model="isLoading"></b-loading>
    <div class="column is-12">
      <b-field :type="errors.country ? 'is-danger' : ''" :message="errors.country || ''">
        <template #label>
          {{ 'Country' }} <span class="has-text-danger">*</span>
        </template>
        <b-select :placeholder="'Select'" v-model="country" name="country" expanded
          @update:modelValue="onCountrySelected">
          <option v-for="c in countries" :key="c.id" :value="c">{{ c.value }}</option>
        </b-select>
      </b-field>
    </div>
    <div class="column is-6">
      <b-field :type="errors.province ? 'is-danger' : ''">
        <template #label>
          {{ 'Province/State' }} <span class="has-text-danger">*</span>
        </template>
        <template #message>
          <span v-if="errors.province">{{ errors.province }}</span>
          <a v-if="provinceSelected && isAdmin && enableProvinceSettings"
             @click="openProvinceSettings"
             class="province-configure-link">
            {{ provinceSelected.settings ? 'See Settings' : 'Configure' }}
          </a>
        </template>
        <b-autocomplete :data="filteredProvinces" :placeholder="'Select'" v-model="province" open-on-focus
          name="province" :loading="loadingProvinces" @select="onProvinceSelected"></b-autocomplete>
      </b-field>
    </div>
    <div class="column is-6">
      <b-field :type="errors.city ? 'is-danger' : ''" :message="errors.city || ''">
        <template #label>
          {{ 'City' }} <span class="has-text-danger">*</span>
        </template>
        <b-autocomplete :data="filteredCities" ref="autoCompleteCities" :placeholder="'Select'" v-model="city"
          open-on-focus name="city" :loading="loadingCities" selectable-footer @select="onCitySelected"
          @select-footer="addCity">
          <template v-if="isAgency" #footer>
            <a><span> Add new... </span></a>
          </template>
          <template #empty>No results for {{ city }}</template>
        </b-autocomplete>
      </b-field>
    </div>
    <div class="column is-6">
      <b-field :type="errors.address ? 'is-danger' : ''" :message="errors.address || ''">
        <template #label>
          {{ 'Address' }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="address" name="address" />
      </b-field>
    </div>
    <div class="column is-6">
      <b-field :type="errors.postalCode ? 'is-danger' : ''" :message="errors.postalCode || ''">
        <template #label>
          {{ 'Postal/ZIP Code' }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="postalCode" name="postalCode" />
      </b-field>
    </div>
    <b-modal custom-content-class="card" v-model="showProvinceSettingsModal" width="500px">
      <province-settings-modal v-if="provinceSelected" :provinceId="provinceSelected.id"
        :provinceName="provinceSelected.value" :currentSettings="provinceSelected.settings"
        @saved="onProvinceSettingsSaved" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import { useSecurityStore } from '@/stores/security';
import { agencyStaff } from "@/security/roles";
import ProvinceSettingsModal from "@/components/ProvinceSettingsModal.vue";
import { getDialog } from '@/utils/buefyProgrammatic';
import { useAdmin } from '@/composables/useAdmin';
import {
  getCountries as fetchCountries,
  getProvinces as fetchProvinces,
  getCities as fetchCities,
  createCity,
} from "@/api/locationApi";
import { postalCodeSchema } from '@/utils/validation';
import { appGlobals } from '@/varaibles';

const addressSchema = yup.object({
  country: yup.mixed().required('Country is required'),
  province: yup.string().required('Province is required'),
  city: yup.string().required('City is required'),
  address: yup
    .string()
    .required('Address is required')
    .min(6, 'Address must be at least 6 characters')
    .max(100, 'Address must be at most 100 characters')
    .matches(appGlobals.$regexAddress, 'Invalid address format'),
  postalCode: postalCodeSchema(true),
});

const props = defineProps<{ model?: any; enableProvinceSettings?: boolean }>();
const emit = defineEmits<{
  (e: 'isCanada', v: boolean): void;
  (e: 'isLoading', v: boolean): void;
  (e: 'update:model', v: any): void;
}>();

const { isAdmin } = useAdmin();
const securityStore = useSecurityStore();

const { errors: formErrors, defineField, setFieldError, validate } = useForm({
  validationSchema: addressSchema,
  initialValues: {
    country: null,
    province: '',
    city: '',
    address: props.model?.address || '',
    postalCode: props.model?.postalCode || '',
  },
});

const [country] = defineField('country');
const [province] = defineField('province');
const [city] = defineField('city');
const [address] = defineField('address');
const [postalCode] = defineField('postalCode');

const interacted = reactive<Record<string, boolean>>({});
watch(country, () => { interacted.country = true; });
watch(province, () => { interacted.province = true; });
watch(city, () => { interacted.city = true; });
watch(address, () => { interacted.address = true; });
watch(postalCode, () => { interacted.postalCode = true; });

const errors = computed(() => ({
  country: interacted.country ? ((formErrors.value as Record<string, string>).country || '') : '',
  province: interacted.province ? ((formErrors.value as Record<string, string>).province || '') : '',
  city: interacted.city ? ((formErrors.value as Record<string, string>).city || '') : '',
  address: interacted.address ? ((formErrors.value as Record<string, string>).address || '') : '',
  postalCode: interacted.postalCode ? ((formErrors.value as Record<string, string>).postalCode || '') : '',
}));

function markAllInteracted() {
  interacted.country = true;
  interacted.province = true;
  interacted.city = true;
  interacted.address = true;
  interacted.postalCode = true;
}

const isLoading = ref(false);
const localModel = ref<any>(JSON.parse(JSON.stringify(props.model)));
const countries = ref<any[]>([]);
const provinces = ref<any[]>([]);
const provinceSelected = ref<any>(null);
const cities = ref<any[]>([]);
const citySelected = ref<any>(null);
const loadingProvinces = ref(false);
const loadingCities = ref(false);
const showProvinceSettingsModal = ref(false);
const autoCompleteCities = ref<any>(null);

function loadCountries() {
  emit('isLoading', true);
  fetchCountries().then((r) => {
    countries.value = r;
    emit('isLoading', false);
  });
}

function getProvincesByCountry(c: any) {
  if (!c || !c.id) return;
  loadingProvinces.value = true;
  fetchProvinces(c.id).then((r) => {
    provinces.value = r;
    loadingProvinces.value = false;
  });
}

function getCityByProvince(p: any) {
  if (!p || !p.id) return;
  loadingCities.value = true;
  fetchCities(p.id).then((response) => {
    cities.value = response;
    loadingCities.value = false;
  });
}

// created()
loadCountries();
if (localModel.value && localModel.value.id) {
  getProvincesByCountry(localModel.value.city.province.country);
  getCityByProvince(localModel.value.city.province);
  country.value = localModel.value.city.province.country;
  province.value = localModel.value.city.province.value;
  provinceSelected.value = localModel.value.city.province;
  city.value = localModel.value.city.value;
  citySelected.value = localModel.value.city;
}

function onCountrySelected(c: any) {
  province.value = '';
  city.value = '';
  getProvincesByCountry(c);
  emit('isCanada', c.code === 'CA');
}

function onProvinceSelected(p: any) {
  provinceSelected.value = p;
  city.value = '';
  getCityByProvince(p);
}

function onCitySelected(c: any) {
  citySelected.value = c;
  localModel.value.city = {
    ...citySelected.value,
    province: {
      ...provinceSelected.value,
      country: {
        ...country.value,
      },
    },
  };
  emit('update:model', localModel.value);
}

function addCity() {
  getDialog().prompt({
    message: `City`,
    inputAttrs: {
      placeholder: 'City',
      maxlength: 20,
      value: city.value,
    },
    closeOnConfirm: false,
    confirmText: 'Add',
    onConfirm: async (value: string, dialog: any) => {
      const payload = {
        value,
        province: { id: provinceSelected.value.id },
      };
      const newCity = await createCity(payload);
      cities.value.push(newCity);
      autoCompleteCities.value?.setSelected(newCity);
      dialog.close();
    },
  });
}

async function validateAddress(): Promise<boolean> {
  markAllInteracted();
  const result = await validate();
  let valid = result.valid;

  if (province.value && (!provinceSelected.value || provinceSelected.value.value.toLowerCase() !== province.value.toLowerCase())) {
    setFieldError('province', 'Please select a province from the list');
    valid = false;
  }

  if (city.value && (!citySelected.value || citySelected.value.value.toLowerCase() !== city.value.toLowerCase())) {
    setFieldError('city', 'Please select a city from the list');
    valid = false;
  }

  return valid;
}

function openProvinceSettings() {
  showProvinceSettingsModal.value = true;
}

function onProvinceSettingsSaved(settings: any) {
  provinceSelected.value.settings = settings;
  if (localModel.value.city && localModel.value.city.province) {
    localModel.value.city.province.settings = settings;
    emit('update:model', localModel.value);
  }
  showProvinceSettingsModal.value = false;
}

watch(() => props.model, (newVal) => {
  localModel.value = JSON.parse(JSON.stringify(newVal));
}, { deep: true });

watch(address, () => {
  if (localModel.value) {
    localModel.value.address = address.value;
    emit('update:model', localModel.value);
  }
});

watch(postalCode, () => {
  if (localModel.value) {
    localModel.value.postalCode = postalCode.value;
    emit('update:model', localModel.value);
  }
});

watch(province, (newVal) => {
  if (provinceSelected.value && provinceSelected.value.value !== newVal) {
    provinceSelected.value = null;
    city.value = '';
    citySelected.value = null;
    cities.value = [];
  }
});

watch(city, (newVal) => {
  if (citySelected.value && citySelected.value.value !== newVal) {
    citySelected.value = null;
  }
});

const filteredProvinces = computed(() =>
  provinces.value.filter(c => c.value.toLowerCase().includes((province.value || '').toLowerCase()))
);

const filteredCities = computed(() =>
  cities.value.filter(c => c.value.toLowerCase().includes((city.value || '').toLowerCase()))
);

const isAgency = computed(() =>
  securityStore.userRoles.some(ur => agencyStaff.includes(ur))
);

defineExpose({ validateAddress });
</script>

<style scoped>
.province-configure-link {
  margin-left: 0.5rem;
  color: #3273dc;
  text-decoration: underline;
  cursor: pointer;
}

.province-configure-link:hover {
  color: #1d5cb8;
  text-decoration: underline;
}
</style>
