<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-field grouped position="is-right">
      <b-button type="is-ghost" icon-right="plus-circle" @click="openAddContactModal">Add</b-button>
    </b-field>
    <b-table :data="contactPeople" narrowed hoverable :mobile-cards="false" paginated
      pagination-rounded>
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
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
    <b-modal v-model="showModal">
      <div class="p-3">
        <div class="container-flex">
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Title" :type="formErrors.title ? 'is-danger' : ''"
              :message="formErrors.title || ''">
              <b-select v-model="title" name="title" expanded>
                <option v-for="(item, index) in titleOptions" :key="'title' + index" :value="item">
                  {{ item }}
                </option>
              </b-select>
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="First Name" :type="formErrors.firstName ? 'is-danger' : ''"
              :message="formErrors.firstName || ''">
              <b-input v-model="firstName" name="first name" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Middle Name" :type="formErrors.middleName ? 'is-danger' : ''"
              :message="formErrors.middleName || ''">
              <b-input v-model="middleName" name="middle name" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Last Name" :type="formErrors.lastName ? 'is-danger' : ''"
              :message="formErrors.lastName || ''">
              <b-input v-model="lastName" name="last name" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Email" :type="formErrors.email ? 'is-danger' : ''"
              :message="formErrors.email || ''">
              <b-input v-model="email" name="email" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-6 col-padding">
            <b-field label="Position" :type="formErrors.position ? 'is-danger' : ''"
              :message="formErrors.position || ''">
              <b-input v-model="position" name="position" />
            </b-field>
          </div>
          <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
            <phone-input ref="mobileComponent" :required="true" :defaultValue="mobileNumber"
              model="Mobile Number" @formattedPhone="(phone) => mobileNumber = phone" />
          </div>
          <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
            <phone-input ref="officeComponent" :required="false" :defaultValue="officeNumber"
              model="Office Number" @formattedPhone="(phone) => officeNumber = phone" />
          </div>
          <div class="col-sm-12 col-md-6 col-lg-4 col-padding">
            <b-field label="Extension" :type="formErrors.officeNumberExt ? 'is-danger' : ''"
              :message="formErrors.officeNumberExt || ''">
              <b-input v-model="officeNumberExt" name="officeNumberExt" />
            </b-field>
          </div>
        </div>
        <div class="col-12 col-padding">
          <b-button type="is-primary" @click="validateForm">SAVE</b-button>
        </div>
      </div>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { getContactPeople, deleteContactPerson, saveContactPerson } from '@/api/companyApi';

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

export default {
  props: ['companyData', 'isDisabled'],
  setup() {
    const form = useStickyForm({
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

    const titleOptions = ['Mr', 'Mrs', 'Ms', 'Miss', 'Mx', 'Master', 'Madam'];

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
      resetForm: form.resetAll,
      titleOptions,
    };
  },
  data() {
    return {
      isLoading: false,
      showModal: false,
      contactPeople: [] as any[],
    };
  },
  components: {
    phoneInput: defineAsyncComponent(() => import("../../components/PhoneInput.vue")),
  },
  methods: {
    openAddContactModal() {
      this.resetForm();
      this.showModal = true;
    },
    async getContactPersons() {
      this.contactPeople = await getContactPeople();
    },
    async removeLine(id: any) {
      await deleteContactPerson(id);
      await this.getContactPersons();
    },
    async validateForm() {
      this.markInteracted();
      const phoneValid = await (this.$refs.mobileComponent as any).validatePhone();
      const officeValid = await (this.$refs.officeComponent as any).validatePhone();
      this.handleSubmit((values) => {
        if (!phoneValid || !officeValid) return;
        this.isLoading = true;
        const contact = {
          ...values,
          officeNumberExt: values.officeNumberExt ? parseInt(values.officeNumberExt, 10) : null,
        };
        saveContactPerson(contact)
          .then(async () => {
            this.isLoading = false;
            this.showModal = false;
            this.resetForm();
            await this.getContactPersons();
            showAlertSuccess('Profile updated');
          })
          .catch((error: any) => {
            this.isLoading = false;
            showAlertError(error.data);
          });
      })();
    },
  },
  async created() {
    await this.getContactPersons();
  },
};
</script>
