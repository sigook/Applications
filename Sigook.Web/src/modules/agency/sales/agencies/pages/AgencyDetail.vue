<template>
  <div class="company-wrapper">
    <b-loading v-model="isLoading"></b-loading>

    <Breadcrumbs :crumbs="agenciesCrumbs" back-to="/sales/agencies" />
    <section class="company-top" v-if="agency">
      <div class="hover-actions">
        <h2 class="is-capitalized fz1 has-text-weight-bold">
          {{ lowercase(agency.fullName) }}
        </h2>
      </div>
    </section>

    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="agency">
      <b-tab-item label="Requests" value="Requests">
        <agency-requests v-if="visitedTabs.includes('Requests')" :agency="agency" class="p-2" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from '@/shared/utils/toast';
import { getAgency } from '@/modules/agency/sales/agencies/api';
import { lowercase } from '@/shared/format';
import AgencyRequests from '@/modules/agency/shared/AgencyRequests.vue';
import Breadcrumbs from '@/shared/ui/Breadcrumbs.vue';
import { agenciesCrumbs } from '@/shared/ui/breadcrumbs';
import type { AgencyDetail } from '@/modules/agency/profile/types';

const route = useRoute();
const router = useRouter();

const currentTab = ref<string>('Requests');
const visitedTabs = ref<string[]>(['Requests']);
const agency = ref<AgencyDetail | null>(null);
const isLoading = ref(true);

loadAgency();
if (route.query && route.query.tab) {
  currentTab.value = route.query.tab as string;
  if (!visitedTabs.value.includes(route.query.tab as string)) {
    visitedTabs.value.push(route.query.tab as string);
  }
}

function changeTab(tab: string) {
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  router.push({
    path: `/sales/agencies/${route.params.id}`,
    query: {
      tab: tab,
    },
  });
}

function loadAgency() {
  isLoading.value = false;
  getAgency(route.params.id as string)
    .then((a) => {
      agency.value = a;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
