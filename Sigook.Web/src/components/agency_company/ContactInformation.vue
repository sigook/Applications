<template>
  <div>
    <div class="col-12 col-padding highlight-content" v-if="company">
      <div class="item">
        <span class="fw-bold">{{ 'Phone' }}</span>
        <p v-if="company.phone">
          {{ company.phone }}
          <i v-show="company.phoneExt">
            {{ 'Ext.' }} {{ company.phoneExt }}
          </i>
        </p>
        <p v-else class="op3">Phone</p>
      </div>

      <div class="item" v-if="company.fax">
        <span class="fw-bold">{{ 'Fax' }}</span>
        <p>{{ company.fax }}
          <i v-show="company.faxExt">
            {{ 'Ext.' }} {{ company.faxExt }}</i>
        </p>
      </div>

      <div class="item" v-if="company.website">
        <span class="fw-bold">{{ 'Website' }}</span>
        <p class="d-flex align-items-center">
          <a :href="getFullUrl(company.website)" target="_blank">{{ company.website }}</a>
          <b-button type="is-ghost" icon-left="pencil" class="ms-1" @click="showModal = true" />
        </p>
      </div>

      <div class="item">
        <span class="fw-bold">{{ 'Email' }}</span>
        <p class="d-flex align-items-center">
          {{ company.email }}
          <b-button type="is-ghost" icon-left="pencil" class="ms-1" @click="showModalUpdateEmail = true" />
        </p>
      </div>

      <div class="item">
        <span class="fw-bold">{{ 'Vaccination Required' }}</span>
        <p class="d-flex align-items-center">
          {{ getLabelVaccinationRequired(company.vaccinationRequired) }}
          <b-button type="is-ghost" icon-left="pencil" class="ms-1" @click="showEditVaccinationRequired = true" />
        </p>
      </div>
    </div>

    <b-modal v-model="showModal" width="800px">
      <contact-information-form :model="company" @update:model="$emit('update:company', $event)" @save="closeEditModal" />
    </b-modal>


    <b-modal v-model="showModalUpdateEmail" width="500px">
      <dialog-company-update-email :company-profile-id="company.id" @closeModal="closeEditEmailModal" />
    </b-modal>

    <b-modal v-model="showEditVaccinationRequired" width="500px" v-if="company">
      <edit-vaccination-required :company-profile-id="company.id" :vaccination-required="company.vaccinationRequired"
        :vaccination-comments="company.vaccinationRequiredComments" @updated="vaccinationRequiredUpdated" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { useRoute } from 'vue-router';
import DialogCompanyUpdateEmail from "@/components/company/DialogCompanyUpdateEmail.vue";
import ContactInformationForm from "@/components/agency_company/ContactInformationForm.vue";
import EditVaccinationRequired from "@/components/agency_company/EditVaccinationRequired.vue";

const props = defineProps<{ company: any }>();
const emit = defineEmits<{ (e: 'update:company', value: any): void }>();

const route = useRoute();

const showModal = ref(false);
const showModalUpdateEmail = ref(false);
const showEditVaccinationRequired = ref(false);
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

function getLabelVaccinationRequired(vaccinationRequired: boolean | null | undefined) {
  return vaccinationRequired ? "Yes" : "No";
}

function vaccinationRequiredUpdated(model: { required: boolean; comments: string | null }) {
  showEditVaccinationRequired.value = false;
  localCompany.value.vaccinationRequired = model.required;
  localCompany.value.vaccinationRequiredComments = model.comments;
  emit('update:company', localCompany.value);
}

// Preserve profileId reference for potential template use
void profileId;
</script>

<style scoped>
.highlight-content .item p {
  display: flex;
  align-items: center;
  min-height: 38px;
  margin: 0;
}
</style>
