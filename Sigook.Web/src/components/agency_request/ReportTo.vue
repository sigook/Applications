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
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import {
  getAgencyRequestReportTo,
  postAgencyRequestReportTo,
  deleteAgencyRequestReportTo
} from "@/api/agencyRequestApi";
import ContactList from './ContactListModal.vue';

const props = defineProps<{ requestId: any; companyId: any; canEdit?: boolean }>();

const showModal = ref(false);
const isLoading = ref(false);
const data = ref<any>(null);

function loadReportTo() {
  getAgencyRequestReportTo(props.requestId)
    .then(response => {
      data.value = response;
    })
    .catch(error => {
      showAlertError(error);
    });
}

function updateContactList(item: any) {
  data.value.items.push(item);
  showModal.value = false;
}

function addReportTo(item: any) {
  isLoading.value = true;
  postAgencyRequestReportTo(props.requestId, item.id)
    .then(() => {
      isLoading.value = false;
      updateContactList(item);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function removeReportTo(item: any) {
  const index = data.value.items.findIndex((x: any) => x.id === item.id);
  isLoading.value = true;
  deleteAgencyRequestReportTo(props.requestId, item.id)
    .then(() => {
      isLoading.value = false;
      data.value.items.splice(index, 1);
      showModal.value = false;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadReportTo();
</script>
