<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="p-3">
      <h2 class="text-center fz1">Recruiters</h2>
      <ul v-if="data">
        <li v-for="item in data" :key="item.id" class="list-item-border-bottom content-flex-between align-center mb-0">
          <div>
            {{ item.email }}
          </div>
          <b-button v-if="item.active" type="is-danger" @click="removeRequestRecruiter(item)">Remove</b-button>
          <b-button v-else type="is-primary is-light" @click="addRequestRecruiter(item)">Add</b-button>
        </li>
      </ul>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from "@/utils/toast";
import { getAgencyPersonnel } from "@/api/agencyApi";
import { postAgencyRequestRecruiter, deleteAgencyRequestRecruiter } from "@/api/agencyRequestApi";

const props = defineProps<{ request: any; recruiters: any[] }>();
const emit = defineEmits<{
  (e: 'selectUser', item: any): void;
  (e: 'removeUser', item: any): void;
}>();

const isLoading = ref(false);
const data = ref<any[] | null>(null);

function loadAgencyPersonnel() {
  isLoading.value = true;
  getAgencyPersonnel()
    .then((response) => {
      isLoading.value = false;
      data.value = response.map((item: any) => ({ ...item, active: false, recruiterId: null }));
      updateRecruiters(props.recruiters);
    })
    .catch((error) => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function updateRecruiters(items: any[]) {
  if (!data.value) return;
  for (let i = 0; i < items.length; i++) {
    for (let j = 0; j < data.value.length; j++) {
      if (items[i].toLowerCase() === data.value[j].name.toLowerCase()) {
        data.value[j].active = true;
      }
    }
  }
}

function addRequestRecruiter(item: any) {
  isLoading.value = true;
  postAgencyRequestRecruiter(props.request.id, { recruiterId: item.id }).then(() => {
    isLoading.value = false;
    item.active = true;
    item.recruiterId = item.id;
    emit('selectUser', item);
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error);
  });
}

function removeRequestRecruiter(item: any) {
  isLoading.value = true;
  deleteAgencyRequestRecruiter(props.request.id, item.id).then(() => {
    isLoading.value = false;
    item.active = false;
    emit('removeUser', item);
  }).catch((error) => {
    isLoading.value = false;
    showAlertError(error);
  });
}

loadAgencyPersonnel();
</script>
