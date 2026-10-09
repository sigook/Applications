<template>
  <div class="profile">

    <b-loading v-model="isLoading"></b-loading>

    <nav class="profile-menu">
      <div class="contain-profile">

        <div class="profile-selected">
          <svg width="100" height="100">
            <circle cx="50" cy="50" r="35" fill="#aeaeae" />
            <text x="50%" y="50%" text-anchor="middle" fill="white" font-size="25px" font-family="Arial" dy=".3em">
              {{ avatarLetters(companyUser?.email) }}
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
          <h1 class="is-capitalized fz2">{{ companyUser.name }} {{ companyUser.lastname }}</h1>
        </div>
      </div>
      <component v-bind:is="currentTabComponent" class="tab" v-model:user="companyUser"></component>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Component } from 'vue';
import CompanyUserUpdate from '@/modules/company/profile/components/CompanyUserUpdate.vue';
import ProfileAccountInformation from '@/shared/company-profile/ProfileAccountInformation.vue';
import { avatarLetters } from '@/shared/format';
import { getCompanyUserDetail } from '@/modules/company/profile/api';
import type { CompanyUserModel } from '@/shared/company-profile/types';

const tabComponents: Record<string, Component> = {
  CompanyUserUpdate,
  ProfileAccountInformation,
};

const isLoading = ref(false);
const currentTab = ref<string>('CompanyUserUpdate');
const tabs = ref<string[]>(['CompanyUserUpdate', 'ProfileAccountInformation']);
const companyUser = ref<CompanyUserModel | null>(null);

const currentTabComponent = computed(() => tabComponents[currentTab.value]);

function changeTab(newTab: string) {
  currentTab.value = newTab;
}

function getCompanyUser() {
  isLoading.value = true;
  getCompanyUserDetail()
    .then((user) => {
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
