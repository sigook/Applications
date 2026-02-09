<template>
  <div class="profile white-container-mobile profile-worker">
    <b-loading v-model="isLoading"></b-loading>

    <!-- Agency Profile Selector -->
    <div class="worker-profile-header">
      <!-- Profile Top -->
      <div class="profile-top" v-if="workerProfile">
        <div class="max-width-158">
          <image-detail :data="workerProfile" @updateProfile="() => updateProfile()" />
        </div>
        <div>
          <h1 class="capitalize fz1">
            {{ workerProfile.firstName | lowercase }}
            {{ workerProfile.middleName | lowercase }}
            {{ workerProfile.lastName | lowercase }}
          </h1>
          <p v-if="workerProfile.location" class="fz-1">
            <b>Id:</b>{{ workerProfile.numberId }} <br />
            <b>Phone:</b>{{ workerProfile.mobileNumber }}
          </p>
        </div>
      </div>
    </div>

    <!-- Buefy Tabs -->
    <b-tabs v-model="currentTab" @input="changeTab" v-if="workerProfile">
      <b-tab-item value="PersonalDetails">
        <template #header>
          <span>Personal Details</span>
          <b-icon v-if="hasPersonalDetailsMissing" icon="alert-circle" size="is-small" type="is-danger" class="ml-1" />
        </template>
        <PersonalDetails v-if="visitedTabs.includes('PersonalDetails')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item value="SkillsInfo">
        <template #header>
          <span>Skills & Languages</span>
          <b-icon v-if="hasSkillsMissing" icon="alert-circle" size="is-small" type="is-danger" class="ml-1" />
        </template>
        <SkillsInfo v-if="visitedTabs.includes('SkillsInfo')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Licenses & Certifications" value="LicensesInfo">
        <LicensesInfo v-if="visitedTabs.includes('LicensesInfo')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Work Experience" value="WorkExperience">
        <WorkExperience v-if="visitedTabs.includes('WorkExperience')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item value="AdditionalInfo">
        <template #header>
          <span>Additional Info</span>
          <b-icon v-if="hasAdditionalInfoMissing" icon="alert-circle" size="is-small" type="is-danger" class="ml-1" />
        </template>
        <AdditionalInfo v-if="visitedTabs.includes('AdditionalInfo')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Comments" value="Comments">
        <Comments v-if="visitedTabs.includes('Comments')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Account Security" value="AccountSecurity">
        <WorkerAccountSecurity v-if="visitedTabs.includes('AccountSecurity')" />
      </b-tab-item>

      <b-tab-item label="Notifications" value="UserNotification">
        <UserNotification v-if="visitedTabs.includes('UserNotification')" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script>
import switchLocaleMixin from "../../mixins/switchLocaleMixin";
import confirmationAlert from "../../mixins/confirmationAlert";
import qrCodeMixin from "../../mixins/qrCodeMixin";

export default {
  components: {
    PersonalDetails: () => import("../../components/worker/ProfilePersonal"),
    SkillsInfo: () => import("../../components/worker/ProfileSkills"),
    LicensesInfo: () => import("../../components/worker/ProfileLicenses"),
    AdditionalInfo: () => import("../../components/worker/ProfileAdditionalInfo"),
    WorkExperience: () => import("../../components/worker/ProfileExperience"),
    Comments: () => import("../../components/worker/ProfileComments"),
    WorkerAccountSecurity: () => import("../../components/worker/WorkerAccountSecurity"),
    UserNotification: () => import("../../components/UserNotification"),
    imageDetail: () => import("../../components/worker/WorkImageDetail"),
  },
  data() {
    return {
      dropdownAgency: false,
      currentTab: "PersonalDetails",
      visitedTabs: ["PersonalDetails"],
      isLoading: true,
      dropdownOptions: false,
      lang: this.$validator.dictionary.locale,
    };
  },
  methods: {
    changeProfile(profile) {
      this.dropdownAgency = false;
      if (profile !== this.profileSelected) {
        this.isLoading = true;
        this.$store.commit("worker/setProfileSelected", profile);
        this.$store.dispatch("worker/getProfile", profile.id)
          .then((response) => {
            this.getProvinces(response.location.province.id);
            this.isLoading = false;
          })
          .catch((error) => {
            this.showAlertError(error);
            this.isLoading = false;
          });
      }
    },
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: "/worker-profile",
        query: { tab: tab },
      });
    },
    updateProfile() {
      this.isLoading = true;
      this.$store
        .dispatch("worker/getProfile", this.workerProfile.id)
        .then(() => {
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
  },
  computed: {
    profileSelected() {
      return this.$store.state.worker.profileSelected;
    },
    workerProfiles() {
      return this.$store.state.worker.workerProfiles;
    },
    workerProfile() {
      return this.$store.state.worker.workerProfile;
    },
    hasPersonalDetailsMissing() {
      if (!this.workerProfile) return false;
      return !this.workerProfile.socialInsurance
        || !this.workerProfile.socialInsuranceFile
        || !this.workerProfile.identificationType1File
        || !this.workerProfile.identificationType2File
        || !this.workerProfile.resume;
    },
    hasSkillsMissing() {
      if (!this.workerProfile) return false;
      return this.workerProfile.skills && this.workerProfile.skills.length === 0;
    },
    hasAdditionalInfoMissing() {
      if (!this.workerProfile) return false;
      const wp = this.workerProfile;
      return (wp.availabilities && wp.availabilities.length === 0)
        || (wp.availabilityTimes && wp.availabilityTimes.length === 0)
        || (wp.availabilityDays && wp.availabilityDays.length === 0)
        || (wp.locationPreferences && wp.locationPreferences.length === 0)
        || !wp.contactEmergencyPhone;
    },
  },
  mixins: [switchLocaleMixin, confirmationAlert, qrCodeMixin],
  created() {
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
    this.$store.dispatch("worker/getProfiles")
      .then((response) => {
        this.$store.commit("worker/setProfileSelected", response[0]);
        this.$store.dispatch("worker/getProfile", response[0].id)
          .then((response) => {
            this.getProvinces(response.location.province.id);
            this.isLoading = false;
          })
          .catch((error) => {
            this.showAlertError(error);
            this.isLoading = false;
          });
      })
      .catch((error) => {
        this.showAlertError(error);
        this.isLoading = false;
      });
  },
};
</script>

<style lang="scss">
.profile-worker section.focus {
  transition: 1s;
  background-color: #ffefdd;
}

.profile-worker {
  .profile-information .section-title {
    margin-bottom: 0;
  }

  section:not(.worker-comments) {
    padding: 10px 16px;
    border: 1px solid #ddd;
    margin: 16px 0;
    border-radius: 5px;
    box-shadow: 1px 1px 5px #e9e9e9;
    transition: 1s;
  }

  .worker-documents>div:nth-of-type(1) {
    margin-top: 15px;
  }

  .worker-documents>div.worker-skills.margin-top-15,
  .worker-skills>div {
    margin: 0;
  }

  .worker-skills>div span {
    margin-top: 16px;
  }

  section.missing {
    box-shadow: 1px 2px 4px #ffabab;
    border-color: #ad0715;

    .section-title,
    .detail-worker-profile .width-30,
    h3 {
      color: #cf1a2b;

      &:before {
        content: "";
        width: 16px;
        height: 16px;
        display: inline-block;
        vertical-align: middle;
        margin-right: 10px;
        background-image: url("../../assets/images/danger.png");
        background-size: contain;
        position: relative;
        top: -1px;
      }
    }
  }
}

.worker-profile-header {
  margin-bottom: 20px;
}

.contain-profile .profile-selected {
  background: transparent;
  border: 0;
  border-bottom: 1px solid #eee;
  padding: 0 0 15px;
  margin-bottom: 15px;
}

@media (max-width: 767px) {
  .profile-worker .button-right {
    position: relative;
    display: flex;

    button {
      margin: 0;
    }

    .md-btn {
      padding: 5px 12px;
    }
  }
}
</style>
