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
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { getAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";

export default {
  props: ['requestId', 'companyId', 'activeUsers'],
  data() {
    return {
      isLoading: false,
      data: []
    }
  },
  methods: {
    loadContactPersons() {
      this.isLoading = true;
      getAgencyCompanyContactPerson(this.companyId)
        .then(response => {
          this.isLoading = false;
          this.data = response.map(item => ({ ...item, active: false }));
          this.updateContacts();
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error)
        })
    },
    updateContacts() {
      for (let i = 0; i < this.activeUsers.length; i++) {
        for (let j = 0; j < this.data.length; j++) {
          if (this.activeUsers[i].id === this.data[j].id) {
            this.data[j].active = true;
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
    this.loadContactPersons()
  }
}
</script>