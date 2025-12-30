<template>
    <div class="notes-container">
        <div class="container-flex space-between mb-4">
            <h3 class="fw-700 mt-0 mb-0">Notes</h3>
            <button @click="showModalNotes = true" class="sm-save-button align-s-center">Add</button>
        </div>

        <div class="mb-5">
            <ul v-if="notesList && notesList.items.length > 0" class="container-shadow container-notes" >
                <li v-for="item in notesList.items" :key="item.id">
                    <p class="fw-400">
                        <span :style="{backgroundColor: item.color}" class="note-color-icon" :class="{'border': item.color === '#fefefe'}"></span>
                        {{ item.note }}
                        <br><i class="fz-2" v-if="item.createdBy">By: {{item.createdBy | emailName}} | </i>
                        <i class="fz-2" v-if="item.createdAt">{{item.createdAt | dateFromNow}}</i>
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
                            <modal-notes :user-id="this.workerId"
                                         :on-get="getNotes"
                                         :on-create="createNote"
                                         @onUpdateNote="() => getAgencyWorkerProfileNote()">
                            </modal-notes>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
    </div>
</template>
<script>
    import toast from '../../mixins/toastMixin';
    export default {
        data(){
            return {
                workerId: this.$route.params.id,
                showModalNotes: false,
                notesList: null,
                pageSize: 8,
                pageIndex: 1,
                isLoading: false,
                getNotes: 'agency/getAgencyWorkerProfileNote',
                createNote: 'agency/createAgencyWorkerProfileNote',
                updateNote: null,
                deleteNote: null,
            }
        },
        mixins: [toast],
        components: {
            ModalNotes: () => import("../notes/ModalNotes")
        },
        methods: {
            getAgencyWorkerProfileNote(index){
                this.isLoading = true
                this.$store.dispatch(this.getNotes, {userId: this.workerId, pagination: {page: index, size: this.pageSize}})
                        .then(response => {
                            this.isLoading = false
                            this.notesList = response;
                        })
                        .catch(error => {
                            this.isLoading = false
                            this.showAlertError(error);
                        })
            },
            onCloseModalNotes(){
                this.showModalNotes = false;
                this.getAgencyWorkerProfileNote(this.pageIndex);
            }
        },
        created(){
            this.getAgencyWorkerProfileNote(this.pageIndex);
        }
    }
</script>