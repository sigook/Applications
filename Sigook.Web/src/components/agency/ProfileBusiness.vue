<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Agency Name" :type="errors.has('agency name') ? 'is-danger' : ''"
          :message="errors.has('agency name') ? errors.first('agency name') : ''">
          <b-input v-model="localAgencyData.fullName" v-validate="'required|max:50|min:2'" name="agency name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Agency HST Number" :type="errors.has('hts #') ? 'is-danger' : ''"
          :message="errors.has('hts #') ? errors.first('hts #') : ''">
          <b-input v-model="localAgencyData.hstNumber" v-validate="'max:20|min:15'" name="hts #" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="BN/EIN" :type="errors.has('business #') ? 'is-danger' : ''"
          :message="errors.has('business #') ? errors.first('business #') : ''">
          <b-input v-model="localAgencyData.businessNumber" v-validate="'required|max:15|min:9'" name="business #" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <phone-input ref="phoneComponent" :required="true" :defaultValue="localAgencyData.phonePrincipal"
          model="Phone Principal" @formattedPhone="(phone) => (localAgencyData.phonePrincipal = phone)">
        </phone-input>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <b-field label="Ext" :type="errors.has('phonePrincipalExt') ? 'is-danger' : ''"
          :message="errors.has('phonePrincipalExt') ? errors.first('phonePrincipalExt') : ''">
          <b-input v-model="localAgencyData.phonePrincipalExt" v-validate="'max:8|min:1|numeric'" name="phonePrincipalExt" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Web Page" :type="errors.has('web page') ? 'is-danger' : ''"
          :message="errors.has('web page') ? errors.first('web page') : ''">
          <b-input v-model="localAgencyData.webPage" v-validate="'max:50|url'" name="web page" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <phone-input ref="faxComponent" :required="false" :defaultValue="localAgencyData.fax" model="Fax Number"
          @formattedPhone="(phone) => (localAgencyData.fax = phone)">
        </phone-input>
      </div>
      <div class="col-sm-12 col-md-3 col-lg-3 col-padding">
        <b-field label="Ext" :type="errors.has('faxExt') ? 'is-danger' : ''"
          :message="errors.has('faxExt') ? errors.first('faxExt') : ''">
          <b-input v-model="localAgencyData.faxExt" v-validate="'max:8|min:1|numeric'" name="faxExt" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="WSIB Group" :type="errors.has('wsibGroup') ? 'is-danger' : ''"
          :message="errors.has('wsibGroup') ? errors.first('wsibGroup') : ''">
          <b-taginput v-model="localAgencyData.wsibGroup" autocomplete :data="wsibGroups" open-on-focus field="value"
            icon="label" :placeholder="$t('ClickHereToAddMore')" :before-adding="beforeWsibBeingSelected">
          </b-taginput>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="validateForm">SAVE</b-button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { getWsibGroups } from "@/api/catalogApi";
import { updateAgency } from "@/api/agencyApi";
export default {
  data() {
    return {
      isLoading: false,
      localAgencyData: JSON.parse(JSON.stringify(this.agencyData)),
      wsibGroups: [],
    };
  },
  watch: {
    agencyData: {
      handler(newVal) {
        this.localAgencyData = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  props: ["agencyData"],
  components: {
    phoneInput: () => import("@/components/PhoneInput.vue"),
  },
  methods: {
    async validateForm() {
      const phoneValid = await this.$refs.phoneComponent.validatePhone();
      const faxValid = await this.$refs.faxComponent.validatePhone();
      const valid = await this.$validator.validateAll();
      if (phoneValid && faxValid && valid) {
        this.isLoading = true;
        this.$emit('update:agencyData', this.localAgencyData);
        updateAgency(this.localAgencyData)
          .then(() => {
            this.isLoading = false;
            this.showAlertSuccess(this.$t("Updated"));
          })
      }
    },
    beforeWsibBeingSelected(tag) {
      return !this.localAgencyData.wsibGroup.some(item => item.value === tag.value);
    }
  },
  created() {
    this.isLoading = true;
    getWsibGroups()
      .then((response) => {
        this.isLoading = false;
        this.wsibGroups = response;
      })
      .catch(() => {
        this.isLoading = false;
      });
  }
}
</script>
