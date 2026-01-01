<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <h2 v-if="isUpdate" class="text-center main-title">Update Client</h2>
    <h2 v-else class="text-center main-title">Create Client</h2>
    <span class="line-orange"></span>
    <form class="form-md" @submit.prevent="validateForm">
      <div class="container-flex">
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <div class="container-image margin-10-auto">
            <upload-image @imageSelected="(img) => (company.logo.fileName = img)" :required="false"
              @onUpload="() => subscribe('file')" @finishUpload="() => unsubscribe()"></upload-image>
          </div>
        </div>
        <div v-if="isPayrollManager" class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field>
            <b-checkbox v-model="company.requiresPermissionToSeeOrders">
              Requires permission to see orders?
            </b-checkbox>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('full name') ? 'is-danger' : ''" :label="$t('CompanyFullName')"
            :message="errors.has('full name') ? errors.first('full name') : ''">
            <b-input type="text" v-model="company.fullName" name="full name" v-validate="'required|max:60|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('business name') ? 'is-danger' : ''" :label="$t('CompanyBusinessName')"
            :message="errors.has('business name') ? errors.first('business name') : ''">
            <b-input type="text" v-model="company.businessName" name="business name"
              v-validate="'required|max:50|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('industry') ? 'is-danger' : ''" :label="$t('CompanyTypeIndustry')"
            :message="errors.has('industry') ? errors.first('industry') : ''">
            <b-autocomplete v-model="industrySelected" :data="filteredIndustries" open-on-focus v-validate="'required'"
              ref="autoCompleteIndustries" name="industry" placeholder="Industry" selectable-footer
              @select="selectIndustry" @select-footer="addIndustry">
              <template #footer>
                <a><span> Add new... </span></a>
              </template>
              <template #empty>You don't have any industry created</template>
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('state') ? 'is-danger' : ''" label="Status"
            :message="errors.has('state') ? errors.first('state') : ''">
            <b-select v-model="company.companyStatus" placeholder="Select option" name="state" expanded
              v-validate="'required'">
              <option v-for="status in statuses" :key="status.id" :value="status.id">{{ status.value }}</option>
            </b-select>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
          <b-field :type="errors.has('salesRepresentative') ? 'is-danger' : ''" label="Sales Representative"
            :message="errors.has('salesRepresentative') ? errors.first('salesRepresentative') : ''">
            <b-autocomplete :data="filteredSalesRepresentative" :placeholder="$t('Select')"
              v-model="salesRepresentative" open-on-focus name="salesRepresentative" v-validate="'required'"
              :custom-formatter="(option) => `${option.name} - ${option.email}`"
              @select="onSalesRepresentativeSelected">
            </b-autocomplete>
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field :type="errors.has('about') ? 'is-danger' : ''" :label="$t('CompanyAbout')"
            :message="errors.has('about') ? errors.first('about') : ''">
            <b-input type="textarea" v-model="company.about" name="about" v-validate="'max:5000|min:2'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-12 col-lg-12 col-padding">
          <b-field label="Internal Info">
            <div class="vue-trix-editor">
              <div>
                <vue-editor id="internalRequirements-input" v-model="company.internalInfo"></vue-editor>
              </div>
            </div>
          </b-field>
        </div>
      </div>
      <h3 class="fz1 col-padding">{{ $t("CompanyContactInformation") }}</h3>
      <div class="container-flex">
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <phone-input :required="false" :defaultValue="company.phone" model="Phone"
            @formattedPhone="(phone) => (company.phone = phone)"></phone-input>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('phoneExt') ? 'is-danger' : ''" label="Phone Ext"
            :message="errors.has('phoneExt') ? errors.first('phoneExt') : ''">
            <b-input type="text" v-model="company.phoneExt" name="phoneExt" v-validate="'max:8|min:1|numeric'" />
          </b-field>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <phone-input :required="false" :defaultValue="company.fax" model="Fax"
            @formattedPhone="(phone) => (company.fax = phone)"></phone-input>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
          <b-field :type="errors.has('faxExt') ? 'is-danger' : ''" label="Fax Ext"
            :message="errors.has('faxExt') ? errors.first('faxExt') : ''">
            <b-input type="text" v-model="company.faxExt" name="faxExt" v-validate="'max:8|min:1|numeric'" />
          </b-field>
        </div>
        <div v-if="!isUpdate" class="col-sm-12 col-md-6 col-padding">
          <b-field :type="errors.has('email') ? 'is-danger' : ''" :label="$t('Email')"
            :message="errors.has('email') ? errors.first('email') : ''">
            <b-input type="email" v-model="company.email" name="email" v-validate="'required|max:50|min:6|email'" />
          </b-field>
        </div>
        <div v-if="displayPassword" class="col-sm-12 col-md-6 col-padding">
          <b-field :type="errors.has('password') ? 'is-danger' : ''" :label="$t('Password')"
            :message="errors.has('password') ? errors.first('password') : ''">
            <b-input type="password" v-model="company.password" name="password" v-validate="{
              required: displayPassword,
              max: 100,
              min: 6
            }" />
          </b-field>
        </div>
        <div
          :class="displayPassword ? 'col-sm-12 col-md-12 col-lg-12 col-padding' : 'col-sm-12 col-md-6 col-lg-6 col-padding'">
          <b-field :type="errors.has('website') ? 'is-danger' : ''" :label="$t('CompanyWebsite')"
            :message="errors.has('website') ? errors.first('website') : ''">
            <b-input type="text" v-model="company.website" name="website" v-validate="'max:50|url'"
              placeholder="www.example.com" />
          </b-field>
        </div>
        <div class="col-12 mt-5">
          <b-button v-if="isUpdate" type="is-primary" native-type="submit">Update</b-button>
          <b-button v-else type="is-primary" native-type="submit">{{ $t("Create") }}</b-button>
        </div>
      </div>
    </form>
  </div>
</template>

<script>
import pubSub from "../../mixins/pubSub";
import confirmationAlert from "../../mixins/confirmationAlert";
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  data() {
    return {
      isLoading: true,
      statuses: [],
      industryOptions: [],
      industrySelected: '',
      salesRepresentatives: [],
      salesRepresentative: '',
      company: {
        companyProfileId: null,
        logo: {},
        industry: {
          industry: null,
          otherIndustry: null,
        },
        requiresPermissionToSeeOrders: false,
      },
      submitted: false,
      isUpdate: false
    };
  },
  mixins: [
    pubSub,
    confirmationAlert,
    billingAdminMixin
  ],
  components: {
    UploadImage: () => import("@/components/PreviewImage"),
    phoneInput: () => import("@/components/PhoneInput"),
  },
  async created() {
    const company = this.$route.meta.company;
    if (company) {
      this.company = {
        ...company,
        companyProfileId: company.id
      }
      this.statuses = this.$route.meta.companyStatuses;
      this.industryOptions = this.$route.meta.industryList;
      this.salesRepresentatives = this.$route.meta.agencyPersonnel;
      this.isUpdate = true;
      if (company.industry.industry) {
        setTimeout(() => {
          this.$refs.autoCompleteIndustries.setSelected(company.industry.industry);
        });
      }
      let record = this.salesRepresentatives.find((sr) => sr.id === company.salesRepresentativeId);
      if (record) {
        this.salesRepresentative = `${record.name} - ${record.email}`;
      }
    } else {
      this.industryOptions = await this.$store.dispatch('getCompanyIndustry');
      this.statuses = await this.$store.dispatch('getCompanyStatus');
      this.salesRepresentatives = await this.$store.dispatch("agency/getAgencyPersonnel")
    }
    this.isLoading = false;
  },
  methods: {
    validateForm() {
      this.submitted = true;
      this.$validator.validateAll().then((result) => {
        if (result) {
          if (this.isUpdate) {
            this.updateCompany();
          } else {
            this.createCompany();
          }
          return;
        }
        this.showAlertError(this.$t("PleaseVerifyThatTheFieldsAreCorrect"));
      });
    },
    createCompany() {
      this.isLoading = true;
      this.$store.dispatch("agency/createCompany", this.company)
        .then((response) => {
          this.isLoading = false;
          this.showAlertSuccess(this.$t("CompanyCreated"));
          this.$router.push("/agency-companies/company/" + response.data.id);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error.data);
        });
    },
    updateCompany() {
      this.isLoading = true;
      this.$store.dispatch("agency/updateCompany", { companyId: this.company.companyProfileId, company: this.company })
        .then((response) => {
          this.isLoading = false;
          this.showAlertSuccess("Company updated");
          this.$router.push("/agency-companies/company/" + response.data.id);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error.data);
        })
    },
    onSalesRepresentativeSelected(option) {
      if (option) {
        this.company.salesRepresentativeId = option.id;
      } else {
        this.company.salesRepresentativeId = null;
      }
    },
    selectIndustry(option) {
      if (option) {
        this.company.industry.industry = option;
      } else {
        this.company.industry = null;
      }
    },
    addIndustry() {
      this.$buefy.dialog.prompt({
        message: `Industry`,
        inputAttrs: {
          placeholder: 'Industry',
          maxlength: 100,
          value: this.industrySelected,
        },
        closeOnConfirm: false,
        confirmText: 'Add',
        onConfirm: async (value, dialog) => {
          const payload = { value };
          const newIndustry = await this.$store.dispatch('addIndustry', payload);
          this.industryOptions.push(newIndustry);
          this.$refs.autoCompleteIndustries.setSelected(newIndustry);
          dialog.close();
        }
      })
    }
  },
  computed: {
    filteredIndustries() {
      const industries = this.industryOptions.filter((option) => option.value.toLowerCase().includes(this.industrySelected.toLowerCase()));
      return industries;
    },
    filteredSalesRepresentative() {
      const salesRepresentatives = this.salesRepresentatives
        .filter(sr => `${sr.name} - ${sr.email}`.toLowerCase().includes(this.salesRepresentative.toLowerCase()));
      return salesRepresentatives;
    },
    displayPassword() {
      return !this.isUpdate && this.company.companyStatus === 5;
    }
  }
};
</script>
