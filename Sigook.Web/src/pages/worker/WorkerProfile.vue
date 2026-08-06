<template>
  <div class="profile profile-worker">
    <b-loading v-model="isLoading"></b-loading>

    <div class="profile-content">
      <!-- Profile Top -->
      <div class="profile-top" v-if="workerProfile">
        <div>
          <ImageDetail :data="workerProfile" @updateProfile="() => updateProfile()" />
        </div>
        <div>
          <h1 class="is-capitalized">
            {{ lowercase(workerProfile.firstName) }}
            {{ lowercase(workerProfile.middleName) }}
            {{ lowercase(workerProfile.lastName) }}
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
      <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="workerProfile">
        <b-tab-item value="PersonalDetails">
          <template #header>
            <span>Personal Details</span>
            <b-icon v-if="hasPersonalDetailsMissing" icon="alert-circle" size="is-small" type="is-danger" class="ml-1" />
          </template>
          <PersonalDetails v-if="visitedTabs.includes('PersonalDetails')" :worker="workerProfile" @updateProfile="updateProfile()" />
        </b-tab-item>

        <b-tab-item label="Work Experience" value="WorkExperience">
          <WorkExperience v-if="visitedTabs.includes('WorkExperience')" :worker="workerProfile" @updateProfile="updateProfile()" />
        </b-tab-item>

        <b-tab-item label="Preferences" value="Preferences">
          <Preferences v-if="visitedTabs.includes('Preferences')" :worker="workerProfile" @updateProfile="updateProfile()" />
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

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useWorkerStore } from '@/stores/worker';
import { showAlertError } from '@/utils/toast';
import { getMyProfile } from '@/api/workerApi';
import { lowercase } from '@/utils/filters';
import PersonalDetails from '../../components/worker/ProfilePersonal.vue';
import Preferences from '../../components/worker/ProfilePreferences.vue';
import WorkExperience from '../../components/worker/ProfileExperience.vue';
import Comments from '../../components/worker/ProfileComments.vue';
import WorkerAccountSecurity from '../../components/worker/WorkerAccountSecurity.vue';
import ImageDetail from '../../components/worker/WorkImageDetail.vue';

const route = useRoute();
const router = useRouter();
const workerStore = useWorkerStore();

const currentTab = ref<string>('PersonalDetails');
const visitedTabs = ref<string[]>(['PersonalDetails']);
const isLoading = ref(false);

const workerProfile = computed<any>(() => workerStore.workerProfile);

const hasPersonalDetailsMissing = computed(() => {
  if (!workerProfile.value) return false;
  return !workerProfile.value.socialInsurance
    || !workerProfile.value.socialInsuranceFile
    || !workerProfile.value.identificationType1File
    || !workerProfile.value.identificationType2File
    || !workerProfile.value.resume;
});

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: '/worker-profile',
    query: { tab: tab },
  });
}

function getProfile() {
  isLoading.value = true;
  getMyProfile()
    .then((data: any) => {
      workerStore.setWorkerProfile(data);
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateProfile() {
  isLoading.value = true;
  getMyProfile()
    .then((data: any) => {
      workerStore.setWorkerProfile(data);
      isLoading.value = false;
    })
    .catch((error: unknown) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

if (route.query && route.query.tab) {
  const tab = route.query.tab as string;
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
}
getProfile();
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
  }
}
</style>
