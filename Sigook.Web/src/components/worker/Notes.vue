<template>
    <div class="notes-container">
        <div class="container-flex justify-content-between mb-4">
            <h3 class="fw-bold mt-0 mb-0">Notes</h3>
            <button @click="showModalNotes = true" class="sm-save-button align-self-center">Add</button>
        </div>

        <div class="mb-5">
            <ul v-if="notesList && notesList.items.length > 0" class="container-shadow container-notes" >
                <li v-for="item in notesList.items" :key="item.id">
                    <p class="fw-normal">
                        <span :style="{backgroundColor: item.color}" class="note-color-icon" :class="{'border': item.color === '#fefefe'}"></span>
                        {{ item.note }}
                        <br><i class="fz-2" v-if="item.createdBy">By: {{emailName(item.createdBy)}} | </i>
                        <i class="fz-2" v-if="item.createdAt">{{dateFromNow(item.createdAt)}}</i>
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
                            <modal-notes :user-id="workerId"
                                         :on-get="getNotes"
                                         :on-create="createNote"
                                         @onUpdateNote="() => loadNotes(pageIndex)">
                            </modal-notes>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { showAlertError } from '@/utils/toast';
import { emailName, dateFromNow } from '@/utils/filters';
import { getWorkerProfileNotes, createWorkerProfileNote } from '@/api/agencyNoteApi';
import type { NotesFetchPayload, NotesCreatePayload } from '@/types/agency';
import ModalNotes from '../notes/ModalNotes.vue';

const route = useRoute();

const workerId = route.params.id as string;
const showModalNotes = ref(false);
const notesList = ref<any>(null);
const pageSize = 8;
const pageIndex = ref(1);
const isLoading = ref(false);

const getNotes = ({ userId, pagination }: NotesFetchPayload) => getWorkerProfileNotes(userId, pagination);
const createNote = ({ userId, model }: NotesCreatePayload) => createWorkerProfileNote(userId, model);

function loadNotes(index: number) {
    isLoading.value = true;
    getWorkerProfileNotes(workerId, { page: index, size: pageSize })
        .then(response => {
            isLoading.value = false;
            notesList.value = response;
        })
        .catch(error => {
            isLoading.value = false;
            showAlertError(error);
        });
}

function onCloseModalNotes() {
    showModalNotes.value = false;
    loadNotes(pageIndex.value);
}

loadNotes(pageIndex.value);
</script>
