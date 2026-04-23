<template>
  <div class="container-flex">
    <b-loading v-model="isLoading"></b-loading>
    <!-- Detail -->
    <section class="col-md-8 col-sm-12 p-3 pr-5">
      <!-- Highlight -->
      <contact-information v-if="props.company" :company="props.company" @update:company="$emit('update:company', $event)" />

      <!-- Detail -->
      <table class="table-detail" v-if="props.company">
        <tr v-if="props.company.industry">
          <td><span class="fw-700">Industry </span></td>
          <td>
            <span>
              {{ props.company.industry.industry ? props.company.industry.industry.value : props.company.industry.otherIndustry }}
            </span>
          </td>
        </tr>
        <tr>
          <td>
            <span class="fw-700">{{ "Vaccination Required" }}</span>
          </td>
          <td>
            {{ getLabelVaccinationRequired(localCompany.vaccinationRequired) }}
            {{ localCompany.vaccinationRequiredComments ? "|" : "" }}
            {{ localCompany.vaccinationRequiredComments }}
            <b-button type="is-ghost" @click="showEditVaccinationRequired = true" icon-right="pencil"></b-button>
          </td>
        </tr>
      </table>

      <!-- About -->
      <section class="margin-top-15 mb-4" v-if="props.company">
        <span class="fw-700">About</span>
        <pre class="long-description">{{ props.company.about }} </pre>
      </section>

      <section class="margin-top-15 mb-4" v-if="props.company">
        <span class="fw-700">Internal Info</span>
        <pre class="long-description" v-html="props.company.internalInfo"></pre>
      </section>

      <span class="line-gray mb-5"></span>


      <!-- Documents -->
      <section class="margin-top-15 mb-4">
        <documents />
      </section>

      <div class="mb-5">
        <div class="button-right">
          <span class="fw-700">{{ "Invoice notes " }}</span>
          <button class="show-notes-btn" @click="showNotesEditor()">
            <img src="../../assets/images/right-arrow.svg" alt="edit" :class="{ open: showEditor }" />
          </button>
        </div>
        <span class="line-gray"></span>

        <div class="vue-trix-editor">
          <transition name="fade">
            <div v-if="showEditor">
              <QuillEditor theme="snow" content-type="html" v-model:content="editorContent" :toolbar="customToolbar" />
              <br />
              <button class="sm-save-button" v-if="editorContent" @click="saveInvoiceNotes()">
                {{ "Save" }}
              </button>
            </div>
          </transition>
        </div>
      </div>

      <div>
        <div class="button-right">
          <span class="fw-700">Invoice Recipients</span>
          <button class="show-notes-btn" @click="loadCompanyInvoiceRecipients()">
            <img src="../../assets/images/right-arrow.svg" :class="{ open: showRecipients }" />
          </button>
        </div>
        <span class="line-gray"></span>

        <div class="vue-trix-editor">
          <transition name="fade">
            <div v-if="showRecipients">
              <ul class="list-recipients">
                <li v-for="(item, index) in companyRecipients" :key="'companyRecipients' + index">
                  <email-card :item="item" :index="index"
                    @updateDataEmailList="(index) => deleteCompanyInvoiceRecipientArray(index)"></email-card>
                </li>
                <li class="newRecipient">
                  <div class="container-card">
                    <label>{{ "Name" }}:
                      <input type="text" v-model="name" placeholder="Name" name="name"
                        :class="{ 'is-danger': !!formErrors.name }" />
                      <span v-show="formErrors.name" class="help is-danger no-margin">
                        {{ formErrors.name }}
                      </span>
                    </label>

                    <label>{{ "Email" }}:
                      <input type="text" v-model="email" placeholder="Email" name="email"
                        :class="{ 'is-danger': !!formErrors.email }" />
                      <span v-show="formErrors.email" class="help is-danger no-margin">
                        {{ formErrors.email }}
                      </span>
                    </label>
                  </div>
                  <div class="actions">
                    <button @click="validateCreateEmail()">
                      <img src="../../assets/images/checked.png" alt="edit" />
                    </button>
                  </div>
                </li>
              </ul>
            </div>
          </transition>
        </div>
      </div>

      <i class="fz-1 op5" v-if="props.company && props.company.createdAt">
        Created: {{ date(props.company.createdAt) }}
      </i>
    </section>

    <aside class="col-md-4 col-sm-12 p-3">
      <notes />
      <location />
    </aside>

    <b-modal v-model="showEditVaccinationRequired" width="500px" v-if="props.company">
      <edit-vaccination-required :company-profile-id="props.company.id" :vaccination-required="props.company.vaccinationRequired"
        :vaccination-comments="props.company.vaccinationRequiredComments" @updated="vaccinationRequiredUpdated" />
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRoute } from 'vue-router';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { date } from "@/utils/filters";
import {
  getInvoiceNotes,
  postInvoiceNotes,
  getCompanyInvoiceRecipients,
  postCompanyInvoiceRecipient,
} from "@/api/agencyCompanyApi";
import EmailCard from "@/components/EmailCard.vue";
import Location from "../../components/agency_company/LocationDetail.vue";
import ContactInformation from "./ContactInformation.vue";
import Documents from "../../components/agency_company/Documents.vue";
import Notes from "../../components/agency_company/CompanyNotes.vue";
import EditVaccinationRequired from "@/components/agency_company/EditVaccinationRequired.vue";

const recipientSchema = yup.object({
  name: yup.string().required('Name is required').min(3, 'Min 3 characters').max(50, 'Max 50 characters'),
  email: yup.string().required('Email is required').email('Invalid email').min(6).max(50),
});

const props = defineProps<{ company: any }>();
const emit = defineEmits<{ (e: 'update:company', value: any): void }>();

const route = useRoute();

const form = useStickyForm<{ name: string; email: string }>({
  schema: recipientSchema,
  initialValues: { name: '', email: '' },
});
const { name, email } = form.fields;
const formErrors = form.errors;

const isLoading = ref(false);
const localCompany = ref<any>(JSON.parse(JSON.stringify(props.company)));
const editorContent = ref<string | null>(null);
const showEditor = ref(false);
const showRecipients = ref(false);
const showEditVaccinationRequired = ref(false);
const customToolbar = [
  ["bold", "italic", "underline", "strike"],
  [
    { align: "" },
    { align: "center" },
    { align: "right" },
    { align: "justify" },
  ],
  [{ header: 1 }, { header: 2 }],
  [{ list: "ordered" }, { list: "bullet" }],
  [{ script: "sub" }, { script: "super" }],
  ["clean"],
];
const companyRecipients = ref<any[]>([]);

watch(() => props.company, (newVal) => {
  localCompany.value = JSON.parse(JSON.stringify(newVal));
}, { deep: true });

function showNotesEditor() {
  if (showEditor.value) {
    showEditor.value = false;
  } else {
    if (!editorContent.value) {
      loadInvoiceNotes();
    } else {
      showEditor.value = true;
    }
  }
}

function loadInvoiceNotes() {
  isLoading.value = true;
  getInvoiceNotes(route.params.id)
    .then((response) => {
      editorContent.value = response.htmlNotes;
      showEditor.value = true;
      isLoading.value = false;
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function saveInvoiceNotes() {
  const result = (editorContent.value || '').replace(/(<([^>]+)>)/gi, "");
  if (result.length > 500) {
    showAlertError("Notes can't be greater that 500 characters.");
  } else {
    isLoading.value = true;
    postInvoiceNotes(route.params.id, { htmlNotes: editorContent.value })
      .then(() => {
        showAlertSuccess("Updated");
        isLoading.value = false;
      })
      .catch((error) => {
        showAlertError(error);
        isLoading.value = false;
      });
  }
}

function loadCompanyInvoiceRecipients() {
  if (!showRecipients.value) {
    isLoading.value = true;
    showRecipients.value = true;
    getCompanyInvoiceRecipients(route.params.id)
      .then((response) => {
        isLoading.value = false;
        companyRecipients.value = response;
      })
      .catch((error) => {
        showAlertError(error);
        isLoading.value = false;
      });
  } else {
    showRecipients.value = false;
    form.resetAll();
  }
}

function saveCompanyInvoiceRecipient(values: { name: string; email: string }) {
  isLoading.value = true;
  postCompanyInvoiceRecipient(route.params.id, values)
    .then((response) => {
      companyRecipients.value.push({
        id: response.id,
        name: values.name,
        email: values.email,
      });
      form.resetAll();
      isLoading.value = false;
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function deleteCompanyInvoiceRecipientArray(index: number) {
  companyRecipients.value.splice(index, 1);
}

function validateCreateEmail() {
  form.markInteracted(['name', 'email']);
  form.handleSubmit((values) => {
    saveCompanyInvoiceRecipient(values);
  })();
}

function getLabelVaccinationRequired(vaccinationRequired: boolean | null | undefined) {
  if (vaccinationRequired == null) return "";
  return vaccinationRequired ? "Yes" : "No";
}

function vaccinationRequiredUpdated(model: { required: boolean; comments: string | null }) {
  showEditVaccinationRequired.value = false;
  localCompany.value.vaccinationRequired = model.required;
  localCompany.value.vaccinationRequiredComments = model.comments;
  emit('update:company', localCompany.value);
}
</script>
