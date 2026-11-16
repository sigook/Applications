<template>
  <div class="agency-profile">
    <b-loading v-model="isLoading"></b-loading>

    <detail-header v-if="agency" :key="agency.id" :title="lowercase(agency.fullName)" :meta="address"
      :logo="agency.logo?.pathFile" :chips="chips" :kpis="kpis" :header-hidden="!isAgencyTab">
      <template v-if="agencies.length > 1" #actions>
        <b-dropdown :model-value="agency.id" @update:modelValue="selectAgency" position="is-bottom-left"
          aria-role="list">
          <template #trigger>
            <b-button icon-left="swap-horizontal" icon-right="menu-down">Switch agency</b-button>
          </template>
          <b-dropdown-item v-for="option in agencies" :key="option.id" :value="option.id" aria-role="listitem">
            {{ option.name }}
          </b-dropdown-item>
        </b-dropdown>
      </template>

      <b-tabs v-model="currentTab" @update:modelValue="changeTab">
        <b-tab-item label="Profile" value="Profile">
          <agency-profile-tab v-if="visitedTabs.includes('Profile')" v-model:agency="agency"
            :locations-version="locationsVersion" @showTab="changeTab" />
        </b-tab-item>
        <b-tab-item label="Locations" value="Locations">
          <template #header>
            Locations<span class="detail-tab-count">{{ locationsCount }}</span>
          </template>
          <locations v-if="visitedTabs.includes('Locations')" :agency-data="agency" @changed="onLocationsChanged" />
        </b-tab-item>
        <b-tab-item label="Contacts" value="Contacts">
          <template #header>
            Contacts<span class="detail-tab-count">{{ contactsCount }}</span>
          </template>
          <contacts v-if="visitedTabs.includes('Contacts')" v-model:agency-data="agency" />
        </b-tab-item>
        <b-tab-item label="Users" value="Users" header-class="agency-tab-group">
          <template #header>
            Users<span v-if="usersCount !== undefined" class="detail-tab-count">{{ usersCount }}</span>
          </template>
          <users v-if="visitedTabs.includes('Users')" @loaded="usersCount = $event" />
        </b-tab-item>
      </b-tabs>
    </detail-header>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAgencyStore } from '@/modules/agency/store';
import { getAgencyProfile, getAccessibleAgencies, getAgencyPersonnel } from '@/modules/agency/profile/api';
import { lowercase } from '@/shared/format';
import { showAlertError } from '@/shared/utils/toast';
import type { AgencyDetail, AgencyLocationDetail } from '@/modules/agency/profile/types';
import type { DetailChip, DetailKpi } from '@/shared/detail-page/types';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';
import AgencyProfileTab from '@/modules/agency/profile/components/AgencyProfileTab.vue';
import Locations from '@/modules/agency/profile/components/ProfileBilling.vue';
import Contacts from '@/modules/agency/profile/components/ProfileContact.vue';
import Users from '@/modules/agency/profile/components/AgencyPersonnel.vue';

const tabs = ['Profile', 'Locations', 'Contacts', 'Users'];
const agencyTabs = ['Profile', 'Locations', 'Contacts'];

const route = useRoute();
const router = useRouter();
const agencyStore = useAgencyStore();

const isLoading = ref(false);
const agency = ref<AgencyDetail | null>(null);
const locationsVersion = ref(0);
const usersCount = ref<number>();
const agencies = computed(() => agencyStore.agencies);

const initialTab = typeof route.query.tab === 'string' && tabs.includes(route.query.tab) ? route.query.tab : 'Profile';
const currentTab = ref<string>(initialTab);
const visitedTabs = ref<string[]>(['Profile', initialTab]);

const isAgencyTab = computed(() => agencyTabs.includes(currentTab.value));
const locationsCount = computed(() => agency.value?.locations?.length ?? 0);
const contactsCount = computed(() => agency.value?.contactInformation?.length ?? 0);

const address = computed(() => {
  const locations = agency.value?.locations ?? [];
  const location = locations.find((item) => item.isBilling) ?? locations.at(0);
  if (!location) return undefined;
  const city = [location.city?.value, location.city?.province?.code ?? location.province?.code].filter(Boolean).join(', ');
  return [location.address, city, location.postalCode].filter(Boolean).join(' · ');
});

const chips = computed<DetailChip[]>(() => (agency.value?.masterAgency ? [{ label: 'Master agency', variant: 'is-info' }] : []));

const kpis = computed<DetailKpi[]>(() => [
  { key: 'locations', label: 'Locations', value: String(locationsCount.value) },
  { key: 'contacts', label: 'Contacts', value: String(contactsCount.value) },
]);

async function loadAgency(agencyId?: string) {
  isLoading.value = true;
  try {
    agency.value = await getAgencyProfile(agencyId);
  } catch (error) {
    showAlertError(error);
  } finally {
    isLoading.value = false;
  }
}

function selectAgency(agencyId: string) {
  if (agencyId === agency.value?.id) return;
  void router.replace({ query: { ...route.query, agencyId } });
  void loadAgency(agencyId);
}

function onLocationsChanged(locations: AgencyLocationDetail[]) {
  if (!agency.value) return;
  agency.value.locations = [...locations];
  locationsVersion.value++;
}

function changeTab(tab: string) {
  currentTab.value = tab;
  if (!visitedTabs.value.includes(tab)) {
    visitedTabs.value.push(tab);
  }
  void router.push({
    path: '/agency-profile',
    query: { ...route.query, tab },
  });
}

void loadAgency(route.query.agencyId as string | undefined);
void (async () => {
  try {
    usersCount.value = (await getAgencyPersonnel()).length;
  } catch (error) {
    showAlertError(error);
  }
})();
void (async () => {
  try {
    agencyStore.setAgencies(await getAccessibleAgencies());
  } catch (error) {
    showAlertError(error);
  }
})();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.agency-profile :deep(.tabs li.agency-tab-group) {
  display: flex;
  align-items: center;
  margin-left: auto;
  padding-left: 14px;
  border-left: 1px solid $gray-border;

  &::before {
    content: 'All agencies';
    padding-right: 4px;
    font-size: 11px;
    font-weight: 700;
    letter-spacing: 0.06em;
    text-transform: uppercase;
    white-space: nowrap;
    color: $grey-font;
  }
}
</style>
