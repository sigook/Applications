<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-button icon-right="close" type="is-ghost" @click="onNoteClose"></b-button>
    <span>Notes</span>
    <note-form @onSave="(note) => addNote(note)" />
    <div v-if="notes">
      <ul v-if="notes.items.length > 0" class="note-list">
        <li v-for="(item, index) in notes.items" :key="'note' + index">
          <div class="color-black">
            <span :style="{ backgroundColor: item.color }" class="note-color-icon"
              :class="{ 'border': item.color === '#fefefe' }"></span>
            {{ item.note }}
            <br><i class="fz-2" v-if="item.createdBy">By: {{ emailName(item.createdBy) }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ dateFromNow(item.createdAt) }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ dateMonth(item.createdAt) }}</i>
          </div>
          <div>
            <button v-if="onUpdate" class="btn-icon-sm btn-icon-edit" type="button"
              @click="showModalUpdateNote(item, index)"></button>
            <button v-if="onDelete" class="btn-icon-sm btn-icon-delete" type="button"
              @click="deleteNote(item.id, index)"></button>
          </div>
        </li>
      </ul>
      <div class="padding-5 color-gray-light" v-else>
        Notes
      </div>
      <pagination :total-pages="notes.totalPages" :index-page="notes.pageIndex" :size-page="size"
        @changePage="(index) => getNotes(index)">
      </pagination>
    </div>

    <transition name="modal">
      <div v-if="showModalUpdate" class="vue-modal min-width-0">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container small-container modal-light modal-overflow h-auto border-radius">
              <button @click="showModalUpdate = false" type="button" class="cross-icon">close</button>
              <h3 class="fw-bold">Edit</h3>
              <note-form :current-note="editNoteModel" :current-index="editNoteIndex"
                @onSave="(note) => updateNote(note)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { emailName, dateFromNow, dateMonth } from '@/utils/filters';
import NoteForm from "./NoteForm.vue";
import Pagination from "../../components/Paginator.vue";

const props = defineProps<{
  requestId?: any;
  userId?: any;
  onGet?: (p: any) => Promise<any>;
  onCreate?: (p: any) => Promise<any>;
  onUpdate?: (p: any) => Promise<any>;
  onDelete?: (p: any) => Promise<any>;
  canCreate?: boolean;
  currentNote?: any;
}>();

const emit = defineEmits<{
  (e: 'onUpdateNote', v: { size: number }): void;
  (e: 'close'): void;
}>();

const isLoading = ref(false);
const notes = ref<any>(null);
const currentPage = ref(1);
const size = ref(20);
const showModalUpdate = ref(false);
const editNoteModel = ref<any>(null);
const editNoteIndex = ref<number | null>(null);

function getNotes(index: number) {
  if (!props.onGet) return;
  isLoading.value = true;
  props.onGet({
    userId: props.userId,
    requestId: props.requestId,
    pagination: { page: index, size: size.value },
  })
    .then(response => {
      isLoading.value = false;
      notes.value = response;
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function addNote(newNote: any) {
  if (!props.onCreate) return;
  isLoading.value = true;
  props.onCreate({
    userId: props.userId,
    requestId: props.requestId,
    model: newNote,
  })
    .then(response => {
      isLoading.value = false;
      notes.value.items.unshift({
        id: response.id,
        note: newNote.note,
        color: newNote.color,
        createdAt: response.createdAt,
        createdBy: response.createdBy,
      });
      emit('onUpdateNote', { size: notes.value.items.length });
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function deleteNote(id: any, index: number) {
  if (!props.onDelete) return;
  isLoading.value = true;
  props.onDelete({
    userId: props.userId,
    requestId: props.requestId,
    id,
  })
    .then(() => {
      isLoading.value = false;
      notes.value.items.splice(index, 1);
      showAlertSuccess('Deleted');
      emit('onUpdateNote', { size: notes.value.items.length });
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function showModalUpdateNote(item: any, index: number) {
  showModalUpdate.value = true;
  editNoteModel.value = {
    id: item.id,
    note: item.note,
    color: item.color,
    createdAt: item.createdAt,
    createdBy: item.createdBy,
  };
  editNoteIndex.value = index;
}

function updateNote(model: any) {
  if (!props.onUpdate) return;
  isLoading.value = true;
  props.onUpdate({
    userId: props.userId,
    requestId: props.requestId,
    id: editNoteModel.value.id,
    model,
  })
    .then(() => {
      isLoading.value = false;
      notes.value.items[editNoteIndex.value as number] = {
        id: model.id,
        note: model.note,
        color: model.color,
        createdAt: model.createdAt,
        createdBy: model.createdBy,
      };
      showModalUpdate.value = false;
      editNoteIndex.value = null;
      editNoteModel.value = null;
      emit('onUpdateNote', { size: notes.value.items.length });
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function onNoteClose() {
  emit('close');
}

onMounted(() => {
  getNotes(currentPage.value);
});
</script>

<style scoped lang="scss">
.note-list {
  border-radius: 5px;
  padding: 0;
  margin-bottom: 10px;

  li {
    border: 1px solid #dbdcdb;
    border-radius: 5px;
    padding: 5px 8px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 10px;

    &:hover {
      background: #f5f5f5;
    }
  }
}
</style>
