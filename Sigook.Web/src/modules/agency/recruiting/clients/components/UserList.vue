<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="users" :refresh="getUsers">
      <template #actions>
        <b-button icon-left="plus" @click="showModal = true">Add</b-button>
      </template>
      <b-table-column field="email" label="Email" v-slot="props">
        <copy-link kind="email" :value="props.row.email" />
      </b-table-column>
      <b-table-column field="name" label="Name" v-slot="props">
        {{ props.row.name }}
      </b-table-column>
      <b-table-column field="lastname" label="Last Name" v-slot="props">
        {{ props.row.lastname }}
      </b-table-column>
      <b-table-column field="mobileNumber" label="Mobile Number" v-slot="props">
        <copy-link kind="phone" :value="props.row.mobileNumber" />
      </b-table-column>
      <b-table-column field="position" label="Position" v-slot="props">
        {{ props.row.position }}
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <div class="buttons is-flex-wrap-nowrap">
          <b-button type="is-info" outlined rounded icon-right="pencil" title="Edit"
            @click="editingUser = props.row"></b-button>
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="deleteUser(props.row.id)"></b-button>
        </div>
      </b-table-column>
    </SigookGrid>

    <!-- Create user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="showModal = false" width="500px">
      <create-user :save="(user) => createCompanyProfileUser(company.id, user)" @updateUsers="updateUsers" />
    </b-modal>

    <b-modal custom-content-class="card" :model-value="editingUser !== null" @close="editingUser = null"
      width="500px" :destroy-on-hide="true">
      <create-user v-if="editingUser" :user="editingUser" email-editable :save="saveUser" @updateUsers="onUserEdited" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/shared/utils/toast";
import {
  getCompanyUsers, deleteCompanyProfileUser, createCompanyProfileUser, updateCompanyProfileUser, updateCompanyUserEmail,
} from '@/modules/agency/recruiting/clients/api';
import CreateUser from "@/shared/company-profile/CompanyUserModal.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';
import type { AgencyCompanyProfileDetail } from '@/modules/agency/recruiting/clients/types';
import type { CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';

const { company } = defineProps<{ company: AgencyCompanyProfileDetail }>();

const isLoading = ref(false);
const showModal = ref(false);
const editingUser = ref<CompanyUserModel | null>(null);
const users = ref<(CompanyUserModel & { actions: null })[]>([]);

async function getUsers() {
  const response = await getCompanyUsers(company.id);
  users.value = response.map((r) => ({ ...r, actions: null }));
}

async function deleteUser(id: string) {
  isLoading.value = true;
  try {
    await deleteCompanyProfileUser(company.id, id);
  } catch (error) {
    isLoading.value = false;
    showAlertError(typeof error === 'object' && error !== null && 'data' in error ? error.data : undefined);
  }
  await getUsers();
  isLoading.value = false;
}

async function saveUser(model: CreateCompanyUserModel) {
  const user = editingUser.value;
  if (!user) return;
  await updateCompanyProfileUser(company.id, user.id, { ...user, ...model, position: model.position ?? '', mobileNumber: model.mobileNumber ?? '' });
  if (model.email.trim().toLowerCase() !== user.email.toLowerCase()) {
    await updateCompanyUserEmail(company.id, user.id, { newEmail: model.email });
  }
}

async function onUserEdited() {
  editingUser.value = null;
  await getUsers();
}

async function updateUsers() {
  await getUsers();
  showModal.value = false;
}

void (async () => {
  isLoading.value = true;
  await getUsers();
  isLoading.value = false;
})();
</script>
