<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="openAddContactModal">Add</b-button>
    </b-field>
    <b-table sticky-header height="var(--grid-height)" :data="contactPeople" narrowed hoverable paginated pagination-size="is-small"
      pagination-rounded>
      <template v-slot:empty>
        <p class="container has-text-centered">No records available</p>
      </template>
      <template>
        <b-table-column field="firstName" label="Full Name" v-slot="props">
          {{ props.row.firstName }} {{ props.row.middleName }} {{ props.row.lastName }}
        </b-table-column>
        <b-table-column field="position" label="Position" v-slot="props">
          {{ props.row.position }}
        </b-table-column>
        <b-table-column field="officeNumber" label="Phone Number" v-slot="props">
          <p>{{ props.row.mobileNumber }}</p>
          <p>
            <span>{{ props.row.officeNumber }}</span>
            <span v-if="props.row.officeNumberExt">Ext. {{ props.row.officeNumberExt }}</span>
          </p>
        </b-table-column>
        <b-table-column field="email" label="Email" v-slot="props">
          <p>{{ props.row.email }}</p>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-button type="is-danger" outlined rounded icon-right="delete"
            @click="removeLine(props.row.id)" />
        </b-table-column>
      </template>
    </b-table>
    <b-modal custom-content-class="card" v-model="showModal">
      <div class="p-3">
        <div class="columns is-multiline">
          <div class="column is-6">
            <b-field label="Title" :type="formErrors.title ? 'is-danger' : ''"
              :message="formErrors.title || ''">
              <b-select v-model="title" name="title" expanded>
                <option v-for="(item, index) in titleOptions" :key="'title' + index" :value="item">
                  {{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="column is-6">
            <b-field label="First Name" :type="formErrors.firstName ? 'is-danger' : ''"
              :message="formErrors.firstName || ''">
              <b-input v-model="firstName" name="first name" />
            </b-field>
          </div>
          <div class="column is-6">
            <b-field label="Middle Name" :type="formErrors.middleName ? 'is-danger' : ''"
              :message="formErrors.middleName || ''">
              <b-input v-model="middleName" name="middle name" />
            </b-field>
          </div>
          <div class="column is-6">
            <b-field label="Last Name" :type="formErrors.lastName ? 'is-danger' : ''"
              :message="formErrors.lastName || ''">
              <b-input v-model="lastName" name="last name" />
            </b-field>
          </div>
          <div class="column is-6">
            <b-field label="Email" :type="formErrors.email ? 'is-danger' : ''"
              :message="formErrors.email || ''">
              <b-input v-model="email" name="email" />
            </b-field>
          </div>
          <div class="column is-6">
            <b-field label="Position" :type="formErrors.position ? 'is-danger' : ''"
              :message="formErrors.position || ''">
              <b-input v-model="position" name="position" />
            </b-field>
          </div>
          <div class="column is-6 is-4-desktop">
            <PhoneInput ref="mobileComponent" :required="true" :defaultValue="mobileNumber"
              model="Mobile Number" @formattedPhone="(phone: string) => mobileNumber = phone" />
          </div>
          <div class="column is-6 is-4-desktop">
            <PhoneInput ref="officeComponent" :required="false" :defaultValue="officeNumber"
              model="Office Number" @formattedPhone="(phone: string) => officeNumber = phone" />
          </div>
          <div class="column is-6 is-4-desktop">
            <b-field label="Extension" :type="formErrors.officeNumberExt ? 'is-danger' : ''"
              :message="formErrors.officeNumberExt || ''">
              <b-input v-model="officeNumberExt" name="officeNumberExt" />
            </b-field>
          </div>
        </div>
        <div class="">
          <b-button type="is-primary" @click="validateForm">SAVE</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import * as yup from 'yup';
import PhoneInput from "../../components/PhoneInput.vue";
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getContactPeople, deleteContactPerson, saveContactPerson } from '@/api/companyApi';

defineProps<{ companyData?: any; isDisabled?: boolean }>();

const numericExt = yup
  .string()
  .nullable()
  .transform((v) => (v === '' ? null : v))
  .matches(/^\d{1,8}$/, { message: 'Must be 1-8 digits', excludeEmptyString: true });

const schema = yup.object({
  title: yup.string().required('Title is required'),
  firstName: yup.string().required('First name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  middleName: yup.string().nullable().transform((v) => (v === '' ? null : v)).min(1, 'Min 1 character').max(20, 'Max 20 characters'),
  lastName: yup.string().required('Last name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
  email: yup.string().required('Email is required').email('Invalid email'),
  position: yup.string().required('Position is required').min(3, 'Min 3 characters').max(100, 'Max 100 characters'),
  mobileNumber: yup.string().nullable(),
  officeNumber: yup.string().nullable(),
  officeNumberExt: numericExt,
});

const form = useStickyForm<{
  title: string; firstName: string; middleName: string; lastName: string;
  email: string; position: string; mobileNumber: string; officeNumber: string; officeNumberExt: string;
}>({
  schema,
  initialValues: {
    title: '',
    firstName: '',
    middleName: '',
    lastName: '',
    email: '',
    position: '',
    mobileNumber: '',
    officeNumber: '',
    officeNumberExt: '',
  },
});
const {
  title, firstName, middleName, lastName, email, position,
  mobileNumber, officeNumber, officeNumberExt,
} = form.fields;
const formErrors = form.errors;

const titleOptions = ['Mr', 'Mrs', 'Ms', 'Miss', 'Mx', 'Master', 'Madam'];

const isLoading = ref(false);
const showModal = ref(false);
const contactPeople = ref<any[]>([]);
const mobileComponent = ref<any>(null);
const officeComponent = ref<any>(null);

function openAddContactModal() {
  form.resetAll();
  showModal.value = true;
}

async function getContactPersons() {
  contactPeople.value = await getContactPeople();
}

async function removeLine(id: any) {
  await deleteContactPerson(id);
  await getContactPersons();
}

async function validateForm() {
  form.markInteracted();
  const phoneValid = await mobileComponent.value?.validatePhone();
  const officeValid = await officeComponent.value?.validatePhone();
  form.handleSubmit((values) => {
    if (!phoneValid || !officeValid) return;
    isLoading.value = true;
    const contact = {
      ...values,
      officeNumberExt: values.officeNumberExt ? parseInt(values.officeNumberExt, 10) : null,
    };
    saveContactPerson(contact)
      .then(async () => {
        isLoading.value = false;
        showModal.value = false;
        form.resetAll();
        await getContactPersons();
        showAlertSuccess('Profile updated');
      })
      .catch((error: unknown) => {
        isLoading.value = false;
        showAlertError((error as { data?: unknown }).data);
      });
  })();
}

(async () => {
  await getContactPersons();
})();
</script>
