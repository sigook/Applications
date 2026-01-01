<template>
  <div class="profile white-container-mobile profile-worker">
    <b-loading v-model="isLoading"></b-loading>
    <!-- SIDE BAR MENU -->
    <nav class="user-profile-menu">
      <div class="contain-profile">
        <div class="profile-selected" @click="dropdownAgency = !dropdownAgency">
          <img :src="profileSelected.agencyLogo" alt="logo agency" />
          <span :class="{ 'no-arrow': workerProfiles.length === 1 }">
            {{ profileSelected.agencyFullName }}
          </span>
        </div>
        <transition name="fadeHeight">
          <div v-if="dropdownAgency && workerProfiles.length > 1" class="dropdown">
            <div v-for="item in workerProfiles" :key="'workerprofiles' + item.agencyFullName" class="item"
              @click="changeProfile(item)">
              <img :src="item.agencyLogo" alt="logo agency" />
              {{ item.agencyFullName }}
            </div>
          </div>
        </transition>
      </div>
      <div class="only-sm button-toggle-menu-mobile" @click="toggleMenuMobile = !toggleMenuMobile">
        {{ currentTab.name }}
        <img src="../../assets/images/arrow-down.svg" alt="menu" :class="{ up: toggleMenuMobile }" />
      </div>
      <div class="toggle-menu-mobile" v-show="toggleMenuMobile">
        <a v-for="tab in tabs" :key="'tabs' + tab.tab"
          v-bind:class="['tab-button', { active: currentTab.name === tab.name }]">
          <span class="fw-700 block" v-on:click="changeTab(tab)">{{ tab.name }}</span>
          <ul>
            <li v-for="item in tab.children" :class="{ missing: item.missing }" @click="scrollTo(tab, item.name)"
              :key="'childrentab' + item.name">
              {{ item.name }}
            </li>
          </ul>
        </a>
      </div>

      <div class="options-profile">
        <p @click="dropdownOptions = !dropdownOptions" :class="{ 'dropdown-open': dropdownOptions }">
          <span>▶</span> {{ $t("Options") }}
        </p>
        <transition name="fade">
          <div class="dropdown-rol" v-if="dropdownOptions">
            <b class="fz-1">{{ $t("ChangeLanguage") }}:</b>
            <div class="navbar-languages">
              <span v-on:click="switchLocale('en')" :class="lang === 'en' ? 'active' : ''">EN</span>
              <span v-on:click="switchLocale('fr')" :class="lang === 'fr' ? 'active' : ''">FR</span>
              <span v-on:click="switchLocale('es')" :class="lang === 'es' ? 'active' : ''">ES</span>
            </div>
          </div>
        </transition>
      </div>
    </nav>

    <div class="profile-content" v-if="workerProfile">
      <div class="profile-top">
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
          <button class="download-qr-button mt-2" @click="getQRCode(workerProfile.workerId)">
            <img src="../../assets/images/qr-code-orange.png" alt="Qr code icon" width="20px"
              class="d-inline-block v-middle" />
            <span class="d-inline-block v-middle ml-2">Qr Code</span>
          </button>
          <worker-qr :worker="workerProfile" :qr-image="QrImage"></worker-qr>
          <p class="color-primary fw-400"></p>
        </div>
      </div>
      <component v-if="workerProfile" v-bind:is="currentTabComponent" class="tab" :worker="workerProfile" />
    </div>
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
    UploadImage: () => import("../../components/PreviewImage"),
    AccountSecurity: () => import("../../components/agency/ProfileAccountInformation"),
    UserNotification: () => import("../../components/UserNotification"),
    imageDetail: () => import("../../components/worker/WorkImageDetail"),
    WorkerQr: () => import("../../components/WorkerPunchCardPrint"),
  },
  data() {
    return {
      dropdownAgency: false,
      currentTab: {
        tab: "PersonalDetails",
        name: "Personal Details",
      },
      isLoading: true,
      dropdownOptions: false,
      lang: this.$validator.dictionary.locale,
      hideEditButton: false,
      toggleMenuMobile: true,
    };
  },
  methods: {
    titleTab(val) {
      return val.split(/(?=[A-Z])/).join(" ");
    },
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
      this.currentTab = tab;
      if (this.$store.state.isMobile) this.toggleMenuMobile = false;
    },
    scrollTo(tab, value) {
      if (tab === this.currentTab) {
        let anchor = value.toLowerCase().replace(/\s/g, "");
        let element = document.getElementById(anchor);
        element.scrollIntoView();
        element.classList.add("focus");
        setTimeout(function () {
          element.classList.remove("focus");
        }, 1000);
      }

      this.currentTab = tab;
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
    currentTabComponent() {
      return this.currentTab.tab;
    },
    profileSelected() {
      return this.$store.state.worker.profileSelected;
    },
    workerProfiles() {
      return this.$store.state.worker.workerProfiles;
    },
    workerProfile() {
      return this.$store.state.worker.workerProfile;
    },
    worker() {
      return this.$store.state.worker.workerProfile;
    },
    tabs() {
      return [
        {
          tab: "PersonalDetails",
          name: "Personal Details",
          children: [
            {
              name: "Basic Information",
              missing: false,
            },
            {
              name: "Contact information",
              missing: false,
            },
            {
              name: "Social Insurance",
              missing:
                !this.$store.state.worker.workerProfile.socialInsurance ||
                !this.$store.state.worker.workerProfile.socialInsuranceFile,
            },
            {
              name: "Documents",
              missing:
                !this.$store.state.worker.workerProfile
                  .identificationType1File ||
                !this.$store.state.worker.workerProfile.identificationType2File,
            },
            {
              name: "Resume",
              missing: !this.$store.state.worker.workerProfile.resume,
            },
          ],
        },
        {
          tab: "SkillsInfo",
          name: "Skills & Languages",
          children: [
            {
              name: "Skills",
              missing:
                this.$store.state.worker.workerProfile.skills &&
                this.$store.state.worker.workerProfile.skills.length === 0,
            },
            {
              name: "Languages",
              missing: false,
            },
          ],
        },
        {
          tab: "LicensesInfo",
          name: "Licenses & Certifications",
        },
        {
          tab: "WorkExperience",
          name: "Work Experience",
        },
        {
          tab: "AdditionalInfo",
          name: "Additional Info",
          children: [
            {
              name: "Lift",
              missing: false,
            },
            {
              name: "Availability",
              missing:
                this.$store.state.worker.workerProfile.availabilities &&
                this.$store.state.worker.workerProfile.availabilities.length ===
                0,
            },
            {
              name: "Available Time",
              missing:
                this.$store.state.worker.workerProfile.availabilityTimes &&
                this.$store.state.worker.workerProfile.availabilityTimes
                  .length === 0,
            },
            {
              name: "Available days",
              missing:
                this.$store.state.worker.workerProfile.availabilityDays &&
                this.$store.state.worker.workerProfile.availabilityDays
                  .length === 0,
            },
            {
              name: "Location preferences",
              missing:
                this.$store.state.worker.workerProfile.locationPreferences &&
                this.$store.state.worker.workerProfile.locationPreferences
                  .length === 0,
            },
            {
              name: "Emergency information",
              missing:
                !this.$store.state.worker.workerProfile.contactEmergencyPhone,
            },
          ],
        },
        {
          tab: "Comments",
          name: "Comments",
        },
        {
          tab: "AccountSecurity",
          name: "Account Security",
        },
        {
          tab: "UserNotification",
          name: "Notifications",
        },
      ];
    },
  },
  mixins: [switchLocaleMixin, confirmationAlert, qrCodeMixin],
  created() {
    this.$store.state.isMobile
      ? (this.toggleMenuMobile = false)
      : (this.toggleMenuMobile = true);
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

  .profile-menu {
    a {
      font-size: 16px;
      height: auto;
      margin-bottom: 10px;
      font-weight: normal;

      ul li {
        padding-left: 5px;
        margin: 10px 0 0;
        font-weight: lighter;
      }
    }
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

// NEW PROFILE FOR WORKER
.profile-worker {
  .profile-content {
    border-left: 0;
  }
}

.user-profile-menu {
  margin-left: -30px;
  background: #fbfbfb;
  margin-top: -30px;
  padding: 20px;
  margin-bottom: -30px;

  a:hover {
    text-decoration: none;
  }

  ul {
    margin-bottom: 20px;
    padding-left: 15px;

    li {
      margin-top: 5px;
      border-bottom: 1px solid #dddddd;
      padding-bottom: 5px;
      margin-bottom: 5px;

      &.missing {
        position: relative;

        &:before {
          content: "";
          width: 13px;
          height: 13px;
          background-image: url("../../assets/images/danger.png");
          background-size: contain;
          position: absolute;
          left: -18px;
          top: 4px;
        }
      }
    }
  }
}

.contain-profile .profile-selected {
  background: transparent;
  border: 0;
  border-bottom: 1px solid white;
  padding: 0 0 15px;
  margin-bottom: 15px;
}

@media (max-width: 767px) {
  .user-profile-menu {
    margin: -15px -15px 20px;
    background: #f3f3f3;
    padding: 0;

    ul {
      display: none;
    }

    .tab-button {
      display: block;
      padding: 0;

      span {
        padding: 12px 0;
        font-weight: normal;
        display: block;
        margin: 0;
      }
    }

    .options-profile {
      display: none;
    }
  }

  .contain-profile {
    display: none;
  }

  .button-toggle-menu-mobile {
    font-weight: bold;
    padding: 10px 20px;
    box-shadow: 0 1px 5px #afafaf;

    img {
      width: 20px;
      height: 20px;
      display: inline-block;
      float: right;
      position: relative;
      top: 2px;
    }
  }

  .toggle-menu-mobile {
    padding: 0 20px;
    border-bottom: 1px solid #cfcfcf;
  }

  img.up {
    transform: rotate(180deg);
  }

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