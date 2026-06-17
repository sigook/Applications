<template>
  <div>
    <div class="container-flex justify-content-between mb-4">
      <h3 class="fw-bold">Notes</h3>
      <button @click="showModalNotes = true" class="sm-save-button">Add</button>
    </div>

    <div class="mb-5">
      <ul v-if="notesList && notesList.items.length > 0" class="container-shadow container-notes">
        <li v-for="item in notesList.items" :key="item.id">
          <p class="fw-normal">
            <span :style="{ backgroundColor: item.color }" class="note-color-icon"
              :class="{ 'border': item.color === '#fefefe' }"></span>
            {{ item.note }}
            <br><i class="fz-2" v-if="item.createdBy">By: {{ emailName(item.createdBy) }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ dateFromNow(item.createdAt) }}</i>
          </p>
        </li>
      </ul>
    </div>

    <!-- NOTES custom modal -->
    <transition name="modal">
      <div v-if="showModalNotes" class="vue-modal min-width-0">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light modal-overflow h-auto border-radius">
              <button @click="onCloseModalNotes()" type="button" class="cross-icon">close</button>
              <modal-notes :user-id="profileId" :on-get="getNotes" :on-create="createNote" :on-update="updateNote"
                :on-delete="deleteNote" @onUpdateNote="() => loadFirstNotes()">
              </modal-notes>
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end CREATE custom modal -->
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from "@/utils/toast";
import { emailName, dateFromNow } from "@/utils/filters";
import {
  getAgencyCompanyNotes,
  createAgencyCompanyNote,
  updateAgencyCompanyNote,
  deleteAgencyCompanyNote
} from "@/api/agencyNoteApi";
import type { NotesFetchPayload, NotesCreatePayload, NotesUpdatePayload, NotesDeletePayload } from '@/types/agency';
import ModalNotes from "../notes/ModalNotes.vue";

const route = useRoute();

const showModalNotes = ref(false);
const profileId = route.params.id as string;
const getNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyCompanyNotes(userId, pagination);
const createNote = ({ userId, model }: NotesCreatePayload) => createAgencyCompanyNote(userId, model);
const updateNote = ({ userId, id, model }: NotesUpdatePayload) => updateAgencyCompanyNote(userId, id, model);
const deleteNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyCompanyNote(userId, id);
const notesList = ref<any>(null);

function loadFirstNotes() {
  getAgencyCompanyNotes(profileId, { page: 1, size: 3 })
    .then(response => {
      notesList.value = response;
    })
    .catch(error => {
      showAlertError(error);
    });
}

function onCloseModalNotes() {
  showModalNotes.value = false;
  loadFirstNotes();
}

loadFirstNotes();
</script>
