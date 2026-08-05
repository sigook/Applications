<template>
    <section>
        <div class="is-flex is-align-items-center is-justify-content-space-between">
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
        <b-modal custom-content-class="card" v-model="modal" width="500px">
            <resume-edit :data="props.worker" @closeModal="() => closeModalEdit()" />
        </b-modal>
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
