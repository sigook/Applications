<template>
  <div class="landing-address">
    <div class="landing-address__row landing-address__row--full">
      <Select
        v-model="country"
        :options="countries"
        label="Country"
        placeholder="Select a country"
        :required="true"
        :error="errors.country"
        @update:model-value="onCountrySelected"
      />
    </div>

    <div class="landing-address__row">
      <Autocomplete
        v-model="provinceQuery"
        :data="provinces"
        label="State"
        placeholder="Select a state"
        :required="true"
        :error="errors.province"
        :disabled="!country"
        @select="onProvinceSelected"
      />
      <Autocomplete
        v-model="cityQuery"
        :data="cities"
        label="City"
        placeholder="Select a city"
        :required="true"
        :error="errors.city"
        :disabled="!provinceSelected"
        @select="onCitySelected"
      />
    </div>

    <div class="landing-address__row">
      <Input
        v-model="addressLine"
        label="Address"
        placeholder="Street address"
        :required="true"
        :error="errors.address"
        @update:model-value="emitChange"
      />
      <Input
        v-model="postalCode"
        label="ZIP Code"
        placeholder="12345"
        :required="true"
        :error="errors.postalCode"
        @update:model-value="emitChange"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import Input from '@/components/landing/shared/forms/Input.vue'
import Select from '@/components/landing/shared/forms/Select.vue'
import Autocomplete from '@/components/landing/shared/forms/Autocomplete.vue'
import {
  getCountries as fetchCountries,
  getProvinces as fetchProvinces,
  getCities as fetchCities,
} from '@/api/locationApi'

/**
 * AddressFields — composite address picker matching the layout / data
 * structure of the legacy Address.vue but using V2 atoms.
 *
 * Cascading load: pick country → loads provinces → pick province → loads
 * cities. Emits a normalized `address` object via `update:modelValue` and
 * a `change` event whenever any field updates.
 *
 * Validation: parent reads `errors` reactive object and feeds it back via
 * the optional `errors` prop. `validate()` is exposed via defineExpose so
 * the parent can trigger validation at submit time.
 */
interface CountryOption { id: number | string; value: string; code?: string }
interface ProvinceOption { id: number | string; value: string }
interface CityOption { id: number | string; value: string }

export interface AddressModel {
  country: CountryOption | null
  province: ProvinceOption | null
  city: CityOption | null
  address: string
  postalCode: string
}

const props = withDefaults(defineProps<{
  modelValue?: AddressModel
}>(), {
  modelValue: () => ({
    country: null,
    province: null,
    city: null,
    address: '',
    postalCode: '',
  }),
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: AddressModel): void
}>()

/* ── State ──────────────────────────────────────────────────────────────── */

const country = ref<CountryOption | null>(props.modelValue.country)
const provinceSelected = ref<ProvinceOption | null>(props.modelValue.province)
const citySelected = ref<CityOption | null>(props.modelValue.city)
const provinceQuery = ref(props.modelValue.province?.value ?? '')
const cityQuery = ref(props.modelValue.city?.value ?? '')
const addressLine = ref(props.modelValue.address ?? '')
const postalCode = ref(props.modelValue.postalCode ?? '')

const countries = ref<CountryOption[]>([])
const provinces = ref<ProvinceOption[]>([])
const cities = ref<CityOption[]>([])

const errors = reactive({
  country: '',
  province: '',
  city: '',
  address: '',
  postalCode: '',
})

/* ── API loads ──────────────────────────────────────────────────────────── */

async function loadCountries(): Promise<void> {
  const all = await fetchCountries()
  countries.value = all.filter((c) => c.code === 'USA')
  if (!country.value && countries.value.length === 1) {
    onCountrySelected(countries.value[0])
  }
}

async function loadProvinces(c: CountryOption | null): Promise<void> {
  if (!c?.id) {
    provinces.value = []
    return
  }
  provinces.value = await fetchProvinces(c.id as number)
}

async function loadCities(p: ProvinceOption | null): Promise<void> {
  if (!p?.id) {
    cities.value = []
    return
  }
  cities.value = await fetchCities(p.id as number)
}

void loadCountries()

/* ── Selection handlers ─────────────────────────────────────────────────── */

function onCountrySelected(c: CountryOption | null): void {
  country.value = c
  // Reset downstream
  provinceQuery.value = ''
  provinceSelected.value = null
  cityQuery.value = ''
  citySelected.value = null
  provinces.value = []
  cities.value = []
  void loadProvinces(c)
  emitChange()
}

function onProvinceSelected(p: ProvinceOption): void {
  provinceSelected.value = p
  provinceQuery.value = p.value
  cityQuery.value = ''
  citySelected.value = null
  cities.value = []
  void loadCities(p)
  emitChange()
}

function onCitySelected(c: CityOption): void {
  citySelected.value = c
  cityQuery.value = c.value
  emitChange()
}

/* ── Keep selection in sync if user clears the query ────────────────────── */

watch(provinceQuery, (val) => {
  if (provinceSelected.value && provinceSelected.value.value !== val) {
    provinceSelected.value = null
    cityQuery.value = ''
    citySelected.value = null
    cities.value = []
    emitChange()
  }
})

watch(cityQuery, (val) => {
  if (citySelected.value && citySelected.value.value !== val) {
    citySelected.value = null
    emitChange()
  }
})

/* ── Emit ───────────────────────────────────────────────────────────────── */

function emitChange(): void {
  emit('update:modelValue', {
    country: country.value,
    province: provinceSelected.value,
    city: citySelected.value,
    address: addressLine.value,
    postalCode: postalCode.value,
  })
}

/* ── Validation ─────────────────────────────────────────────────────────── */

function validate(): boolean {
  let valid = true
  errors.country = ''
  errors.province = ''
  errors.city = ''
  errors.address = ''
  errors.postalCode = ''

  if (!country.value) {
    errors.country = 'Country is required'
    valid = false
  }
  if (!provinceSelected.value) {
    errors.province = 'Please select a state from the list'
    valid = false
  }
  if (!citySelected.value) {
    errors.city = 'Please select a city from the list'
    valid = false
  }
  if (!addressLine.value || addressLine.value.length < 6) {
    errors.address = 'Address must be at least 6 characters'
    valid = false
  } else if (addressLine.value.length > 100) {
    errors.address = 'Address must be at most 100 characters'
    valid = false
  }
  if (!postalCode.value) {
    errors.postalCode = 'Postal / ZIP code is required'
    valid = false
  }
  return valid
}

defineExpose({ validate })
</script>

<style scoped>
.landing-address {
  display: flex;
  flex-direction: column;
  gap: clamp(14px, 1.8vw, 20px);
}

.landing-address__row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(14px, 1.8vw, 20px);
}

.landing-address__row--full {
  grid-template-columns: 1fr;
}

@media (max-width: 639px) {
  .landing-address__row {
    grid-template-columns: 1fr;
  }
}
</style>
