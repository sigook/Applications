<template>
    <section>
        <div class="button-right">
            <h3 class="section-title">{{ 'Resume' }}</h3>
            <b-button type="is-info" outlined rounded icon-right="pencil"
              @click="modal = true"></b-button>
        </div>
        <div class="worker-documents">
            <div v-if="worker.resume">
                <span>{{ 'File' }}</span>
                <span>
                    <a :href="worker.resume.pathFile" target="_blank" download>
                        Resume-File
                        <span class="download-button"></span>
                    </a>
                </span>
            </div>
        </div>
        <!-- custom modal -->
        <transition name="modal">
            <div v-if="modal" class="vue-modal">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-container modal-light">
                            <span class="fz1 fw-700">{{ "Resume" }}</span>
                            <button @click="modal = false" type="button" class="cross-icon">{{ 'Close' }}</button>
                            <resume-edit :data="worker" @closeModal="() => closeModalEdit()" />
                        </div>
                    </div>
                </div>
            </div>
        </transition>
        <!-- end custom modal -->
    </section>
</template>

<script lang="ts">
export default {
    props: ['worker'],
    data() {
        return {
            modal: false
        }
    },
    methods: {
        closeModalEdit() {
            this.$emit('updateProfile', true);
            this.modal = false
        }
    },
    components: {
        resumeEdit: () => import("./WorkResumeForm.vue")
    }
}
</script>