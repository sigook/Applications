<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> Contacts </h2>
    <ul v-if="data" class="border-top-gray">
      <li v-for="item in data" :key="item.id"
        class="list-item-border-bottom content-flex-between align-center mb-0">
        <div>
          <b>{{ item.title }} {{ item.firstName }} {{ item.lastName }}</b> <span> | {{ item.position }}</span>
          <span class="d-block fz-1">{{ item.mobileNumber }}</span>
          <span class="d-block fz-1">{{ item.officeNumber }} {{ item.officeNumberExt }}</span>
          <span class="d-block fz-1">{{ item.email }}</span>
        </div>
        <div>
          <button v-if="item.active" class="sm-btn red-button outline-btn btn-radius"
            @click="removeContactFromActive(item)">Remove</button>
          <button v-else class="sm-save-button" @click="selectContact(item)">Add</button>
        </div>
      </li>
    </ul>
  </div>
</template>
<script>
export default {
  props: ['requestId', 'companyId', 'activeUsers'],
  data() {
    return {
      isLoading: false,
      data: [],
    }
  },
  methods: {
    getAgencyCompanyContactPerson(index) {
      this.isLoading = true;
      this.$store.dispatch('agency/getAgencyCompanyContactPerson', this.companyId)
        .then(response => {
          this.isLoading = false;
          this.data = response;
          this.updateContacts();
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    updateContacts() {
      for (let i = 0; i < this.activeUsers.length; i++) {
        for (let j = 0; j < this.data.items.length; j++) {
          if (this.activeUsers[i].id === this.data.items[j].id) {
            this.$set(this.data.items[j], 'active', true);
          }
        }
      }
    },
    selectContact(item) {
      this.$emit('selectContact', item)
    },
    removeContactFromActive(item) {
      this.$emit('removeContact', item)
    }
  },
  created() {
    this.getAgencyCompanyContactPerson()
  }
}
</script>