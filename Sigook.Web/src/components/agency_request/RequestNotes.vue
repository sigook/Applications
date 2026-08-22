<template>
    <div>
        <div class="is-flex is-flex-wrap-wrap is-justify-content-space-between mb-4">
            <h3 class="has-text-weight-bold">Notes</h3>
            <b-button type="is-primary" size="is-small" outlined rounded @click="showModalNotes = true">Add</b-button>
        </div>

        <div class="mb-5">
            <ul v-if="notesList && notesList.items.length > 0" class="container-shadow container-notes" >
                <li v-for="item in notesList.items" :key="item.id">
                    <p class="has-text-weight-normal">
                        <span :style="{backgroundColor: item.color}" class="note-color-icon" :class="{'has-border': item.color === '#fefefe'}"></span>
                        {{ item.note }}
                        <br><i class="fz-2" v-if="item.createdBy">By: {{emailName(item.createdBy)}} | </i>
                        <i class="fz-2" v-if="item.createdAt">{{dateFromNow(item.createdAt)}}</i>
                    </p>
                </li>
            </ul>
        </div>

        <b-modal has-modal-card v-model="showModalNotes" width="500px" :destroy-on-hide="true" @close="onCloseModalNotes">
            <div class="modal-card" style="width: 100%">
                <section class="modal-card-body">
                    <modal-notes :user-id="profileId"
                                 :show-close="false"
                                 :on-get="getNotes"
                                 :on-create="createNote"
                                 :on-update="updateNote"
                                 :on-delete="deleteNote"
                                 @onUpdateNote="() => loadFirstNotes()">
                    </modal-notes>
                </section>
            </div>
        </b-modal>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
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

defineProps<{ canEdit?: boolean }>();

const route = useRoute();

const showModalNotes = ref(false);
const profileId = route.params.id as string;
const getNotes = ({ userId, pagination }: NotesFetchPayload) => getAgencyRequestNotes(userId, pagination);
const createNote = ({ userId, model }: NotesCreatePayload) => createAgencyRequestNote(userId, model);
const updateNote = ({ userId, id, model }: NotesUpdatePayload) => updateAgencyRequestNote(userId, id, model);
const deleteNote = ({ userId, id }: NotesDeletePayload) => deleteAgencyRequestNote(userId, id);
const notesList = ref<any>(null);

function loadFirstNotes() {
    getAgencyRequestNotes(profileId, { page: 1, size: 3 })
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
