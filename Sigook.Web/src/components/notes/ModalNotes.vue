<template>
  <div class="white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>
    <b-button icon-right="close" type="is-ghost" @click="onNoteClose"></b-button>
    <span>Notes</span>
    <note-form @onSave="(note) => addNote(note)" />
    <div v-if="notes">
      <ul v-if="notes.items.length > 0" class="note-list">
        <li v-for="(item, index) in notes.items" :key="'note' + index">
          <div class="color-black">
            <span :style="{ backgroundColor: item.color }" class="note-color-icon"
              :class="{ 'border': item.color === '#fefefe' }"></span>
            {{ item.note }}
            <br><i class="fz-2" v-if="item.createdBy">By: {{ item.createdBy | emailName }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ item.createdAt | dateFromNow }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ item.createdAt | dateMonth }}</i>
          </div>
          <div>
            <button v-if="onUpdate" class="btn-icon-sm btn-icon-edit" type="button"
              @click="showModalUpdateNote(item, index)"></button>
            <button v-if="onDelete" class="btn-icon-sm btn-icon-delete" type="button"
              @click="deleteNote(item.id, index)"></button>
          </div>
        </li>
      </ul>
      <div class="padding-5 color-gray-light" v-else>
        Notes
      </div>
      <pagination :total-pages="notes.totalPages" :index-page="notes.pageIndex" :size-page="this.size"
        @changePage="(index) => getNotes(index)">
      </pagination>
    </div>

    <!-- NOTES custom modal -->
    <transition name="modal">
      <div v-if="showModalUpdate" class="vue-modal min-width-0">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light modal-overflow height-auto border-radius">
              <button @click="showModalUpdate = false" type="button" class="cross-icon">close</button>
              <h3 class="fw-700">Edit</h3>
              <note-form :current-note="editNoteModel" :current-index="editNoteIndex"
                @onSave="(note) => updateNote(note)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end CREATE custom modal -->
  </div>
</template>
<script>
import toast from '../../mixins/toastMixin';
export default {
  props: ['requestId', 'userId', 'onGet', 'onCreate', 'onUpdate', 'onDelete', 'canCreate', 'currentNote'],
  mixins: [toast],
  data() {
    return {
      isLoading: false,
      isDisabled: false,
      notes: null,
      currentPage: 1,
      size: 20,
      showModalUpdate: false,
      editNoteModel: null,
      editNoteIndex: null
    }
  },
  components: {
    NoteForm: () => import("./NoteForm"),
    Pagination: () => import("../../components/Paginator")
  },
  mounted() {
    this.getNotes(this.currentPage);
  },
  methods: {
    getNotes(index) {
      this.isLoading = true;
      this.$store.dispatch(this.onGet, {
        userId: this.userId,
        requestId: this.requestId,
        pagination: { page: index, size: this.size }
      })
        .then(response => {
          this.isLoading = false;
          this.notes = response
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    addNote(newNote) {
      this.isLoading = true;
      this.$store.dispatch(this.onCreate, {
        userId: this.userId,
        requestId: this.requestId,
        model: newNote
      })
        .then(response => {
          this.isLoading = false;
          this.notes.items.unshift({
            id: response.id,
            note: newNote.note,
            color: newNote.color,
            createdAt: response.createdAt,
            createdBy: response.createdBy
          })
          this.$emit("onUpdateNote", { size: this.notes.items.length });
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    deleteNote(id, index) {
      this.isLoading = true;
      this.$store.dispatch(this.onDelete, {
        userId: this.userId,
        requestId: this.requestId,
        id: id
      })
        .then(() => {
          this.isLoading = false;
          this.notes.items.splice(index, 1)
          this.showAlertSuccess('Deleted');
          this.$emit("onUpdateNote", { size: this.notes.items.length });
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    showModalUpdateNote(item, index) {
      this.showModalUpdate = true;
      this.editNoteModel = {
        id: item.id,
        note: item.note,
        color: item.color,
        createdAt: item.createdAt,
        createdBy: item.createdBy
      }
      this.editNoteIndex = index;
    },
    updateNote(model) {
      this.isLoading = true;
      this.$store.dispatch(this.onUpdate, {
        userId: this.userId,
        requestId: this.requestId,
        id: this.editNoteModel.id,
        model: model
      })
        .then(() => {
          this.isLoading = false;
          this.notes.items[this.editNoteIndex] = {
            id: model.id,
            note: model.note,
            color: model.color,
            createdAt: model.createdAt,
            createdBy: model.createdBy
          }
          this.showModalUpdate = false;
          this.editNoteIndex = null;
          this.editNoteModel = null;
          this.$emit("onUpdateNote", { size: this.notes.items.length });
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error);
        })
    },
    onNoteClose() {
      this.$emit("close")
    }
  }
}
</script>