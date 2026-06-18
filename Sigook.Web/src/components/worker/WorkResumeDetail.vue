<template>
    <section>
        <div class="d-flex align-items-center justify-content-between">
            <h3 class="section-title">{{ 'Resume' }}</h3>
            <b-button type="is-info" outlined rounded icon-right="pencil"
              @click="modal = true"></b-button>
        </div>
        <div class="worker-documents">
            <div v-if="props.worker.resume">
                <span>{{ 'File' }}</span>
                <span>
                    <a :href="props.worker.resume.pathFile" target="_blank" download>
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
                            <span class="fz1 fw-bold">{{ "Resume" }}</span>
                            <button @click="modal = false" type="button" class="cross-icon">{{ 'Close' }}</button>
                            <resume-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
                        </div>
                    </div>
                </div>
            </div>
        </transition>
        <!-- end custom modal -->
    </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import ResumeEdit from './WorkResumeForm.vue';

const props = defineProps<{ worker?: any }>();
const emit = defineEmits<{ (e: 'updateProfile', value: boolean): void }>();

const modal = ref(false);

function closeModalEdit() {
    emit('updateProfile', true);
    modal.value = false;
}
</script>
