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
          name="province" v-validate="'required'" @select="onProvinceSelected"></b-autocomplete>
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('city') ? 'is-danger' : ''"
        :message="errors.has('city') ? errors.first('city') : ''">
        <template #label>
          {{ $t('City') }} <span class="has-text-danger">*</span>
        </template>
        <b-autocomplete :data="filteredCities" ref="autoCompleteCities" :placeholder="$t('Select')" v-model="city"
          open-on-focus name="city" v-validate="'required'" selectable-footer @select="onCitySelected"
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
        <b-input type="text" v-model="model.address" name="address"
          v-validate="{ required: true, max: 100, min: 6, regex: $regexAddress }" />
      </b-field>
    </div>
    <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
      <b-field :type="errors.has('postalCode') ? 'is-danger' : ''"
        :message="errors.has('postalCode') ? errors.first('postalCode') : ''">
        <template #label>
          {{ $t('PostalCode') }} <span class="has-text-danger">*</span>
        </template>
        <b-input type="text" v-model="model.postalCode" name="postalCode"
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

<script>
import roles from "@/security/roles";
import ProvinceSettingsModal from "@/components/ProvinceSettingsModal";
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  mixins: [billingAdminMixin],
  components: {
    ProvinceSettingsModal
  },
  props: ["model", "enableProvinceSettings"],
  data() {
    return {
      isLoading: false,
      countries: [],
      country: null,
      provinces: [],
      province: '',
      provinceSelected: null,
      cities: [],
      city: '',
      citySelected: null,
      showProvinceSettingsModal: false,
    };
  },
  created() {
    this.getCountries();
    if (this.model && this.model.id) {
      this.getProvincesByCountry(this.model.city.province.country);
      this.getCityByProvince(this.model.city.province);
      this.country = this.model.city.province.country;
      this.province = this.model.city.province.value;
      this.provinceSelected = this.model.city.province;
      this.city = this.model.city.value;
      this.citySelected = this.model.city;
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
      this.model.city = {
        ...this.citySelected,
        province: {
          ...this.provinceSelected,
          country: {
            ...this.country
          }
        }
      };
    },
    getCountries() {
      this.$emit("isLoading", true);
      this.$store.dispatch("getCountries").then((r) => {
        this.countries = r;
        this.$emit("isLoading", false);
      });
    },
    getProvincesByCountry(country) {
      if (!country || !country.id) {
        return;
      }
      this.$emit("isLoading", true);
      this.$store
        .dispatch("getProvinces", country.id)
        .then((r) => {
          this.provinces = r;
          this.$emit("isLoading", false);
        });
    },
    getCityByProvince(province) {
      if (!province || !province.id) {
        return
      }
      this.$emit("isLoading", true);
      this.$store.dispatch("getCities", province.id).then((response) => {
        this.cities = response;
        this.$emit("isLoading", false);
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
          const newCity = await this.$store.dispatch('addCity', payload);
          this.cities.push(newCity);
          this.$refs.autoCompleteCities.setSelected(newCity);
          dialog.close();
        }
      })
    },
    async validateAddress() {
      return await this.$validator.validateAll();
    },
    openProvinceSettings() {
      this.showProvinceSettingsModal = true;
    },
    onProvinceSettingsSaved(settings) {
      this.provinceSelected.settings = settings;
      if (this.model.city && this.model.city.province) {
        this.model.city.province.settings = settings;
      }
      this.showProvinceSettingsModal = false;
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
