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
      <create-user :companyId="company.companyId" @updateUsers="updateUsers" />
    </b-modal>
  </div>
</template>

<script>
export default {
  props: ['company'],
  components: {
    CreateUser: () => import("@/components/CompanyCreateUserModal")
  },
  data() {
    return {
      isLoading: false,
      showModal: false,
      pageIndex: 1,
      pageSize: 30,
      users: [],
    }
  },
  methods: {
    async getUsers() {
      this.users = await this.$store.dispatch('agency/getCompanyProfileUsers', this.company.companyId);
      this.users = this.users.map(r => ({ ...r, actions: null }));
    },
    async deleteUser(id) {
      this.isLoading = true;
      await this.$store.dispatch('agency/deleteCompanyProfileUser', { companyId: this.company.companyId, userId: id })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        })
      await this.getUsers();
      this.isLoading = false;
    },
    async updateUsers() {
      await this.getUsers();
      this.showModal = false;
    }
  },
  async created() {
    this.isLoading = true;
    await this.getUsers();
    this.isLoading = false;
  }
}
</script>