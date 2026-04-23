<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">Add</b-button>
    </b-field>
    <b-table :data="users" narrowed hoverable :mobile-cards="false" paginated pagination-rounded :per-page="pageSize"
      v-model:current-page="pageIndex">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
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
    </b-table>

    <!-- Create user modal-->
    <b-modal v-model="showModal" @close="showModal = false" width="500px">
      <create-user :companyId="props.company.companyId" @updateUsers="updateUsers" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getCompanyProfileUsers, deleteCompanyProfileUser } from "@/api/agencyCompanyApi";
import CreateUser from "@/components/CompanyCreateUserModal.vue";

const props = defineProps<{ company: any }>();

const isLoading = ref(false);
const showModal = ref(false);
const pageIndex = ref(1);
const pageSize = 30;
const users = ref<any[]>([]);

async function getUsers() {
  const response = await getCompanyProfileUsers(props.company.companyId);
  users.value = response.map((r: any) => ({ ...r, actions: null }));
}

async function deleteUser(id: any) {
  isLoading.value = true;
  await deleteCompanyProfileUser(props.company.companyId, id)
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
