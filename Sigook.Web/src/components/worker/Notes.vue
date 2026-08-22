<template>
    <div class="notes-container">
        <div class="is-flex is-flex-wrap-wrap is-justify-content-space-between mb-4">
            <h3 class="has-text-weight-bold mt-0 mb-0">Notes</h3>
            <b-button type="is-primary" size="is-small" outlined rounded class="is-align-self-center"
                @click="showModalNotes = true">Add</b-button>
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
                    <modal-notes :user-id="workerId"
                                 :show-close="false"
                                 :on-get="getNotes"
                                 :on-create="createNote"
                                 @onUpdateNote="() => loadNotes(pageIndex)">
                    </modal-notes>
                </section>
            </div>
        </b-modal>
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
