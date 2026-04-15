<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex space-between">
      <h3 class="fw-700">Requested by</h3>
      <button @click="showModal = true" v-if="canEdit" class="sm-save-button">Add</button>
    </div>
    <div>
      <ul v-if="data" class="p-1">
        <li v-for="(item) in data.items" :key="item.id"
          class="content-flex-between align-center mb-0 hover-actions fz-14">
          <span class="d-inline-block valign-middle">{{ item.firstName }} {{ item.lastName }}</span>
          <button class="btn-icon-sm btn-icon-reject valign-middle actions" @click="removeRequestedBy(item)"
            v-if="canEdit">DELETE</button>
        </li>
      </ul>
    </div>
    <!-- Select custom modal -->
    <transition name="modal">
      <div v-if="showModal" class="vue-modal min-width-0">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light modal-overflow height-auto border-radius">
              <button @click="showModal = false" type="button" class="cross-icon">close</button>
              <contact-list :requestId="requestId" :companyId="companyId" :activeUsers="data.items"
                @removeContact="(item) => removeRequestedBy(item)"
                @selectContact="(item) => addRequestedBy(item)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end Select custom modal -->
  </div>
</template>
<script lang="ts">
import { showAlertError } from "@/utils/toast";
import {
  getAgencyRequestRequestedBy,
  postAgencyRequestRequestedBy,
  deleteAgencyRequestRequestedBy
} from "@/api/agencyRequestApi";

export default {
  props: ['requestId', 'companyId', 'activeUsers', 'canEdit'],
  data() {
    return {
      showModal: false,
      isLoading: false,
      data: null
    }
  },
  components: {
    ContactList: () => import("./ContactListModal.vue")
  },
  methods: {
    loadRequestedBy() {
      getAgencyRequestRequestedBy(this.requestId)
        .then(response => {
          this.data = response;
        })
        .catch(error => {
          showAlertError(error)
        })
    },
    updateContactList(item) {
      this.data.items.push(item)
      this.showModal = false;
    },
    addRequestedBy(item) {
      this.isLoading = true;
      postAgencyRequestRequestedBy(this.requestId, item.id)
        .then(() => {
          this.isLoading = false;
          this.updateContactList(item)
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error)
        })
    },
    removeRequestedBy(item) {
      let index = this.data.items.findIndex(x => x.id === item.id);
      this.isLoading = true;
      deleteAgencyRequestRequestedBy(this.requestId, item.id)
        .then(() => {
          this.isLoading = false;
          this.showModal = false;
          this.data.items.splice(index, 1)
        })
        .catch(error => {
          this.isLoading = false;
          showAlertError(error)
        })
    }
  },
  created() {
    this.loadRequestedBy();
  }
}
</script>