<template>
  <div class="profile white-container-mobile profile-worker">
    <b-loading v-model="isLoading"></b-loading>

    <div class="profile-content">
      <!-- Profile Top -->
      <div class="profile-top" v-if="workerProfile">
        <div>
          <image-detail :data="workerProfile" @updateProfile="() => updateProfile()" />
        </div>
        <div>
          <h1 class="capitalize">
            {{ workerProfile.firstName | lowercase }}
            {{ workerProfile.middleName | lowercase }}
            {{ workerProfile.lastName | lowercase }}
          </h1>
          <p v-if="workerProfile.numberId">
            <b-icon icon="card-account-details-outline" size="is-small" />
            {{ workerProfile.numberId }}
          </p>
          <p v-if="workerProfile.mobileNumber">
            <b-icon icon="phone" size="is-small" />
            {{ workerProfile.mobileNumber }}
          </p>
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

      <b-tab-item label="Work Experience" value="WorkExperience">
        <WorkExperience v-if="visitedTabs.includes('WorkExperience')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Preferences" value="Preferences">
        <Preferences v-if="visitedTabs.includes('Preferences')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Comments" value="Comments">
        <Comments v-if="visitedTabs.includes('Comments')" :worker="workerProfile" />
      </b-tab-item>

      <b-tab-item label="Account" value="AccountSecurity">
        <WorkerAccountSecurity v-if="visitedTabs.includes('AccountSecurity')" />
      </b-tab-item>
    </b-tabs>
    </div>
  </div>
</template>

<script>
import switchLocaleMixin from "../../mixins/switchLocaleMixin";
import confirmationAlert from "../../mixins/confirmationAlert";

export default {
  components: {
    PersonalDetails: () => import("../../components/worker/ProfilePersonal"),
    Preferences: () => import("../../components/worker/ProfilePreferences"),
    WorkExperience: () => import("../../components/worker/ProfileExperience"),
    Comments: () => import("../../components/worker/ProfileComments"),
    WorkerAccountSecurity: () => import("../../components/worker/WorkerAccountSecurity"),
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
  },
  mixins: [switchLocaleMixin, confirmationAlert],
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
  display: block;

  .profile-content {
    width: 100%;
    border-left: none;
    padding: 15px 20px;
  }

  .profile-top {
    .worker-profile-image {
      max-width: 158px;

      img {
        max-width: 100%;
        border-radius: 5px;
      }
    }
  }

  .profile-information .section-title {
    margin-bottom: 0;
    color: inherit;
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
