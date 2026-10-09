<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="users" :refresh="getUsers">
      <template #actions>
        <b-button icon-left="plus" @click="showModal = true">Add</b-button>
      </template>
      <b-table-column field="email" label="Email" v-slot="props">
        {{ props.row.email }}
      </b-table-column>
      <b-table-column field="name" label="Name" v-slot="props">
        {{ props.row.name }}
      </b-table-column>
      <b-table-column field="lastname" label="Last Name" v-slot="props">
        {{ props.row.lastname }}
      </b-table-column>
      <b-table-column field="mobileNumber" label="Mobile Number" v-slot="props">
        {{ props.row.mobileNumber }}
      </b-table-column>
      <b-table-column field="position" label="Position" v-slot="props">
        {{ props.row.position }}
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-button type="is-danger" outlined rounded icon-right="delete"
          @click="deleteUser(props.row.id)"></b-button>
      </b-table-column>
    </SigookGrid>

    <!-- Create user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="showModal = false" width="500px">
      <create-user :save="(user) => createCompanyProfileUser(props.company.id, user)" @updateUsers="updateUsers" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import { getCompanyUsers, deleteCompanyProfileUser, createCompanyProfileUser } from '@/modules/agency/recruiting/clients/api';
import CreateUser from "@/shared/company-profile/CompanyCreateUserModal.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';
import type { CompanyUserModel } from '@/shared/company-profile/types';

const props = defineProps<{ company: AgencyCompanyProfileDetail }>();

const isLoading = ref(false);
const showModal = ref(false);
const users = ref<(CompanyUserModel & { actions: null })[]>([]);

async function getUsers() {
  const response = await getCompanyUsers(props.company.id);
  users.value = response.map((r) => ({ ...r, actions: null }));
}

async function deleteUser(id: string) {
  isLoading.value = true;
  await deleteCompanyProfileUser(props.company.id, id)
    .catch(error => {
      isLoading.value = false;
      showAlertError(error.data);
    });
  await getUsers();
  isLoading.value = false;
}

async function updateUsers() {
  await getUsers();
  showModal.value = false;
}

(async () => {
  isLoading.value = true;
  await getUsers();
  isLoading.value = false;
})();
</script>
