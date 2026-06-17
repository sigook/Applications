<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <h2 class="text-center main-title"> Contacts </h2>
    <ul v-if="data" class="border-top-gray">
      <li v-for="item in data" :key="item.id"
        class="list-item-border-bottom content-flex-between align-items-center mb-0">
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
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAgencyCompanyContactPerson } from "@/api/agencyCompanyApi";

const props = defineProps<{ requestId?: any; companyId: any; activeUsers: any[] }>();
const emit = defineEmits<{
  (e: 'selectContact', item: any): void;
  (e: 'removeContact', item: any): void;
}>();

const isLoading = ref(false);
const data = ref<any[]>([]);

function loadContactPersons() {
  isLoading.value = true;
  getAgencyCompanyContactPerson(props.companyId)
    .then(response => {
      isLoading.value = false;
      data.value = response.map((item: any) => ({ ...item, active: false }));
      updateContacts();
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateContacts() {
  for (let i = 0; i < props.activeUsers.length; i++) {
    for (let j = 0; j < data.value.length; j++) {
      if (props.activeUsers[i].id === data.value[j].id) {
        data.value[j].active = true;
      }
    }
  }
}

function selectContact(item: any) {
  emit('selectContact', item);
}

function removeContactFromActive(item: any) {
  emit('removeContact', item);
}

loadContactPersons();
</script>
