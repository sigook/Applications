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
      <create-user @updateUsers="() => updateList()" />
    </b-modal>
  </div>
</template>

<script>

export default {
  name: "AgencyUsers",
  components: {
    CreateUser: () => import("./AgencyCreatePersonnelModal")
  },
  data() {
    return {
      isLoading: false,
      pageIndex: 1,
      pageSize: 30,
      showModal: false,
      users: []
    }
  },
  methods: {
    getUsers() {
      this.isLoading = true;
      this.$store.dispatch('agency/getAgencyPersonnel')
        .then((response) => {
          this.isLoading = false;
          this.users = response.map(r => ({ ...r, actions: null }))
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    updateList() {
      this.showModal = false;
      this.getUsers();
    },
    deleteUser(id) {
      this.showAlertConfirm(this.$t('AreYouSure'), this.$t('YouWantToDeleteUser'))
        .then(response => {
          if (response) {
            this.isLoading = true;
            this.$store.dispatch('agency/deleteAgencyPersonnel', id)
              .then(() => {
                this.isLoading = false;
                this.getUsers();
              })
              .catch(error => {
                this.isLoading = false;
                this.showAlertError(error);
              })
          }
        });
    },
  },
  created() {
    this.getUsers()
  }
}
</script>
