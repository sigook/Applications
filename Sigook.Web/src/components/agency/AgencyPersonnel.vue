<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field v-if="isAdmin" grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="openCreateModal">Add</b-button>
    </b-field>
    <b-table sticky-header height="var(--grid-height)" :data="users" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" pagination-rounded :per-page="pageSize"
      v-model:current-page="pageIndex">
      <template v-slot:empty>
        <p class="container has-text-centered">No records available</p>
      </template>
      <template>
        <b-table-column field="name" label="Name" searchable v-slot="props">
          {{ props.row.name }}
        </b-table-column>
        <b-table-column field="email" label="Email" searchable v-slot="props">
          {{ props.row.email }}
        </b-table-column>
        <b-table-column field="role" label="Role" v-slot="props">
          {{ roleLabels[props.row.role] || props.row.role }}
        </b-table-column>
        <b-table-column field="actions" :visible="isAdmin" v-slot="props">
          <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2"
            @click="openEditModal(props.row)"></b-button>
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="deleteUser(props.row.id)"></b-button>
        </b-table-column>
      </template>
    </b-table>

    <!-- Create / edit user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="closeModal" width="500px">
      <personnel-form :personnel="currentPersonnel" @updateUsers="() => updateList()" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { getAgencyPersonnel, deleteAgencyPersonnel } from "@/api/agencyApi";
import { useAdmin } from "@/composables/useAdmin";
import { roleLabels } from "@/security/roles";
import type { AgencyPersonnelListItem } from "@/types/agency";
import PersonnelForm from "./AgencyPersonnelModal.vue";

const { isAdmin } = useAdmin();

const isLoading = ref(false);
const pageIndex = ref(1);
const pageSize = ref(30);
const showModal = ref(false);
const users = ref<AgencyPersonnelListItem[]>([]);
const currentPersonnel = ref<AgencyPersonnelListItem | null>(null);

function getUsers() {
  isLoading.value = true;
  getAgencyPersonnel()
    .then((response) => {
      isLoading.value = false;
      users.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function openCreateModal() {
  currentPersonnel.value = null;
  showModal.value = true;
}

function openEditModal(personnel: AgencyPersonnelListItem) {
  currentPersonnel.value = personnel;
  showModal.value = true;
}

function closeModal() {
  currentPersonnel.value = null;
  showModal.value = false;
}

function updateList() {
  closeModal();
  getUsers();
}

function deleteUser(id: string) {
  showAlertConfirm('Are you sure?', 'You want to delete user.')
    .then(response => {
      if (response) {
        isLoading.value = true;
        deleteAgencyPersonnel(id)
          .then(() => {
            isLoading.value = false;
            getUsers();
          })
          .catch(error => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    });
}

getUsers();
</script>
