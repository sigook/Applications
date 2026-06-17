<template>
    <div v-if="notesList">
        <div v-for="item in notesList.items" :key="item.id">
            <p class="fw-normal fz-14">
                <span :style="{backgroundColor: item.color}" class="note-color-icon" :class="{'border': item.color === '#fefefe'}"></span>
                {{ item.note }}
                <br><i class="fz-2" v-if="item.createdBy">By: {{emailName(item.createdBy)}} | </i>
                <i class="fz-2" v-if="item.createdAt">{{dateFromNow(item.createdAt)}}</i>
            </p>
        </div>

        <span v-if="canEdit" @click="showModalNotes = true" class="color-primary pointer fw-normal fz-2 text-decoration-underline">See more + </span>

        <!-- NOTES custom modal -->
        <transition name="modal">
            <div v-if="showModalNotes" class="vue-modal min-width-0">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-container small-container modal-light modal-overflow h-auto border-radius">
                            <button @click="onCloseModalNotes()" type="button" class="cross-icon">close</button>
                            <modal-notes :user-id="profileId"
                                         :on-get="getNotes"
                                         :on-create="createNote"
                                         :on-update="updateNote"
                                         :on-delete="deleteNote"
                                         @onUpdateNote="() => loadFirstNotes()">
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
import { showAlertError } from "@/utils/toast";
import { emailName, dateFromNow } from '@/utils/filters';
import {
  getAgencyRequestNotes,
  createAgencyRequestNote,
  updateAgencyRequestNote,
  deleteAgencyRequestNote
} from "@/api/agencyNoteApi";
import type { NotesFetchPayload, NotesCreatePayload, NotesUpdatePayload, NotesDeletePayload } from '@/types/agency';
import ModalNotes from '../notes/ModalNotes.vue';

const props = defineProps<{ canEdit?: boolean; requestId: any }>();

const showModalNotes = ref(false);
const profileId = props.requestId;
const getNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyRequestNotes(userId, pagination);
const createNote = ({ userId, model }: NotesCreatePayload) => createAgencyRequestNote(userId, model);
const updateNote = ({ userId, id, model }: NotesUpdatePayload) => updateAgencyRequestNote(userId, id, model);
const deleteNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyRequestNote(userId, id);
const notesList = ref<any>(null);

function loadFirstNotes() {
    getAgencyRequestNotes(profileId, { page: 1, size: 1 })
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
