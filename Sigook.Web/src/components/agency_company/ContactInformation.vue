<template>
  <div>
    <div class="highlight-content" v-if="company">
      <div class="d-inline-flex relative mr-3">
        <div class="item">
          <span class="fw-700">{{ 'Phone' }}</span>
          <p v-if="company.phone">
            {{ company.phone }}
            <i v-show="company.phoneExt">
              {{ 'Ext.' }} {{ company.phoneExt }}
            </i>
          </p>
          <p v-else class="op3">Phone</p>
        </div>

        <div class="item" v-if="company.fax">
          <span class="fw-700">{{ 'Fax' }}</span>
          <p>{{ company.fax }}
            <i v-show="company.faxExt">
              {{ 'Ext.' }} {{ company.faxExt }}</i>
          </p>
        </div>

        <div class="item" v-if="company.website">
          <span class="fw-700">{{ 'Website' }}</span>
          <p class="ellipsis-150 block">
            <a :href="getFullUrl(company.website)" target="_blank">{{ company.website }}</a>
          </p>
        </div>
        <button class="btn-icon-sm btn-icon-edit bg-transparent" @click="showModal = true">{{ "Edit" }}</button>
      </div>
      <div class="d-inline-flex relative">
        <div class="item">
          <span class="fw-700">{{ 'Email' }}</span>
          <p class="word-break">{{ company.email }}</p>
        </div>
        <button class="btn-icon-sm btn-icon-edit bg-transparent" @click="showModalUpdateEmail = true">
          {{ "Edit" }}
        </button>
      </div>
    </div>

    <b-modal v-model="showModal" width="800px">
      <contact-information-form :model="company" @update:model="$emit('update:company', $event)" @save="closeEditModal" />
    </b-modal>


    <b-modal v-model="showModalUpdateEmail" width="500px">
      <dialog-company-update-email :company-profile-id="company.id" @closeModal="closeEditEmailModal" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { useRoute } from 'vue-router';
import DialogCompanyUpdateEmail from "@/components/company/DialogCompanyUpdateEmail.vue";
import ContactInformationForm from "@/components/agency_company/ContactInformationForm.vue";

const props = defineProps<{ company: any }>();
const emit = defineEmits<{ (e: 'update:company', value: any): void }>();

const route = useRoute();

const showModal = ref(false);
const showModalUpdateEmail = ref(false);
const localCompany = ref<any>(JSON.parse(JSON.stringify(props.company)));
const model = reactive<any>({
  phone: props.company.phone,
  phoneExt: props.company.phoneExt,
  fax: props.company.fax,
  faxExt: props.company.faxExt,
  website: props.company.website,
});
const profileId = route.params.id;

watch(() => props.company, (newVal) => {
  localCompany.value = JSON.parse(JSON.stringify(newVal));
}, { deep: true });

function closeEditModal() {
  showModal.value = false;
  model.phone = props.company.phone;
  model.phoneExt = props.company.phoneExt;
  model.fax = props.company.fax;
  model.faxExt = props.company.faxExt;
  model.website = props.company.website;
}

function closeEditEmailModal(_closeModal: any, newEmail: string) {
  showModalUpdateEmail.value = false;
  if (newEmail) {
    localCompany.value.email = newEmail;
    emit('update:company', localCompany.value);
  }
}

function getFullUrl(url: string) {
  if (url.includes('http')) {
    return url;
  }
  return `https://${url}`;
}

// Preserve profileId reference for potential template use
void profileId;
</script>
