<template>
  <div>
    <div class="highlight-content" v-if="company">
      <div class="d-inline-flex relative mr-3">
        <div class="item">
          <span class="fw-700">{{ $t('Phone') }}</span>
          <p v-if="company.phone">
            {{ company.phone }}
            <i v-show="company.phoneExt">
              {{ $t('Ext') }} {{ company.phoneExt }}
            </i>
          </p>
          <p v-else class="op3">Phone</p>
        </div>

        <div class="item" v-if="company.fax">
          <span class="fw-700">{{ $t('Fax') }}</span>
          <p>{{ company.fax }}
            <i v-show="company.faxExt">
              {{ $t('Ext') }} {{ company.faxExt }}</i>
          </p>
        </div>

        <div class="item" v-if="company.website">
          <span class="fw-700">{{ $t('CompanyWebsite') }}</span>
          <p class="ellipsis-150 block">
            <a :href="getFullUrl(company.website)" target="_blank">{{ company.website }}</a>
          </p>
        </div>
        <button class="btn-icon-sm btn-icon-edit bg-transparent" @click="showModal = true">{{ $t("Edit") }}</button>
      </div>
      <div class="d-inline-flex relative">
        <div class="item">
          <span class="fw-700">{{ $t('Email') }}</span>
          <p class="word-break">{{ company.email }}</p>
        </div>
        <button class="btn-icon-sm btn-icon-edit bg-transparent" @click="showModalUpdateEmail = true">
          {{ $t("Edit") }}
        </button>
      </div>
    </div>

    <b-modal v-model="showModal" width="800px">
      <contact-information-form :model="company" @save="closeEditModal" />
    </b-modal>


    <b-modal v-model="showModalUpdateEmail" width="500px">
      <dialog-company-update-email :company-profile-id="company.id" @closeModal="closeEditEmailModal" />
    </b-modal>
  </div>
</template>

<script>

export default {
  props: ['company'],
  data() {
    return {
      showModal: false,
      showModalUpdateEmail: false,
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
      if (newEmail) this.company.email = newEmail;
    },
    getFullUrl(url) {
      if (url.includes('http')) {
        return url
      }
      return `https://${url}`
    }
  },
  components: {
    DialogCompanyUpdateEmail: () => import("@/components/company/DialogCompanyUpdateEmail"),
    ContactInformationForm: () => import("@/components/agency_company/ContactInformationForm"),
  },
}
</script>