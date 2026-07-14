<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field v-if="isAccountingManager" grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">Add</b-button>
    </b-field>
    <b-table sticky-header height="var(--grid-height)" :data="users" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" pagination-rounded :per-page="pageSize"
      v-model:current-page="pageIndex">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="name" label="Name" searchable v-slot="props">
          {{ props.row.name }}
        </b-table-column>
        <b-table-column field="email" label="Email" searchable v-slot="props">
          {{ props.row.email }}
        </b-table-column>
        <b-table-column field="actions" :visible="isAccountingManager" v-slot="props">
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="deleteUser(props.row.id)"></b-button>
        </b-table-column>
      </template>
    </b-table>

    <!-- Create user modal-->
    <b-modal v-model="showModal" @close="showModal = false" width="500px">
      <create-user @updateUsers="() => updateList()" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { getAgencyPersonnel, deleteAgencyPersonnel } from "@/api/agencyApi";
import { useAccountingAdmin } from "@/composables/useAccountingAdmin";
import CreateUser from "./AgencyCreatePersonnelModal.vue";

const { isAccountingManager } = useAccountingAdmin();

const isLoading = ref(false);
const pageIndex = ref(1);
const pageSize = ref(30);
const showModal = ref(false);
const users = ref<any[]>([]);

function getUsers() {
  isLoading.value = true;
  getAgencyPersonnel()
    .then((response) => {
      isLoading.value = false;
      users.value = response.map(r => ({ ...r, actions: null }));
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateList() {
  showModal.value = false;
  getUsers();
}

function deleteUser(id: any) {
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
