<template>
  <div class="container-flex">
    <b-loading v-model="isLoading"></b-loading>
    <div class="col-12 col-padding">
      <b-field :type="errors.country ? 'is-danger' : ''" :message="errors.country || ''">
        <template #label>
          {{ 'Country' }} <span class="has-text-danger">*</span>
        </template>
        <b-select :placeholder="'Select'" v-model="country" name="country" expanded
          @update:modelValue="onCountrySelected">
          <option v-for="country in countries" :key="country.id" :value="country">{{ country.value }}</option>
        </b-select>
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.province ? 'is-danger' : ''">
        <template #label>
          {{ 'Province/State' }} <span class="has-text-danger">*</span>
        </template>
        <template #message>
          <span v-if="errors.province">{{ errors.province }}</span>
          <a v-if="provinceSelected && isPayrollManager && enableProvinceSettings"
             @click="openProvinceSettings"
             class="province-configure-link">
            {{ provinceSelected.settings ? 'See Settings' : 'Configure' }}
          </a>
        </template>
        <b-autocomplete :data="filteredProvinces" :placeholder="'Select'" v-model="province" open-on-focus
          name="province" :loading="loadingProvinces" @select="onProvinceSelected"></b-autocomplete>
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
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
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.address ? 'is-danger' : ''" :message="errors.address || ''">
        <template #label>
          {{ 'Address' }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="address" name="address" />
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.postalCode ? 'is-danger' : ''" :message="errors.postalCode || ''">
        <template #label>
          {{ 'Postal/ZIP Code' }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="postalCode" name="postalCode" />
      </b-field>
    </div>
    <b-modal v-model="showProvinceSettingsModal" width="500px">
      <province-settings-modal v-if="provinceSelected" :provinceId="provinceSelected.id"
        :provinceName="provinceSelected.value" :currentSettings="provinceSelected.settings"
        @saved="onProvinceSettingsSaved" />
    </b-modal>
  </div>
</template>

<script lang="ts">
import { computed, reactive, watch } from 'vue';
import { mapStores } from 'pinia';
import { useForm } from 'vee-validate';
import * as yup from 'yup';
import { useSecurityStore } from '@/stores/security';
import roles from "@/security/roles";
import ProvinceSettingsModal from "@/components/ProvinceSettingsModal.vue";
import { useBillingAdmin } from '@/composables/useBillingAdmin';
import { getCountries, getProvinces, getCities, createCity } from "@/api/locationApi";
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

export default {
  setup(props: any) {
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
      country: interacted.country ? (formErrors.value.country || '') : '',
      province: interacted.province ? (formErrors.value.province || '') : '',
      city: interacted.city ? (formErrors.value.city || '') : '',
      address: interacted.address ? (formErrors.value.address || '') : '',
      postalCode: interacted.postalCode ? (formErrors.value.postalCode || '') : '',
    }));

    function markAllInteracted() {
      interacted.country = true;
      interacted.province = true;
      interacted.city = true;
      interacted.address = true;
      interacted.postalCode = true;
    }

    return {
      ...useBillingAdmin(),
      errors,
      country,
      province,
      city,
      address,
      postalCode,
      validate,
      setFieldError,
      markAllInteracted,
    };
  },
  components: {
    ProvinceSettingsModal
  },
  props: ["model", "enableProvinceSettings"],
  data() {
    return {
      isLoading: false,
      localModel: JSON.parse(JSON.stringify(this.model)),
      countries: [],
      provinces: [],
      provinceSelected: null,
      cities: [],
      citySelected: null,
      loadingProvinces: false,
      loadingCities: false,
      showProvinceSettingsModal: false,
    };
  },
  created() {
    this.getCountries();
    if (this.localModel && this.localModel.id) {
      this.getProvincesByCountry(this.localModel.city.province.country);
      this.getCityByProvince(this.localModel.city.province);
      this.country = this.localModel.city.province.country;
      this.province = this.localModel.city.province.value;
      this.provinceSelected = this.localModel.city.province;
      this.city = this.localModel.city.value;
      this.citySelected = this.localModel.city;
    }
  },
  methods: {
    onCountrySelected(country) {
      this.province = '';
      this.city = '';
      this.getProvincesByCountry(country);
      this.$emit("isCanada", country.code === "CA");
    },
    onProvinceSelected(province) {
      this.provinceSelected = province;
      this.city = '';
      this.getCityByProvince(province);
    },
    onCitySelected(city) {
      this.citySelected = city;
      this.localModel.city = {
        ...this.citySelected,
        province: {
          ...this.provinceSelected,
          country: {
            ...this.country
          }
        }
      };
      this.$emit('update:model', this.localModel);
    },
    getCountries() {
      this.$emit("isLoading", true);
      getCountries().then((r) => {
        this.countries = r;
        this.$emit("isLoading", false);
      });
    },
    getProvincesByCountry(country) {
      if (!country || !country.id) {
        return;
      }
      this.loadingProvinces = true;
      getProvinces(country.id)
        .then((r) => {
          this.provinces = r;
          this.loadingProvinces = false;
        });
    },
    getCityByProvince(province) {
      if (!province || !province.id) {
        return
      }
      this.loadingCities = true;
      getCities(province.id).then((response) => {
        this.cities = response;
        this.loadingCities = false;
      });
    },
    addCity() {
      this.$buefy.dialog.prompt({
        message: `City`,
        inputAttrs: {
          placeholder: 'City',
          maxlength: 20,
          value: this.city
        },
        closeOnConfirm: false,
        confirmText: 'Add',
        onConfirm: async (value, dialog) => {
          const payload = {
            value,
            province: {
              id: this.provinceSelected.id
            }
          };
          const newCity = await createCity(payload);
          this.cities.push(newCity);
          this.$refs.autoCompleteCities.setSelected(newCity);
          dialog.close();
        }
      })
    },
    async validateAddress(): Promise<boolean> {
      (this as any).markAllInteracted();
      const result = await (this as any).validate();
      let valid = result.valid;

      if (this.province && (!this.provinceSelected || this.provinceSelected.value.toLowerCase() !== this.province.toLowerCase())) {
        this.setFieldError('province', 'Please select a province from the list');
        valid = false;
      }

      if (this.city && (!this.citySelected || this.citySelected.value.toLowerCase() !== this.city.toLowerCase())) {
        this.setFieldError('city', 'Please select a city from the list');
        valid = false;
      }

      return valid;
    },
    openProvinceSettings() {
      this.showProvinceSettingsModal = true;
    },
    onProvinceSettingsSaved(settings) {
      this.provinceSelected.settings = settings;
      if (this.localModel.city && this.localModel.city.province) {
        this.localModel.city.province.settings = settings;
        this.$emit('update:model', this.localModel);
      }
      this.showProvinceSettingsModal = false;
    }
  },
  watch: {
    model: {
      handler(newVal) {
        this.localModel = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    },
    address() {
      if (this.localModel) {
        this.localModel.address = this.address;
        this.$emit('update:model', this.localModel);
      }
    },
    postalCode() {
      if (this.localModel) {
        this.localModel.postalCode = this.postalCode;
        this.$emit('update:model', this.localModel);
      }
    },
    province(newVal) {
      if (this.provinceSelected && this.provinceSelected.value !== newVal) {
        this.provinceSelected = null;
        this.city = '';
        this.citySelected = null;
        this.cities = [];
      }
    },
    city(newVal) {
      if (this.citySelected && this.citySelected.value !== newVal) {
        this.citySelected = null;
      }
    }
  },
  computed: {
    ...mapStores(useSecurityStore),
    filteredProvinces() {
      const provinces = this.provinces.filter(c => c.value.toLowerCase().includes((this.province || '').toLowerCase()));
      return provinces;
    },
    filteredCities() {
      const cities = this.cities.filter(c => c.value.toLowerCase().includes((this.city || '').toLowerCase()));
      return cities;
    },
    isAgency() {
      return this.securityStore.userRoles.some(ur => ur === roles.agencyPersonnel);
    }
  },
};
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
