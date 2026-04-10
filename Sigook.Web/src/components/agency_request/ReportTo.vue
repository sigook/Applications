<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex space-between">
      <h3 class="fw-700">Report to</h3>
      <button v-if="canEdit" @click="showModal = true" class="sm-save-button">Add</button>
    </div>
    <div>
      <ul v-if="data" class="p-1">
        <li v-for="(item) in data.items" :key="item.id"
          class="content-flex-between align-center mb-0 hover-actions fz-14">
          <span class="d-inline-block valign-middle">{{ item.firstName }} {{ item.lastName }}</span>
          <button v-if="canEdit" class="btn-icon-sm btn-icon-reject valign-middle actions"
            @click="removeReportTo(item)">DELETE</button>
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
                @removeContact="(item) => removeReportTo(item)"
                @selectContact="(item) => addReportTo(item)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end Select custom modal -->
  </div>
</template>
<script lang="ts">
import toastMixin from "@/mixins/toastMixin";
import {
  getAgencyRequestReportTo,
  postAgencyRequestReportTo,
  deleteAgencyRequestReportTo,
} from "@/api/agencyRequestApi";

export default {
  props: ['requestId', 'companyId', 'canEdit'],
  mixins: [toastMixin],
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
    loadReportTo() {
      getAgencyRequestReportTo(this.requestId)
        .then(response => {
          this.data = response;
        })
        .catch(error => {
          this.showAlertError(error)
        })
    },
    updateContactList(item) {
      this.data.items.push(item)
      this.showModal = false;
    },
    addReportTo(item) {
      this.isLoading = true;
      postAgencyRequestReportTo(this.requestId, item.id)
        .then(() => {
          this.isLoading = false;
          this.updateContactList(item)
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    },
    removeReportTo(item) {
      let index = this.data.items.findIndex(x => x.id === item.id);
      this.isLoading = true;
      deleteAgencyRequestReportTo(this.requestId, item.id)
        .then(() => {
          this.isLoading = false;
          this.data.items.splice(index, 1)
          this.showModal = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error)
        })
    }
  },
  created() {
    this.loadReportTo();
  }
}
</script>