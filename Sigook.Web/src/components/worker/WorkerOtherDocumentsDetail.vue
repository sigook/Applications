<template>
  <section>
    <b-loading v-model="isLoading"></b-loading>
    <div class="button-right">
      <div>
        <h3 class="fw-700 fz-0">{{ justWhmis ? 'WHMIS and Health and Safety Training' : 'Other documents' }} </h3>
        <i class="fz-2" v-if="justWhmis">Complete the training following both links below and uplaod your
          certificates</i>
      </div>
      <button type="button" class="outline-btn md-btn orange-button btn-radius" @click="modalDocuments = true">
        Add Document +
      </button>
    </div>
    <div v-if="justWhmis">
      <p v-if="!worker.location.isUSA">
        <a href="https://aixsafety.com/wp-content/uploads/articulate_uploads/WMS3May2024AixSafety23/story.html"
          target="_blank">WHIMS</a>
      </p>
      <p v-if="!worker.location.isUSA">
        <a href="https://www.labour.gov.on.ca/english/hs/elearn/worker/foursteps.php" target="_blank">
          HS BOOKLET
        </a>
      </p>
    </div>
    <div class="profile-licenses profile-experience">
      <div class="container-license hover-actions" v-for="(item, index) in worker.otherDocuments"
        v-bind:key="'docs' + index">
        <div class="button-right">
          <a :href="item.pathFile" target="_blank" download>
            <h4 class="fw-400">
              {{ item.fileName | filename }}
              <span class="download-button"></span>
            </h4>
          </a>
          <div class="actions text-right">
            <b-tooltip label="Delete" type="is-dark" position="is-top">
              <button class="btn-icon-sm btn-icon-delete" type="button" @click="confirmDelete(item)">
                {{ $t("Delete") }}
              </button>
            </b-tooltip>
          </div>
        </div>
        <div class="fz-1">
          <p>
            <strong class="fw-400">{{ item.description }}</strong>
          </p>
        </div>
      </div>
    </div>

    <!-- custom CREATE modal -->
    <transition name="modal">
      <div v-if="modalDocuments" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light overflow-initial">
              <span class="fz1 fw-700">New Document</span>
              <button @click="modalDocuments = false" type="button" class="cross-icon">
                {{ $t("Close") }}
              </button>
              <documents-form :data="worker" @closeAndUpdate="() => closeAndUpdate()" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal -->
  </section>
</template>
<script>
import toastMixin from "../../mixins/toastMixin";
export default {
  props: ["worker", 'justWhmis'],
  data() {
    return {
      isLoading: false,
      modalDocuments: false,
      documents: [],
    };
  },
  mixins: [toastMixin],
  methods: {
    closeAndUpdate() {
      this.modalDocuments = false;
      this.$emit('updateProfile', true);
    },
    confirmDelete(document) {
      const vm = this;
      this.showAlertConfirm("Are you sure", "You want to delete this document").then((response) => {
        if (response) {
          vm.isLoading = true;
          vm.$store.dispatch("worker/deleteWorkerOtherDocuments", { profileId: this.worker.id, otherDocumentId: document.id })
            .then(() => {
              vm.isLoading = false;
              this.$emit('updateProfile', true);
            })
            .catch((error) => {
              vm.isLoading = false;
              this.showAlertError(error);
            });
        }
      })
    }
  },
  components: {
    documentsForm: () => import("./WorkerOtherDocumentsForm"),
  }
};
</script>
