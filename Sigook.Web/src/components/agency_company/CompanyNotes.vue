<template>
  <div>
    <div class="container-flex space-between mb-4">
      <h3 class="fw-700">Notes</h3>
      <button @click="showModalNotes = true" class="sm-save-button">Add</button>
    </div>

    <div class="mb-5">
      <ul v-if="notesList && notesList.items.length > 0" class="container-shadow container-notes">
        <li v-for="item in notesList.items" :key="item.id">
          <p class="fw-400">
            <span :style="{ backgroundColor: item.color }" class="note-color-icon"
              :class="{ 'border': item.color === '#fefefe' }"></span>
            {{ item.note }}
            <br><i class="fz-2" v-if="item.createdBy">By: {{ item.createdBy | emailName }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ item.createdAt | dateFromNow }}</i>
          </p>
        </li>
      </ul>
    </div>

    <!-- NOTES custom modal -->
    <transition name="modal">
      <div v-if="showModalNotes" class="vue-modal min-width-0">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light modal-overflow height-auto border-radius">
              <button @click="onCloseModalNotes()" type="button" class="cross-icon">close</button>
              <modal-notes :user-id="profileId" :on-get="getNotes" :on-create="createNote" :on-update="updateNote"
                :on-delete="deleteNote" @onUpdateNote="() => getAgencyCompanyFirstNote()">
              </modal-notes>
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end CREATE custom modal -->
  </div>
</template>
<script>
import toastMixin from "@/mixins/toastMixin";
export default {
  data() {
    return {
      showModalNotes: false,
      profileId: this.$route.params.id,
      getNotes: 'agency/getAgencyCompanyNote',
      createNote: 'agency/createAgencyCompanyNote',
      updateNote: 'agency/updateAgencyCompanyNote',
      deleteNote: 'agency/deleteAgencyCompanyNote',
      notesList: null
    }
  },
  mixins: [toastMixin],
  components: {
    ModalNotes: () => import("../notes/ModalNotes")
  },
  methods: {
    getAgencyCompanyFirstNote() {
      this.$store.dispatch(this.getNotes, { userId: this.profileId, pagination: { page: 1, size: 3 } })
        .then(response => {
          this.notesList = response;
        })
        .catch(error => {
          this.showAlertError(error)
        })
    },
    onCloseModalNotes() {
      this.showModalNotes = false;
      this.getAgencyCompanyFirstNote();
    }
  },
  created() {
    this.getAgencyCompanyFirstNote();
  }
}
</script>