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

<script lang="ts">

import { defineAsyncComponent } from 'vue';
export default {
  props: ['company'],
  data() {
    return {
      showModal: false,
      showModalUpdateEmail: false,
      localCompany: JSON.parse(JSON.stringify(this.company)),
      model: {
        phone: this.company.phone,
        phoneExt: this.company.phoneExt,
        fax: this.company.fax,
        faxExt: this.company.faxExt,
        website: this.company.website
      },
      profileId: this.$route.params.id
    }
  },
  watch: {
    company: {
      handler(newVal) {
        this.localCompany = JSON.parse(JSON.stringify(newVal));
      },
      deep: true
    }
  },
  methods: {
    closeEditModal() {
      this.showModal = false;
      this.model = {
        phone: this.company.phone,
        phoneExt: this.company.phoneExt,
        fax: this.company.fax,
        faxExt: this.company.faxExt,
        website: this.company.website
      }
    },
    closeEditEmailModal(closeModal, newEmail) {
      this.showModalUpdateEmail = false;
      if (newEmail) {
        this.localCompany.email = newEmail;
        this.$emit('update:company', this.localCompany);
      }
    },
    getFullUrl(url) {
      if (url.includes('http')) {
        return url
      }
      return `https://${url}`
    }
  },
  components: {
    DialogCompanyUpdateEmail: defineAsyncComponent(() => import("@/components/company/DialogCompanyUpdateEmail.vue")),
    ContactInformationForm: defineAsyncComponent(() => import("@/components/agency_company/ContactInformationForm.vue")),
  },
}
</script>