<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">
        {{ 'Create' }}
      </b-button>
    </b-field>
    <b-table sticky-header height="var(--grid-height)" :data="data" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" pagination-rounded :per-page="10">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
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
          <b-button type="is-info" outlined rounded icon-right="pencil" class="me-2"
            @click="openEditModal(props.row)"></b-button>
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="onDeleteContactPerson(props.row.id)"></b-button>
        </b-table-column>
      </template>
    </b-table>

    <b-modal v-model="showModal" @close="showModal = false" width="500px">
      <contact-form :current-contact="currentContact" :profile-id="profileId"
        @updateContent="onUpdateModal"></contact-form>
    </b-modal>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertConfirm, showAlertError, showAlertSuccess } from "@/utils/toast";
import { getAgencyCompanyContactPerson, deleteAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";
import ContactForm from "./ContactPersonForm.vue";

const route = useRoute();

const isLoading = ref(false);
const profileId = route.params.id;
const showModal = ref(false);
const data = ref<any[]>([]);
const currentContact = ref<any>(null);

async function loadContactPersons() {
  isLoading.value = true;
  await getAgencyCompanyContactPerson(profileId)
    .then(response => {
      isLoading.value = false;
      data.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openEditModal(item: any) {
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

function onDeleteContactPerson(id: any) {
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
