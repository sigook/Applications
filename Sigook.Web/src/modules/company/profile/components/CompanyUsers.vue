<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <SigookGrid :data="isTouch ? filteredUsers : users" :refresh="getUsers">
      <template #actions>
        <b-input v-if="isTouch" v-model="userSearch" placeholder="Search..." icon="magnify"></b-input>
        <b-button icon-left="plus" @click="showModal = true">Add</b-button>
      </template>
      <template #mobile-card="{ row: user }">
        <div class="rcard">
          <div class="rcard__head">
            <p class="rcard__title">{{ user.name }} {{ user.lastname }}</p>
            <div class="buttons is-flex-wrap-nowrap">
              <b-button size="is-small" type="is-info" outlined rounded icon-right="pencil" title="Edit"
                @click="editingUser = user"></b-button>
              <b-button size="is-small" type="is-danger" outlined rounded icon-right="delete"
                @click="deleteUser(user.id)"></b-button>
            </div>
          </div>
          <p class="rcard__sub">
            <copy-link kind="email" :value="user.email" />
            <span v-if="user.mobileNumber"> · <copy-link kind="phone" :value="user.mobileNumber" /></span>
          </p>
          <div class="rcard__rows" v-if="user.position">
            <div class="rcard__row">
              <span class="rcard__label">Position</span>
              <span>{{ user.position }}</span>
            </div>
          </div>
        </div>
      </template>
      <b-table-column field="name" label="Name" searchable v-slot="props">
        {{ props.row.name }}
      </b-table-column>
      <b-table-column field="lastname" label="Last Name" searchable v-slot="props">
        {{ props.row.lastname }}
      </b-table-column>
      <b-table-column field="mobileNumber" label="Phone Number" searchable v-slot="props">
        <copy-link kind="phone" :value="props.row.mobileNumber" />
      </b-table-column>
      <b-table-column field="position" label="Position" searchable v-slot="props">
        {{ props.row.position }}
      </b-table-column>
      <b-table-column field="email" label="Email" searchable v-slot="props">
        <copy-link kind="email" :value="props.row.email" />
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <div class="buttons is-flex-wrap-nowrap">
          <b-button type="is-info" outlined rounded icon-right="pencil" title="Edit" @click="editingUser = props.row"></b-button>
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="deleteUser(props.row.id)"></b-button>
        </div>
      </b-table-column>
    </SigookGrid>

    <!-- Create user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="showModal = false" width="500px">
      <CreateUser :save="createCompanyUser" @updateUsers="updateList" />
    </b-modal>

    <b-modal custom-content-class="card" :model-value="editingUser !== null" @close="editingUser = null"
      width="500px" :destroy-on-hide="true">
      <CreateUser v-if="editingUser" :user="editingUser" email-editable :save="saveUser" @updateUsers="onUserEdited" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import CreateUser from "@/shared/company-profile/CompanyUserModal.vue";
import SigookGrid from '@/shared/ui/SigookGrid.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';
import { showAlertConfirm, showAlertError } from "@/shared/utils/toast";
import {
  getCompanyUser, deleteCompanyUser, createCompanyUser, updateCompanyUser, updateCompanyUserEmail,
} from '@/modules/company/profile/api';
import { useBreakpoint } from '@/shared/composables/useBreakpoint';
import type { CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';

const emit = defineEmits<(e: 'loaded', count: number) => void>();

const { isTouch } = useBreakpoint();
const userSearch = ref('');
const isLoading = ref(false);
const showModal = ref(false);
const editingUser = ref<CompanyUserModel | null>(null);
const users = ref<CompanyUserModel[]>([]);

const filteredUsers = computed(() => {
  const term = userSearch.value.trim().toLowerCase();
  if (!term) return users.value;
  return users.value.filter((u) =>
    [u.name, u.lastname, u.mobileNumber, u.position, u.email]
      .some((v: unknown) => typeof v === 'string' && v.toLowerCase().includes(term)),
  );
});

async function getUsers() {
  users.value = await getCompanyUser();
  emit('loaded', users.value.length);
}

async function deleteUser(id: string) {
  if (!(await showAlertConfirm('Are you sure?', 'You want to delete user.'))) return;
  isLoading.value = true;
  try {
    await deleteCompanyUser(id);
    await getUsers();
    isLoading.value = false;
  } catch (error) {
    isLoading.value = false;
    showAlertError(error);
  }
}

async function saveUser(model: CreateCompanyUserModel) {
  const user = editingUser.value;
  if (!user) return;
  await updateCompanyUser(user.id, { ...user, ...model, email: user.email, position: model.position ?? '', mobileNumber: model.mobileNumber ?? '' });
  if (model.email.trim().toLowerCase() !== user.email.toLowerCase()) {
    await updateCompanyUserEmail(user.id, { newEmail: model.email });
  }
}

async function onUserEdited() {
  editingUser.value = null;
  await getUsers();
}

async function updateList() {
  showModal.value = false;
  await getUsers();
}

void getUsers();
</script>
