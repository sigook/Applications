<template>
  <div class="company-wrapper white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>

    <section class="company-top" v-if="agency">
      <div class="hover-actions">
        <h2 class="capitalize fz1 fw-700">
          <span class="fw-400 fz-1" v-if="agency.numberId">{{ agency.numberId }} |
          </span>
          {{ lowercase(agency.fullName) }}
        </h2>
      </div>
    </section>

    <b-tabs v-model="currentTab" @update:modelValue="changeTab" v-if="agency">
      <b-tab-item label="Orders" value="Orders">
        <agency-requests v-if="visitedTabs.includes('Orders')" :agency="agency" class="p-2" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { showAlertError } from '@/utils/toast';
import { getAgency } from '@/api/agencyApi';
import { lowercase } from '@/utils/filters';
import AgencyRequests from '@/components/agency/AgencyRequests.vue';

const route = useRoute();
const router = useRouter();

const currentTab = ref<string>('Orders');
const visitedTabs = ref<string[]>(['Orders']);
const agency = ref<any>(null);
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
    path: `/agency-detail/${route.params.id}`,
    query: {
      tab: tab,
    },
  });
}

function loadAgency() {
  isLoading.value = false;
  getAgency(route.params.id as any)
    .then((a: any) => {
      agency.value = a;
      isLoading.value = false;
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}
</script>
