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
      <create-user :profileId="props.company.id" @updateUsers="updateUsers" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getCompanyUsers, deleteCompanyProfileUser } from "@/api/agencyCompanyApi";
import CreateUser from "@/components/CompanyCreateUserModal.vue";
import SigookGrid from '@/components/SigookGrid.vue';

const props = defineProps<{ company: any }>();

const isLoading = ref(false);
const showModal = ref(false);
const users = ref<any[]>([]);

async function getUsers() {
  const response = await getCompanyUsers(props.company.id);
  users.value = response.map((r: any) => ({ ...r, actions: null }));
}

async function deleteUser(id: any) {
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
