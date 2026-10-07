<template>
  <div class="p-4">
    <b-loading v-model="isLoading"></b-loading>
    <template v-if="isCreating">
      <h2 class="has-text-centered fz1 mb-4">New contact</h2>
      <contact-person-form :profile-id="companyProfileId" @updateContent="onContactCreated"
        @cancel="isCreating = false" />
    </template>
    <template v-else>
      <div class="is-flex is-justify-content-space-between is-align-items-center mb-4">
        <h2 class="fz1 mb-0">Contacts</h2>
        <b-button type="is-primary" size="is-small" outlined rounded icon-left="plus"
          @click="isCreating = true">New contact</b-button>
      </div>
      <ul class="border-top-gray">
        <li v-for="item in data" :key="item.id"
          class="list-item-border-bottom content-flex-between is-align-items-center mb-0">
          <div>
            <b>{{ item.title }} {{ item.firstName }} {{ item.lastName }}</b> <span> | {{ item.position }}</span>
            <span class="is-block fz-1">{{ item.mobileNumber }}</span>
            <span class="is-block fz-1">{{ item.officeNumber }} {{ item.officeNumberExt }}</span>
            <span class="is-block fz-1">{{ item.email }}</span>
          </div>
          <div>
            <b-button v-if="isActive(item)" type="is-danger" size="is-small" outlined rounded
              @click="emit('removeContact', item)">Remove</b-button>
            <b-button v-else type="is-primary" size="is-small" outlined rounded
              @click="emit('selectContact', item)">Add</b-button>
          </div>
        </li>
      </ul>
    </template>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";
import ContactPersonForm from '@/components/agency_company/ContactPersonForm.vue';
import type { AgencyCompanyContactPerson, AgencyRequestPersonItem } from "@/types/agency";

const props = defineProps<{ companyProfileId: string; activeUsers: AgencyRequestPersonItem[] }>();
const emit = defineEmits<{
  (e: 'selectContact', item: AgencyCompanyContactPerson): void;
  (e: 'removeContact', item: AgencyCompanyContactPerson): void;
}>();

const isLoading = ref(false);
const isCreating = ref(false);
const data = ref<AgencyCompanyContactPerson[]>([]);

function isActive(item: AgencyCompanyContactPerson) {
  return props.activeUsers.some(x => x.id === item.id);
}

function loadContactPersons() {
  isLoading.value = true;
  getAgencyCompanyContactPerson(props.companyProfileId)
    .then(response => {
      isLoading.value = false;
      data.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onContactCreated(contact: AgencyCompanyContactPerson) {
  data.value.push(contact);
  isCreating.value = false;
  emit('selectContact', contact);
}

loadContactPersons();
</script>
