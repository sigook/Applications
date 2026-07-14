<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex justify-content-between">
      <h3 class="fw-bold">Requested by</h3>
      <b-button v-if="canEdit" type="is-primary" size="is-small" outlined rounded @click="showModal = true">Add</b-button>
    </div>
    <div>
      <ul v-if="data" class="p-1">
        <li v-for="(item) in data.items" :key="item.id"
          class="content-flex-between align-items-center mb-0 hover-actions fz-14">
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
            <div class="modal-container small-container modal-light modal-overflow h-auto border-radius">
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
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import {
  getAgencyRequestRequestedBy,
  postAgencyRequestRequestedBy,
  deleteAgencyRequestRequestedBy
} from "@/api/agencyRequestApi";
import ContactList from './ContactListModal.vue';

const props = defineProps<{ requestId: any; companyId: any; activeUsers?: any[]; canEdit?: boolean }>();

const showModal = ref(false);
const isLoading = ref(false);
const data = ref<any>(null);

function loadRequestedBy() {
  getAgencyRequestRequestedBy(props.requestId)
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

function addRequestedBy(item: any) {
  isLoading.value = true;
  postAgencyRequestRequestedBy(props.requestId, item.id)
    .then(() => {
      isLoading.value = false;
      updateContactList(item);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function removeRequestedBy(item: any) {
  const index = data.value.items.findIndex((x: any) => x.id === item.id);
  isLoading.value = true;
  deleteAgencyRequestRequestedBy(props.requestId, item.id)
    .then(() => {
      isLoading.value = false;
      showModal.value = false;
      data.value.items.splice(index, 1);
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

loadRequestedBy();
</script>
