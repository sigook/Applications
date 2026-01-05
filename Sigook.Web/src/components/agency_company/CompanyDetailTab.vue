<template>
  <div class="container-flex">
    <b-loading v-model="isLoading"></b-loading>
    <!-- Detail -->
    <section class="col-md-8 col-sm-12 p-3 pr-5">
      <!-- Highlight -->
      <contact-information v-if="company" :company="company" />

      <!-- Detail -->
      <table class="table-detail">
        <tr v-if="company.industry">
          <td><span class="fw-700">Industry </span></td>
          <td>
            <span>
              {{ company.industry.industry ? company.industry.industry.value : company.industry.otherIndustry }}
            </span>
          </td>
        </tr>
        <tr>
          <td>
            <span class="fw-700">{{ $t("VaccinationRequired") }}</span>
          </td>
          <td>
            {{ getLabelVaccinationRequired(company.vaccinationRequired) }}
            {{ company.vaccinationRequiredComments ? "|" : "" }}
            {{ company.vaccinationRequiredComments }}
            <b-button type="is-ghost" @click="showEditVaccinationRequired = true" icon-right="pencil"></b-button>
          </td>
        </tr>
      </table>

      <!-- About -->
      <section class="margin-top-15 mb-4">
        <span class="fw-700">About</span>
        <pre class="long-description">{{ company.about }} </pre>
      </section>

      <section class="margin-top-15 mb-4">
        <span class="fw-700">Internal Info</span>
        <pre class="long-description" v-html="company.internalInfo"></pre>
      </section>

      <span class="line-gray mb-5"></span>


      <!-- Documents -->
      <section class="margin-top-15 mb-4">
        <documents />
      </section>

      <div class="mb-5">
        <div class="button-right">
          <span class="fw-700">{{ $t("InvoiceNotes") }}</span>
          <button class="show-notes-btn" @click="showNotesEditor()">
            <img src="../../assets/images/right-arrow.svg" alt="edit" :class="{ open: showEditor }" />
          </button>
        </div>
        <span class="line-gray"></span>

        <div class="vue-trix-editor">
          <transition name="fade">
            <div v-if="showEditor">
              <vue-editor v-model="editorContent" :editorToolbar="customToolbar"></vue-editor>
              <br />
              <button class="sm-save-button" v-if="editorContent" @click="postInvoiceNotes()">
                {{ $t("Save") }}
              </button>
            </div>
          </transition>
        </div>
      </div>

      <div>
        <div class="button-right">
          <span class="fw-700">Invoice Recipients</span>
          <button class="show-notes-btn" @click="getCompanyInvoiceRecipients()">
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
                    <label>{{ $t("Name") }}:
                      <input type="text" v-model="newRecipient.name" placeholder="Name" name="name"
                        v-validate="'required|max:50|min:3'" :class="{ 'is-danger': errors.has('name') }" />
                      <span v-show="errors.has('name')" class="help is-danger no-margin">
                        {{ errors.first("name") }}
                      </span>
                    </label>

                    <label>{{ $t("Email") }}:
                      <input type="text" v-model="newRecipient.email" placeholder="Email" name="email"
                        v-validate="'required|max:50|min:6|email'" :class="{ 'is-danger': errors.has('email') }" />
                      <span v-show="errors.has('email')" class="help is-danger no-margin">
                        {{ errors.first("email") }}
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

      <i class="fz-1 op5" v-if="company.createdAt">
        Created: {{ company.createdAt | date }}
      </i>
    </section>

    <aside class="col-md-4 col-sm-12 p-3">
      <notes />
      <location />
    </aside>

    <b-modal v-model="showEditVaccinationRequired" width="500px">
      <edit-vaccination-required :company-profile-id="company.id" :vaccination-required="company.vaccinationRequired"
        :vaccination-comments="company.vaccinationRequiredComments" @updated="vaccinationRequiredUpdated" />
    </b-modal>
  </div>
</template>

<script>
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  props: ["company"],
  data() {
    return {
      isLoading: false,
      editorContent: null,
      showEditor: false,
      showRecipients: false,
      showEditVaccinationRequired: false,
      customToolbar: [
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
      ],
      companyRecipients: [],
      newRecipient: {
        name: "",
        email: "",
      },
    };
  },
  components: {
    EmailCard: () => import("@/components/EmailCard"),
    ContactPerson: () => import("../../components/agency_company/ContactPersonList"),
    Location: () => import("../../components/agency_company/LocationDetail"),
    ContactInformation: () => import("./ContactInformation"),
    Documents: () => import("../../components/agency_company/Documents"),
    Notes: () => import("../../components/agency_company/CompanyNotes"),
    EditVaccinationRequired: () => import("@/components/agency_company/EditVaccinationRequired")
  },
  mixins: [billingAdminMixin],
  methods: {
    showNotesEditor() {
      if (this.showEditor) {
        this.showEditor = false;
      } else {
        if (!this.editorContent) {
          this.getInvoiceNotes();
        } else {
          this.showEditor = true;
        }
      }
    },
    getInvoiceNotes() {
      this.isLoading = true;
      this.$store.dispatch("agency/getInvoiceNotes", this.$route.params.id)
        .then((response) => {
          this.editorContent = response.htmlNotes;
          this.showEditor = true;
          this.isLoading = false;
        })
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    postInvoiceNotes() {
      // to check the size
      let result = this.editorContent.replace(/(<([^>]+)>)/gi, "");
      if (result.length > 500) {
        this.showAlertError(this.$t("ErrorLong"));
      } else {
        this.isLoading = true;
        this.$store
          .dispatch("agency/postInvoiceNotes", {
            id: this.$route.params.id,
            model: { htmlNotes: this.editorContent },
          })
          .then(() => {
            this.showAlertSuccess(this.$t("Updated"));
            this.isLoading = false;
          })
          .catch((error) => {
            this.showAlertError(error);
            this.isLoading = false;
          });
      }
    },
    getCompanyInvoiceRecipients() {
      if (!this.showRecipients) {
        this.isLoading = true;
        this.showRecipients = true;
        this.$store.dispatch("agency/getCompanyInvoiceRecipients", this.$route.params.id)
          .then((response) => {
            this.isLoading = false;
            this.companyRecipients = response;
          })
          .catch((error) => {
            this.showAlertError(error);
            this.isLoading = false;
          });
      } else {
        this.showRecipients = false;
        this.newRecipient = { name: "", email: "" };
      }
    },
    postCompanyInvoiceRecipient() {
      this.isLoading = true;
      this.$store
        .dispatch("agency/postCompanyInvoiceRecipient", {
          companyProfileId: this.$route.params.id,
          model: this.newRecipient,
        })
        .then((response) => {
          this.companyRecipients.push({
            id: response.id,
            name: this.newRecipient.name,
            email: this.newRecipient.email,
          });
          this.newRecipient = { name: "", email: "" };
          this.$validator.reset();
          this.isLoading = false;
        })
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
    deleteCompanyInvoiceRecipientArray(index) {
      this.companyRecipients.splice(index, 1);
    },
    validateCreateEmail() {
      let valid = true;
      Promise.all([
        this.$validator.validate("email"),
        this.$validator.validate("name"),
      ]).then((isValid) => {
        isValid.forEach(function (value) {
          if (value === false) {
            valid = false;
          }
        });
        if (valid) {
          this.postCompanyInvoiceRecipient();
        }
      });
    },
    getLabelVaccinationRequired(vaccinationRequired) {
      if (vaccinationRequired == null) return "";
      return vaccinationRequired ? "Yes" : "No";
    },
    vaccinationRequiredUpdated(model) {
      this.showEditVaccinationRequired = false;
      this.company.vaccinationRequired = model.required;
      this.company.vaccinationRequiredComments = model.comments;
    },
    updateRequiresPermissionToSee(e) {
      this.isLoading = true;
      this.$store
        .dispatch("agency/updatePermissionToSeeOrders", {
          companyId: this.company.id,
          value: e.target.checked,
        })
        .then(() => this.isLoading = false)
        .catch((error) => {
          this.showAlertError(error);
          this.isLoading = false;
        });
    },
  },
};
</script>