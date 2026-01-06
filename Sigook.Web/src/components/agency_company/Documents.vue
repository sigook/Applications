<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>

    <div class="button-right">
      <span class="fw-700">Documents</span>
      <button class="show-notes-btn" @click="onShowDocuments()">
        <img src="../../assets/images/right-arrow.svg" :class="{ open: showDocuments }" />
      </button>
    </div>
    <span class="line-gray"></span>

    <transition name="fade">
      <div v-if="showDocuments && data" class="mb-5">
        <div class="profile-licenses profile-experience">
          <div v-for="(document, index) in data.items" :key="document.id" class="container-license hover-actions">
            <div v-if="document.canDownload" class="button-right">
              <a :href="document.pathFile" target="_blank" download>
                <p class="fw-400">
                  {{ document.fileName | filename }}
                  <span class="download-button"></span>
                </p>
              </a>
              <div class="actions text-right">
                <b-tooltip label="Delete" type="is-dark" position="is-top">
                  <button class="btn-icon-sm btn-icon-delete bg-transparent" type="button"
                    @click="deleteAgencyCompanyDocument(document.id, index)">
                    {{ $t("Delete") }}
                  </button>
                </b-tooltip>
              </div>
            </div>
            <p v-else class="fw-400">{{ document.fileName | filename }}</p>
            <div class="fz-1">
              <p>
                <strong class="fw-400">{{ document.description }}</strong>
              </p>
            </div>
          </div>
        </div>

        <button @click="showModal = true" class="sm-save-button">Add</button>

        <pagination :total-pages="data.totalPages" :index-page="data.pageIndex" :size-page="this.size"
          @changePage="(index) => getAgencyCompanyDocument(index)">
        </pagination>
      </div>
    </transition>

    <transition name="modal">
      <div v-if="showModal" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container modal-overflow height-auto border-radius">
              <button @click="showModal = false" type="button" class="cross-icon">
                close
              </button>
              <documents-form :profile-id="profileId" @onCreateDocument="(item) => onCreateDocument(item)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>
<script>
export default {
  data() {
    return {
      showDocuments: false,
      isLoading: false,
      showModal: false,
      data: null,
      profileId: this.$route.params.id,
      size: 10,
      currentPage: 1,
    };
  },
  components: {
    documentsForm: () =>
      import("../../components/agency_company/DocumentsForm"),
    Pagination: () => import("../../components/Paginator"),
  },
  methods: {
    onShowDocuments() {
      if (!this.showDocuments) {
        this.showDocuments = true;
        this.getAgencyCompanyDocument(this.currentPage);
      } else {
        this.showDocuments = false;
      }
    },
    getAgencyCompanyDocument(index) {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyCompanyDocument", {
        profileId: this.profileId,
        pagination: { size: this.size, page: index },
      })
        .then((response) => {
          this.isLoading = false;
          this.data = response;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    onCreateDocument(item) {
      this.showModal = false;
      this.getAgencyCompanyDocument(this.currentPage);
    },
    deleteAgencyCompanyDocument(id, index) {
      let vm = this;
      this.showAlertConfirm("Are you sure", "You want to delete this document")
        .then((response) => {
          if (response) {
            vm.isLoading = true;
            vm.$store
              .dispatch("agency/deleteAgencyCompanyDocument", {
                profileId: this.profileId,
                id: id,
              })
              .then(() => {
                vm.isLoading = false;
                vm.showAlertSuccess("Deleted");
                vm.data.items.splice(index, 1);
              })
              .catch((error) => {
                vm.isLoading = false;
                vm.showAlertError(error);
              });
          }
        })
        .catch((error) => {
          vm.showAlertError(error);
        });
    },
  },
};
</script>
