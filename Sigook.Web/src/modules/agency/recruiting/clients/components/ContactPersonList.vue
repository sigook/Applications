<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="data" :refresh="loadContactPersons">
      <template #actions>
        <b-button icon-left="plus" @click="showModal = true">Create</b-button>
      </template>
      <b-table-column field="fullName" label="Full Name" v-slot="props">
        {{ props.row.title }} {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }}
      </b-table-column>
      <b-table-column field="position" label="Position" v-slot="props">
        {{ props.row.position }}
      </b-table-column>
      <b-table-column field="mobileNumber" label="Mobile Number" v-slot="props">
        {{ props.row.mobileNumber || 'None' }}
      </b-table-column>
      <b-table-column field="officeNumber" label="Office Number" v-slot="props">
        <span v-if="props.row.officeNumber">
          {{ props.row.officeNumber }}
          <span v-if="props.row.officeNumberExt">Ext. {{ props.row.officeNumberExt }}</span>
        </span>
        <span v-else>None</span>
      </b-table-column>
      <b-table-column field="email" label="Email" v-slot="props">
        {{ props.row.email }}
      </b-table-column>
      <b-table-column field="actions" label="Actions" v-slot="props">
        <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2"
          @click="openEditModal(props.row)"></b-button>
        <b-button type="is-danger" outlined rounded icon-right="delete"
          @click="onDeleteContactPerson(props.row.id)"></b-button>
      </b-table-column>
    </SigookGrid>

    <b-modal custom-content-class="card" v-model="showModal" @close="closeModal" width="500px">
      <div class="p-4">
        <h2 class="has-text-centered fz1 mb-4">{{ currentContact ? 'Edit contact' : 'New contact' }}</h2>
        <contact-form :current-contact="currentContact" :profile-id="profileId"
          @updateContent="onUpdateModal" @cancel="closeModal"></contact-form>
      </div>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/shared/utils/toast";
import { getAgencyCompanyContactPerson, deleteAgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/api';
import ContactForm from "@/modules/agency/recruiting/clients/components/ContactPersonForm.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import type { AgencyCompanyContactPerson } from '@/modules/agency/recruiting/clients/types';

const emit = defineEmits<{ (e: 'changed', count: number): void }>();

const route = useRoute();

const isLoading = ref(false);
const profileId = route.params.id as string;
const showModal = ref(false);
const data = ref<AgencyCompanyContactPerson[]>([]);
const currentContact = ref<AgencyCompanyContactPerson | null>(null);

async function loadContactPersons() {
  isLoading.value = true;
  await getAgencyCompanyContactPerson(profileId)
    .then(response => {
      isLoading.value = false;
      data.value = response;
      emit('changed', response.length);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openEditModal(item: AgencyCompanyContactPerson) {
  currentContact.value = item;
  showModal.value = true;
}

function closeModal() {
  currentContact.value = null;
  showModal.value = false;
}

async function onUpdateModal() {
  await loadContactPersons();
  closeModal();
}

function onDeleteContactPerson(id: string) {
  showAlertConfirm("Are you sure", "You want to delete this contact")
    .then(response => {
      if (response) {
        isLoading.value = true;
        deleteAgencyCompanyContactPerson(profileId, id)
          .then(async () => {
            showAlertSuccess('Deleted');
            await loadContactPersons();
            isLoading.value = false;
          })
          .catch(error => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    }).catch(error => {
      showAlertError(error);
    });
}

loadContactPersons();
</script>
