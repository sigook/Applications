<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">Add</b-button>
    </b-field>
    <b-table :data="users" narrowed hoverable :mobile-cards="false" paginated pagination-rounded :per-page="pageSize"
      :current-page.sync="pageIndex">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
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
    <b-modal v-model="showModal" @close="showModal = false" width="500px">
      <create-user @updateUsers="updateList" />
    </b-modal>
  </div>
</template>

<script lang="ts">
import { showAlertConfirm, showAlertError } from "@/utils/toast";
import { getCompanyUser, deleteCompanyUser } from '@/api/companyApi';

export default {
  name: "CompanyUsers",
  components: {
    CreateUser: () => import("@/components/CompanyCreateUserModal.vue")
  },
  data() {
    return {
      isLoading: false,
      showModal: false,
      pageIndex: 1,
      pageSize: 30,
      users: []
    }
  },
  methods: {
    async getUsers() {
      this.users = await getCompanyUser();
      this.users = this.users.map(r => ({ ...r, actions: null }));
    },
    deleteUser(id) {
      showAlertConfirm('Are you sure?', 'You want to delete user.')
        .then(response => {
          if (response) {
            this.isLoading = true;
            deleteCompanyUser(id)
              .then(async () => {
                await this.getUsers(this.initialPage);
                this.isLoading = false;
              })
              .catch(error => {
                this.isLoading = false;
                showAlertError(error);
              })
          }
        });
    },
    async updateList() {
      this.showModal = false;
      await this.getUsers();
    }
  },
  async created() {
    await this.getUsers();
  }
}
</script>
