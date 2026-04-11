<template>
    <div v-if="notesList">
        <div v-for="item in notesList.items" :key="item.id">
            <p class="fw-400 fz-14">
                <span :style="{backgroundColor: item.color}" class="note-color-icon" :class="{'border': item.color === '#fefefe'}"></span>
                {{ item.note }}
                <br><i class="fz-2" v-if="item.createdBy">By: {{item.createdBy | emailName}} | </i>
                <i class="fz-2" v-if="item.createdAt">{{item.createdAt | dateFromNow}}</i>
            </p>
        </div>

        <span v-if="canEdit" @click="showModalNotes = true" class="color-primary pointer fw-400 fz-2 underline">See more + </span>

        <!-- NOTES custom modal -->
        <transition name="modal">
            <div v-if="showModalNotes" class="vue-modal min-width-0">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-container small-container modal-light modal-overflow height-auto border-radius">
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
<script lang="ts">
import toastMixin from "@/mixins/toastMixin";
import {
  getAgencyRequestNotes,
  createAgencyRequestNote,
  updateAgencyRequestNote,
  deleteAgencyRequestNote,
} from "@/api/agencyNoteApi";

export default {
    props: ['canEdit', 'requestId'],
    data() {
        return {
            showModalNotes: false,
            profileId: this.requestId,
            getNotes: ({ userId, pagination }: any) => getAgencyRequestNotes(userId, pagination),
            createNote: ({ userId, model }: any) => createAgencyRequestNote(userId, model),
            updateNote: ({ userId, id, model }: any) => updateAgencyRequestNote(userId, id, model),
            deleteNote: ({ userId, id }: any) => deleteAgencyRequestNote(userId, id),
            notesList: null
        }
    },
    mixins: [toastMixin],
    components: {
        ModalNotes: () => import("../notes/ModalNotes.vue")
    },
    methods: {
        loadFirstNotes(){
            getAgencyRequestNotes(this.profileId, {page: 1, size: 1})
                    .then(response => {
                        this.notesList = response;
                    })
                    .catch(error => {
                        this.showAlertError(error)
                    })
        },
        onCloseModalNotes(){
            this.showModalNotes = false;
            this.loadFirstNotes();
        }
    },
    created() {
        this.loadFirstNotes();
    }
}
</script>