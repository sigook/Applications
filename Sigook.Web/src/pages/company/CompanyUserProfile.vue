<template>
  <div class="profile">

    <b-loading v-model="isLoading"></b-loading>

    <nav class="profile-menu">
      <div class="contain-profile">

        <div class="profile-selected">
          <svg width="100" height="100">
            <circle cx="50" cy="50" r="35" fill="#aeaeae" />
            <text x="50%" y="50%" text-anchor="middle" fill="white" font-size="25px" font-family="Arial" dy=".3em">
              {{ avatarLetters(companyUser.email) }}
            </text>
          </svg>
          <span class="no-arrow"></span>
        </div>
      </div>

      <br class="only-sm">
      <div class="scroll">
        <a v-for="(tab, index) in tabs" :key="'tabCompany' + index"
          v-bind:class="['tab-button', { active: currentTab === tab }]" v-on:click="changeTab(tab)">
          {{ tab }}
        </a>
      </div>

    </nav>

    <div class="profile-content company-profile">
      <div class="profile-top">
        <div v-if="companyUser">
          <h1 class="text-capitalize fz2">{{ companyUser.name }} {{ companyUser.lastname }}</h1>
        </div>
      </div>
      <component v-bind:is="currentTabComponent" class="tab" v-model:user="companyUser"></component>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import CompanyUserUpdate from '@/components/company/CompanyUserUpdate.vue';
import ProfileAccountInformation from '@/components/agency/ProfileAccountInformation.vue';
import { avatarLetters } from '@/utils/filters';
import { getCompanyUserDetail } from '@/api/companyApi';

const tabComponents: Record<string, any> = {
  CompanyUserUpdate,
  ProfileAccountInformation,
};

const isLoading = ref(false);
const currentTab = ref<string>('CompanyUserUpdate');
const tabs = ref<string[]>(['CompanyUserUpdate', 'ProfileAccountInformation']);
const companyUser = ref<any>({});

const currentTabComponent = computed(() => tabComponents[currentTab.value]);

function changeTab(newTab: string) {
  currentTab.value = newTab;
}

function getCompanyUser() {
  isLoading.value = true;
  getCompanyUserDetail()
    .then((user: any) => {
      companyUser.value = user;
      isLoading.value = false;
    })
    .catch(() => {
      isLoading.value = false;
    });
}

getCompanyUser();
</script>

<style lang="scss" scoped>
.profile-top {
  &>.disabled {
    pointer-events: none;
  }
}
</style>
