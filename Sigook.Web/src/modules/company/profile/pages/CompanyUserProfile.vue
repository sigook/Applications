<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <detail-header v-if="companyUser" :title="fullName" :meta="companyUser.position || undefined" logo-icon="account"
      :kpis="kpis">
      <detail-layout :sections="[]" aria-label="User profile sections">
        <detail-card title="Personal">
          <template #actions>
            <b-button type="is-ghost" size="is-small" class="detail-link" @click="showEditModal = true">
              Edit
            </b-button>
          </template>
          <detail-fields :fields="fields" />
        </detail-card>
      </detail-layout>
    </detail-header>

    <b-modal v-if="companyUser" custom-content-class="card" v-model="showEditModal" width="800px"
      :destroy-on-hide="true">
      <company-user-update v-model:user="companyUser" @saved="showEditModal = false" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { getCompanyUserDetail } from '@/modules/company/profile/api';
import { showAlertError } from '@/shared/utils/toast';
import type { CompanyUserModel } from '@/shared/company-profile/types';
import type { DetailFact, DetailKpi } from '@/shared/detail-page/types';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';
import DetailLayout from '@/shared/detail-page/DetailLayout.vue';
import DetailCard from '@/shared/detail-page/DetailCard.vue';
import DetailFields from '@/shared/detail-page/DetailFields.vue';
import CompanyUserUpdate from '@/modules/company/profile/components/CompanyUserUpdate.vue';

const isLoading = ref(false);
const showEditModal = ref(false);
const companyUser = ref<CompanyUserModel | null>(null);
const kpis: DetailKpi[] = [];

const fullName = computed(() => [companyUser.value?.name, companyUser.value?.lastname].filter(Boolean).join(' '));

const fields = computed<DetailFact[]>(() => [
  { label: 'Name', value: companyUser.value?.name || '—' },
  { label: 'Last name', value: companyUser.value?.lastname || '—' },
  { label: 'Position', value: companyUser.value?.position || '—' },
  {
    label: 'Mobile number',
    value: companyUser.value?.mobileNumber || '—',
    copy: { kind: 'phone', value: companyUser.value?.mobileNumber },
  },
  {
    label: 'Email',
    value: companyUser.value?.email || '—',
    copy: { kind: 'email', value: companyUser.value?.email },
  },
]);

async function loadCompanyUser() {
  isLoading.value = true;
  try {
    companyUser.value = await getCompanyUserDetail();
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

void loadCompanyUser();
</script>
