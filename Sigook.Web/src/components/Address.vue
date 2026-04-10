<template>
  <div class="container-flex">
    <b-loading v-model="isLoading"></b-loading>
    <div class="col-12 col-padding">
      <b-field :type="errors.has('country') ? 'is-danger' : ''"
        :message="errors.has('country') ? errors.first('country') : ''">
        <template #label>
          {{ $t('Country') }} <span class="has-text-danger">*</span>
        </template>
        <b-select :placeholder="$t('Select')" v-model="country" name="country" v-validate="'required'" expanded
          @input="onCountrySelected">
          <option v-for="country in countries" :key="country.id" :value="country">{{ country.value }}</option>
        </b-select>
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('province') ? 'is-danger' : ''">
        <template #label>
          {{ $t('Province') }} <span class="has-text-danger">*</span>
        </template>
        <template #message>
          <span v-if="errors.has('province')">{{ errors.first('province') }}</span>
          <a v-if="provinceSelected && isPayrollManager && enableProvinceSettings"
             @click="openProvinceSettings"
             class="province-configure-link">
            {{ provinceSelected.settings ? 'See Settings' : 'Configure' }}
          </a>
        </template>
        <b-autocomplete :data="filteredProvinces" :placeholder="$t('Select')" v-model="province" open-on-focus
          name="province" v-validate="'required'" :loading="loadingProvinces" @select="onProvinceSelected"></b-autocomplete>
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('city') ? 'is-danger' : ''"
        :message="errors.has('city') ? errors.first('city') : ''">
        <template #label>
          {{ $t('City') }} <span class="has-text-danger">*</span>
        </template>
        <b-autocomplete :data="filteredCities" ref="autoCompleteCities" :placeholder="$t('Select')" v-model="city"
          open-on-focus name="city" v-validate="'required'" :loading="loadingCities" selectable-footer @select="onCitySelected"
          @select-footer="addCity">
          <template v-if="isAgency" #footer>
            <a><span> Add new... </span></a>
          </template>
          <template #empty>No results for {{ city }}</template>
        </b-autocomplete>
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('address') ? 'is-danger' : ''"
        :message="errors.has('address') ? errors.first('address') : ''">
        <template #label>
          {{ $t('Address') }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="localModel.address" name="address"
          v-validate="{ required: true, max: 100, min: 6, regex: $regexAddress }" />
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('postalCode') ? 'is-danger' : ''"
        :message="errors.has('postalCode') ? errors.first('postalCode') : ''">
        <template #label>
          {{ $t('PostalCode') }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="localModel.postalCode" name="postalCode"
          v-validate="{ 'cvn-postal-code': 'cvn-postal-code' }" />
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
import roles from "@/security/roles";
import ProvinceSettingsModal from "@/components/ProvinceSettingsModal.vue";
import billingAdminMixin from "@/mixins/billingAdminMixin";
import { getCountries, getProvinces, getCities, createCity } from "@/api/locationApi";

export default {
  mixins: [billingAdminMixin],
  components: {
    ProvinceSettingsModal
  },
  props: ["model", "enableProvinceSettings"],
  data() {
    return {
      isLoading: false,
      localModel: JSON.parse(JSON.stringify(this.model)),
      countries: [],
      country: null,
      provinces: [],
      province: '',
      provinceSelected: null,
      cities: [],
      city: '',
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
    validateSelection() {
      let valid = true;

      if (this.province && (!this.provinceSelected || this.provinceSelected.value.toLowerCase() !== this.province.toLowerCase())) {
        this.errors.add({ field: 'province', msg: 'Please select a province from the list' });
        valid = false;
      }

      if (this.city && (!this.citySelected || this.citySelected.value.toLowerCase() !== this.city.toLowerCase())) {
        this.errors.add({ field: 'city', msg: 'Please select a city from the list' });
        valid = false;
      }

      return valid;
    },
    async validateAddress() {
      const fieldsValid = await this.$validator.validateAll();
      const selectionValid = this.validateSelection();
      return fieldsValid && selectionValid;
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
    'localModel.address'() {
      this.$emit('update:model', this.localModel);
    },
    'localModel.postalCode'() {
      this.$emit('update:model', this.localModel);
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
    filteredProvinces() {
      const provinces = this.provinces.filter(c => c.value.toLowerCase().includes(this.province.toLowerCase()));
      return provinces;
    },
    filteredCities() {
      const cities = this.cities.filter(c => c.value.toLowerCase().includes(this.city.toLowerCase()));
      return cities;
    },
    isAgency() {
      return this.$store.state.security.userRoles.some(ur => ur === roles.agencyPersonnel);
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
