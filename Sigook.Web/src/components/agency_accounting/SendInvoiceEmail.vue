<template>
  <div class="p-3">
    <b-loading v-model="isLoading"></b-loading>
    <div class="container-flex">
      <div class="col-12 col-padding">
        <b-field label="To">
          <b-input disabled :value="invoice.email"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Cc" :type="emailValidationType || ''"
          :message="emailValidationMessage">
          <b-taginput v-model="invoiceRecipients" autocomplete field="email" append-to-body allow-new
            :before-adding="validateEmail" :create-tag="createTag" placeholder="Add email..."
            name="recipients"></b-taginput>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Subject" :type="formErrors.subject ? 'is-danger' : ''"
          :message="formErrors.subject || ''">
          <b-input v-model="subject" placeholder="Subject" name="subject"></b-input>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Attachments">
          <b-upload v-model="newEmail.attachments" multiple>
            <div class="expandable-section-container">
              <div class="expandable-section-header">
                <h3 class="expandable-section-title fz1 fw-600 mb-2 text-center">
                  <b-icon icon="plus-circle" class="mr-2"></b-icon>
                  Add Attachments
                </h3>
                <p class="fz-1 color-gray mb-0 text-center">Click here to add extra files or documents to this email</p>
              </div>
            </div>
          </b-upload>
        </b-field>
      </div>
      <div class="col-12 col-padding" v-if="newEmail.attachments.length > 0">
        <b-taglist>
          <b-tag v-for="file in newEmail.attachments" :key="file.name" type="is-info" closable
            @close="removeFile(file)">
            {{ file.name }}
          </b-tag>
        </b-taglist>
      </div>
      <div class="col-12 col-padding">
        <b-field label="Body" :type="formErrors.body ? 'is-danger' : ''"
          :message="formErrors.body || ''">
          <div class="vue-trix-editor">
            <div>
              <QuillEditor theme="snow" content-type="html" v-model:content="body" />
            </div>
          </div>
        </b-field>
      </div>
      <div class="col-12 col-padding">
        <b-button type="is-primary" @click="sendEmail">Send Email</b-button>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import * as yup from 'yup';
import { useStickyForm } from '@/composables/useStickyForm';
import { showAlertError } from "@/utils/toast";
import { sendInvoiceEmail } from "@/api/agencyInvoiceApi";
import { getCompanyInvoiceRecipients } from "@/api/agencyCompanyApi";

const emailSchema = yup.object({
  subject: yup.string().required('Subject is required').min(2).max(100, 'Max 100 characters'),
  body: yup
    .string()
    .required('Body is required')
    .test('not-empty', 'Body is required', (v) => !!(v || '').replace(/<[^>]*>/g, '').trim()),
});

const defaultBody = `<p>Good Morning,</p>
        <p>Find your invoice attached, please confirm you have received it. <strong>Your timely payment is greatly appreciated.</strong></p>
        <p>We thank you in advance for your continued business, should you have any further requirements or if you have any questions please do not hesitate to contact us.</p>
        <p>Regards,</p>`;

export default {
  props: ["invoice"],
  setup(props) {
    const form = useStickyForm({
      schema: emailSchema,
      initialValues: {
        subject: `Invoice ${props.invoice.invoiceNumber} - ${props.invoice.companyFullName}`,
        body: defaultBody,
      },
    });

    return {
      ...form.fields,
      formErrors: form.errors,
      handleSubmit: form.handleSubmit,
      markInteracted: form.markInteracted,
    };
  },
  data() {
    return {
      isLoading: true,
      invoiceRecipients: [],
      emailValidationMessage: '',
      emailValidationType: '',
      newEmail: {
        attachments: [] as any[]
      }
    };
  },
  methods: {
    async loadInvoiceRecipients() {
      this.invoiceRecipients = await getCompanyInvoiceRecipients(this.invoice.companyProfileId);
    },
    createTag(email) {
      const recipient = { email };
      this.invoiceRecipients.push(recipient);
      return recipient;
    },
    validateEmail(email) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(email)) {
        this.emailValidationMessage = 'Please enter a valid email address';
        this.emailValidationType = 'is-danger';
        setTimeout(() => {
          this.emailValidationMessage = '';
          this.emailValidationType = '';
        }, 3000);
        return false;
      }
      this.emailValidationMessage = '';
      this.emailValidationType = '';
      return true;
    },
    removeFile(file) {
      this.newEmail.attachments = this.newEmail.attachments.filter(f => f !== file);
    },
    sendEmail() {
      this.markInteracted(['subject', 'body']);
      this.handleSubmit((values) => {
        this.isLoading = true;
        sendInvoiceEmail({
          invoiceId: this.invoice.id,
          recipients: this.invoiceRecipients.map(recipient => recipient.email),
          subject: values.subject,
          body: values.body,
          attachments: this.newEmail.attachments
        }).then(() => {
          this.isLoading = false;
          this.$emit('sent');
        }).catch(error => {
          this.isLoading = false;
          showAlertError(error);
        });
      })();
    }
  },
  async created() {
    await this.loadInvoiceRecipients();
    this.isLoading = false;
  }
};
</script>

<style scoped>
.col-12 .upload {
  width: 100%;
  display: block;
}

.col-12 .upload>>>.upload-draggable {
  width: 100%;
  display: block;
}

.col-12 .upload>>>.file-input {
  width: 100%;
}
</style>