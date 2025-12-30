<template>
  <div class="company-wrapper white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>

    <section class="company-top" v-if="company">
      <div class="hover-actions">
        <img v-if="company.logo" :src="company.logo.pathFile" alt="logo" />
        <button class="actions btn-icon-sm btn-icon-edit" type="button" @click="showUpdateLogo = true">
          Edit
        </button>
        <h2 class="capitalize fz1 fw-700">
          <span class="fw-400 fz-1" v-if="company.numberId">{{ company.numberId }} |
          </span>
          {{ company.businessName | lowercase }}
        </h2>
      </div>

      <floating-menu>
        <template slot="options">
          <button v-if="isClient" class="floating-menu-item"
            @click="$router.push({ path: `/agency-create-request/${company.id}` })">
            Create Order
          </button>
          <button class="floating-menu-item" @click="$router.push({ path: `/update-company/${company.id}` })">
            <span>Edit Company</span>
          </button>
        </template>
      </floating-menu>
    </section>

    <b-tabs v-model="currentTab" @input="changeTab" v-if="company">
      <b-tab-item label="Detail" value="Detail">
        <detail v-if="visitedTabs.includes('Detail')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Settings" value="Settings" v-if="isPayrollManager">
        <settings v-if="visitedTabs.includes('Settings')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Users" value="Users">
        <users v-if="visitedTabs.includes('Users')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Contacts" value="ContactPerson">
        <contact-person v-if="visitedTabs.includes('ContactPerson')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Roles" value="JobPosition" v-if="!requiresPayrollPermission">
        <job-position v-if="visitedTabs.includes('JobPosition')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Workers" value="Workers">
        <workers v-if="visitedTabs.includes('Workers')" :company="company" class="p-2" />
      </b-tab-item>
      <b-tab-item label="Orders" value="Requests" v-if="!requiresPayrollPermission">
        <requests v-if="visitedTabs.includes('Requests')" :company="company" class="p-2" />
      </b-tab-item>
    </b-tabs>
    
    <!-- update logo -->
    <company-update-logo v-if="showUpdateLogo" :logo="company.logo" v-on:save="updateLogo"
      v-on:cancel="showUpdateLogo = false" />
    <!-- update logo -->
  </div>
</template>

<script>
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  data() {
    return {
      currentTab: "Detail",
      visitedTabs: ["Detail"],
      company: null,
      isLoading: true,
      showMenuTop: false,
      editNameModal: false,
      showUpdateLogo: false,
    };
  },
  components: {
    Detail: () => import("@/components/agency_company/CompanyDetailTab"),
    Settings: () => import("@/components/agency_company/CompanySettings"),
    Users: () => import("@/components/agency_company/UserList"),
    ContactPerson: () => import("@/components/agency_company/ContactPersonList"),
    JobPosition: () => import("@/components/agency_company/JobPositionList"),
    Requests: () => import("@/components/agency_company/CompanyRequests"),
    Workers: () => import("@/components/agency_company/CompanyWorkers"),
    FloatingMenu: () => import("@/components/FloatingMenuDots"),
    EditTextarea: () => import("@/components/agency_request/EditTextarea"),
    CompanyUpdateLogo: () => import("@/components/agency_company/CompanyUpdateLogo")
  },
  mixins: [billingAdminMixin],
  methods: {
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/agency-companies/company/${this.$route.params.id}`,
        query: {
          tab: tab,
        },
      });
    },
    getCompany() {
      this.$store
        .dispatch("agency/getCompany", this.$route.params.id)
        .then((response) => {
          this.company = response;
          this.isLoading = false;
        })
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    updateLogo(newLogo) {
      this.showUpdateLogo = false;
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyCompanyProfileLogo", {
        profileId: this.company.id,
        model: newLogo,
      })
        .then(() => {
          this.isLoading = false;
          this.company.logo.pathFile = this.company.logo.pathFile.replace(
            this.company.logo.fileName,
            newLogo.fileName
          );
          this.company.logo.fileName = newLogo.fileName;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
  created() {
    this.getCompany();
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
  },
  computed: {
    requiresPayrollPermission() {
      if (this.company && this.company.requiresPermissionToSeeOrders) {
        return !this.isPayrollManager;
      } else {
        return false;
      }
    },
    isClient() {
      return this.company && this.company.companyStatus === 5;
    }
  },
};
</script>
