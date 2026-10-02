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
            <b-button size="is-small" type="is-danger" outlined rounded icon-right="delete"
              @click="deleteUser(user.id)"></b-button>
          </div>
          <p class="rcard__sub">{{ user.email }}<span v-if="user.mobileNumber"> · {{ user.mobileNumber }}</span></p>
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
        {{ props.row.mobileNumber }}
      </b-table-column>
      <b-table-column field="position" label="Position" searchable v-slot="props">
        {{ props.row.position }}
      </b-table-column>
      <b-table-column field="email" label="Email" searchable v-slot="props">
        {{ props.row.email }}
      </b-table-column>
      <b-table-column field="actions" v-slot="props">
        <b-button type="is-danger" outlined rounded icon-right="delete"
          @click="deleteUser(props.row.id)"></b-button>
      </b-table-column>
    </SigookGrid>

    <!-- Create user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="showModal = false" width="500px">
      <CreateUser @updateUsers="updateList" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import CreateUser from "@/components/CompanyCreateUserModal.vue";
import SigookGrid from '@/components/SigookGrid.vue';
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { getCompanyUser, deleteCompanyUser } from '@/api/companyApi';
import { useBreakpoint } from '@/composables/useBreakpoint';
import type { CompanyUserModel } from '@/types/company';

const { isTouch } = useBreakpoint();
const userSearch = ref('');
const isLoading = ref(false);
const showModal = ref(false);
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
}

function deleteUser(id: string) {
  showAlertConfirm('Are you sure?', 'You want to delete user.')
    .then(response => {
      if (response) {
        isLoading.value = true;
        deleteCompanyUser(id)
          .then(async () => {
            await getUsers();
            isLoading.value = false;
          })
          .catch(error => {
            isLoading.value = false;
            showAlertError(error);
          });
      }
    });
}

async function updateList() {
  showModal.value = false;
  await getUsers();
}

(async () => {
  await getUsers();
})();
</script>
