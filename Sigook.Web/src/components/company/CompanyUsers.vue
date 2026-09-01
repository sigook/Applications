<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">Add</b-button>
    </b-field>
    <template v-if="isMobile">
      <div class="mobile-list-toolbar">
        <b-field>
          <b-input v-model="userSearch" placeholder="Search..." icon="magnify" expanded></b-input>
        </b-field>
      </div>
      <div class="rcard-list">
        <div v-for="user in filteredUsers" :key="user.id" class="rcard">
          <div class="rcard__head">
            <p class="rcard__title">{{ user.name }} {{ user.lastName }}</p>
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
        <p v-if="filteredUsers.length === 0" class="has-text-centered">No records available</p>
      </div>
    </template>
    <b-table v-else sticky-header height="var(--grid-height)" :data="users" narrowed hoverable :mobile-cards="false" paginated pagination-size="is-small" pagination-rounded :per-page="pageSize"
      v-model:current-page="pageIndex">
      <template v-slot:empty>
        <p class="container has-text-centered">No records available</p>
      </template>
      <template>
        <b-table-column field="name" label="Name" searchable v-slot="props">
          {{ props.row.name }}
        </b-table-column>
        <b-table-column field="lastName" label="Last Name" searchable v-slot="props">
          {{ props.row.lastName }}
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
      </template>
    </b-table>

    <!-- Create user modal-->
    <b-modal custom-content-class="card" v-model="showModal" @close="showModal = false" width="500px">
      <CreateUser @updateUsers="updateList" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import CreateUser from "@/components/CompanyCreateUserModal.vue";
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { getCompanyUser, deleteCompanyUser } from '@/api/companyApi';
import { useBreakpoint } from '@/composables/useBreakpoint';

const { isMobile } = useBreakpoint();
const userSearch = ref('');
const isLoading = ref(false);
const showModal = ref(false);
const pageIndex = ref(1);
const pageSize = ref(30);
const users = ref<any[]>([]);

const filteredUsers = computed(() => {
  const term = userSearch.value.trim().toLowerCase();
  if (!term) return users.value;
  return users.value.filter((u: any) =>
    [u.name, u.lastName, u.mobileNumber, u.position, u.email]
      .some((v: unknown) => typeof v === 'string' && v.toLowerCase().includes(term)),
  );
});

async function getUsers() {
  const data = await getCompanyUser();
  users.value = data.map((r: any) => ({ ...r, actions: null }));
}

function deleteUser(id: any) {
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
