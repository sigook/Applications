<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="showModal = true">
        {{ $t('Create') }}
      </b-button>
    </b-field>
    <b-table :data="data" narrowed hoverable :mobile-cards="false" paginated pagination-rounded :per-page="10">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="fullName" label="Full Name" v-slot="props">
          {{ props.row.title }} {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }}
        </b-table-column>
        <b-table-column field="position" label="Position" v-slot="props">
          {{ props.row.position }}
        </b-table-column>
        <b-table-column field="mobileNumber" label="Mobile Number" v-slot="props">
          {{ props.row.mobileNumber || 'None' }}
        </b-table-column>
        <b-table-column field="officeNumber" label="Office Number" v-slot="props">
          <span v-if="props.row.officeNumber">
            {{ props.row.officeNumber }}
            <span v-if="props.row.officeNumberExt">Ext. {{ props.row.officeNumberExt }}</span>
          </span>
          <span v-else>None</span>
        </b-table-column>
        <b-table-column field="email" label="Email" v-slot="props">
          {{ props.row.email }}
        </b-table-column>
        <b-table-column field="actions" label="Actions" v-slot="props">
          <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2"
            @click="openEditModal(props.row)"></b-button>
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="deleteAgencyCompanyContactPerson(props.row.id)"></b-button>
        </b-table-column>
      </template>
    </b-table>

    <b-modal v-model="showModal" @close="showModal = false" width="500px">
      <contact-form :current-contact="currentContact" :profile-id="profileId"
        @updateContent="onUpdateModal"></contact-form>
    </b-modal>
  </div>
</template>
<script>

export default {
  data() {
    return {
      isLoading: false,
      profileId: this.$route.params.id,
      showModal: false,
      data: [],
      currentContact: null,
    }
  },
  methods: {
    async getAgencyCompanyContactPerson() {
      this.isLoading = true;
      await this.$store.dispatch('agency/getAgencyCompanyContactPerson', this.profileId)
        .then(response => {
          this.isLoading = false;
          this.data = response;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    openEditModal(item) {
      this.currentContact = item;
      this.showModal = true;
    },
    closeModal() {
      this.currentContact = null;
      this.showModal = false;
    },
    async onUpdateModal() {
      await this.getAgencyCompanyContactPerson();
      this.closeModal();
    },
    deleteAgencyCompanyContactPerson(id) {
      this.showAlertConfirm("Are you sure", "You want to delete this contact")
        .then(response => {
          if (response) {
            this.isLoading = true;
            this.$store.dispatch('agency/deleteAgencyCompanyContactPerson', { profileId: this.profileId, personId: id })
              .then(async () => {
                this.showAlertSuccess('Deleted')
                await this.getAgencyCompanyContactPerson();
                this.isLoading = false;
              })
              .catch(error => {
                this.isLoading = false;
                this.showAlertError(error)
              })
          }
        }).catch(error => {
          this.showAlertError(error)
        })
    }
  },
  created() {
    this.getAgencyCompanyContactPerson();
  },
  components: {
    ContactForm: () => import("./ContactPersonForm")
  }
}
</script>