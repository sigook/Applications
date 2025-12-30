<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Company Business Name" :type="errors.has('business name') ? 'is-danger' : ''"
          :message="errors.has('business name') ? errors.first('business name') : ''">
          <b-input v-model="companyData.businessName" v-validate="'required|max:50|min:2'" name="business name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Company Full Name" :type="errors.has('full name') ? 'is-danger' : ''"
          :message="errors.has('full name') ? errors.first('full name') : ''">
          <b-input v-model="companyData.fullName" v-validate="'required|max:50|min:2'" name="full name" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Industry">
          <b-autocomplete ref="industryComponent" v-model="industrySelected" :data="filteredIndustries"
            :placeholder="$t('SelectIndustry')" open-on-focus @select="selectIndustry">
          </b-autocomplete>
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <phone-input ref="phoneComponent" :required="true" model="Phone" :defaultValue="companyData.phone"
          @formattedPhone="(phone) => companyData.phone = phone"></phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <b-field label="Phone Ext" :type="errors.has('phoneExt') ? 'is-danger' : ''"
          :message="errors.has('phoneExt') ? errors.first('phoneExt') : ''">
          <b-input type="text" v-model="companyData.phoneExt" name="phoneExt" v-validate="'max:8|min:1|numeric'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
        <b-field label="Website" :type="errors.has('website') ? 'is-danger' : ''"
          :message="errors.has('website') ? errors.first('website') : ''">
          <b-input type="text" v-model="companyData.website" name="website" v-validate="'max:50|url'" />
        </b-field>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <phone-input ref="faxComponent" :required="false" model="Fax" :defaultValue="companyData.fax"
          @formattedPhone="(phone) => companyData.fax = phone"></phone-input>
      </div>
      <div class="col-sm-12 col-md-6 col-lg-3 col-padding">
        <b-field label="Fax Ext" :type="errors.has('faxExt') ? 'is-danger' : ''"
          :message="errors.has('faxExt') ? errors.first('faxExt') : ''">
          <b-input type="text" v-model="companyData.faxExt" name="faxExt" v-validate="'max:8|min:1|numeric'" />
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="save">Save</b-button>
      </div>
    </div>
  </div>
</template>

<script>


export default {
  props: ['companyData'],
  data() {
    return {
      isLoading: false,
      industries: [],
      industrySelected: '',
    }
  },
  components: {
    phoneInput: () => import("../../components/PhoneInput"),
  },
  methods: {
    selectIndustry(option) {
      if (option) {
        this.companyData.industry.industry = option;
      } else {
        this.companyData.industry = null;
      }
    },
    async save() {
      const phoneValid = await this.$refs.phoneComponent.validatePhone();
      const faxValid = await this.$refs.faxComponent.validatePhone();
      const mainFormValid = await this.$validator.validateAll();
      if (mainFormValid && phoneValid && faxValid) {
        this.isLoading = true;
        this.$store.dispatch('company/updateProfile', { id: this.companyData.id, company: this.companyData })
          .then((response) => {
            this.isLoading = false;
            this.showAlertSuccess('Profile updated');
          })
          .catch((error) => {
            this.isLoading = false;
            this.showAlertError(error.data);
          });
      }
    }
  },
  async created() {
    this.industries = await this.$store.dispatch('getCompanyIndustry');
    setTimeout(() => {
      this.$refs.industryComponent.setSelected(this.companyData.industry.industry);
    });
  },
  computed: {
    filteredIndustries() {
      return this.industries.filter((industry) => industry.value.toLowerCase().includes(this.industrySelected.toLowerCase()));
    }
  }
}
</script>

<style lang="scss">
.profile-information>div.profile-100 {
  display: block;

  b,
  p {
    display: block;
    width: 100%;
  }

  input {
    width: 100%;
    max-width: 100px;

  }

  input[disabled="disabled"] {
    padding: 0;
  }

  .job-rates {
    width: 100%;
    margin-top: 10px;

    table {
      width: 100%;
      border-collapse: collapse;

      tr {
        transition: .3s all ease;
      }

      tr:hover {
        background-color: #f7f7f7;

      }

      td {
        padding: 5px 15px;
        font-size: 14px;
      }
    }
  }
}

.padding-left span {

  padding-left: 5px;
}
</style>