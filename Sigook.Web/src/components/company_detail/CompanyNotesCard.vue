<template>
  <detail-card title="Notes">
    <template #actions>
      <b-button type="is-ghost" size="is-small" class="detail-link" icon-left="plus" @click="showModal = true">
        Add note
      </b-button>
    </template>
    <span v-if="!notes.length" class="detail-empty">No notes</span>
    <div v-for="note in notes" :key="note.id" class="detail-list-item">
      <span class="detail-muted company-note-meta">
        <span class="company-note-color" :class="{ 'has-border': note.color === '#fefefe' }"
          :style="{ backgroundColor: note.color }"></span>
        {{ note.createdAt ? dateFromNow(note.createdAt) : '' }}<template v-if="note.createdBy"> · {{ emailName(note.createdBy) }}</template>
      </span>
      <span class="company-note-text">{{ note.note }}</span>
    </div>
    <b-button v-if="totalItems > notes.length" type="is-ghost" size="is-small" class="detail-link company-notes-all"
      @click="showModal = true">
      See all {{ totalItems }} notes
    </b-button>
    <span v-if="createdAt" class="detail-muted company-notes-created">Created {{ createdAt }}</span>

    <b-modal has-modal-card v-model="showModal" width="500px" :destroy-on-hide="true" @close="load">
      <div class="modal-card" style="width: 100%">
        <section class="modal-card-body">
          <modal-notes :user-id="profileId" :show-close="false" :on-get="getNotes" :on-create="createNote"
            :on-update="updateNote" :on-delete="deleteNote" @onUpdateNote="load" />
        </section>
      </div>
    </b-modal>
  </detail-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/utils/toast';
import { dateFromNow, emailName } from '@/utils/filters';
import {
  createAgencyCompanyNote,
  deleteAgencyCompanyNote,
  getAgencyCompanyNotes,
  updateAgencyCompanyNote,
} from '@/api/agencyNoteApi';
import type { NoteItem, NotesCreatePayload, NotesDeletePayload, NotesFetchPayload, NotesUpdatePayload } from '@/types/agency';
import DetailCard from '@/components/detail_page/DetailCard.vue';
import ModalNotes from '@/components/notes/ModalNotes.vue';

const props = defineProps<{ profileId: string; createdAt?: string }>();

const showModal = ref(false);
const notes = ref<NoteItem[]>([]);
const totalItems = ref(0);

const getNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyCompanyNotes(userId, pagination);
const createNote = ({ userId, model }: NotesCreatePayload) => createAgencyCompanyNote(userId, model);
const updateNote = ({ userId, id, model }: NotesUpdatePayload) => updateAgencyCompanyNote(userId, id, model);
const deleteNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyCompanyNote(userId, id);

function load() {
  getAgencyCompanyNotes(props.profileId, { page: 1, size: 3 })
    .then((response) => {
      notes.value = response.items;
      totalItems.value = response.totalItems;
    })
    .catch(showAlertError);
}

load();
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.company-note-meta {
  display: flex;
  align-items: center;
  gap: 6px;
}

.company-note-color {
  width: 10px;
  height: 10px;
  border-radius: 50%;

  &.has-border {
    border: 1px solid $gray-border;
  }
}

.company-note-text {
  white-space: pre-line;
  overflow-wrap: anywhere;
}

.button.company-notes-all {
  align-self: flex-start;
}

.company-notes-created {
  padding-top: 12px;
  border-top: 1px solid $gray-border;
}
</style>
