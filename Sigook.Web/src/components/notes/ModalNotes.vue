<template>
  <div class="notes-panel">
    <b-loading v-model="isLoading"></b-loading>
    <div class="is-flex is-flex-wrap-wrap is-align-items-center is-justify-content-space-between mb-3">
      <h3 class="has-text-weight-bold mt-0 mb-0">Notes</h3>
      <b-button v-if="showClose" icon-right="close" type="is-ghost" size="is-small" @click="onNoteClose"></b-button>
    </div>
    <note-form @onSave="addNote" />
    <div v-if="notes">
      <ul v-if="notes.items.length > 0" class="note-list">
        <li v-for="(item, index) in notes.items" :key="item.id">
          <div class="color-black">
            <span :style="{ backgroundColor: item.color }" class="note-color-icon"
              :class="{ 'has-border': item.color === '#fefefe' }"></span>
            {{ item.note }}
            <br><i class="fz-2" v-if="item.createdBy">By: {{ emailName(item.createdBy) }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ dateFromNow(item.createdAt) }} | </i>
            <i class="fz-2" v-if="item.createdAt">{{ dateMonth(item.createdAt) }}</i>
          </div>
          <div class="note-actions">
            <b-button v-if="onUpdate" type="is-text" size="is-small" icon-right="pencil"
              @click="showModalUpdateNote(item, index)"></b-button>
            <b-button v-if="onDelete" type="is-text" size="is-small" icon-right="delete"
              class="has-text-danger" @click="deleteNote(item.id, index)"></b-button>
          </div>
        </li>
      </ul>
      <p class="padding-5 color-gray-light" v-else>No notes yet</p>
      <b-pagination v-if="notes.totalPages > 1" v-model:current="currentPage" :total="notes.totalItems"
        :per-page="size" size="is-small" order="is-centered" simple @change="getNotes">
      </b-pagination>
    </div>

    <Teleport to="body">
      <b-modal custom-content-class="card" v-model="showModalUpdate" width="420px" :destroy-on-hide="true">
        <div class="p-4">
          <h2 class="has-text-centered fz1 mb-4">Edit note</h2>
          <note-form :current-note="editNoteModel" @onSave="updateNote" />
        </div>
      </b-modal>
    </Teleport>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { showAlertError, showAlertSuccess } from "@/utils/toast";
import { emailName, dateFromNow, dateMonth } from '@/utils/filters';
import type { PaginatedList } from '@/types/common';
import type {
  NoteItem,
  NoteFormModel,
  CreateNoteResponse,
  RequestNotesFetchPayload,
  RequestNotesCreatePayload,
  RequestNotesUpdatePayload,
  RequestNotesDeletePayload
} from '@/types/agency';
import NoteForm from "./NoteForm.vue";

const props = withDefaults(defineProps<{
  requestId?: string;
  userId?: string;
  onGet?: (payload: RequestNotesFetchPayload) => Promise<PaginatedList<NoteItem>>;
  onCreate?: (payload: RequestNotesCreatePayload) => Promise<CreateNoteResponse>;
  onUpdate?: (payload: RequestNotesUpdatePayload) => Promise<void>;
  onDelete?: (payload: RequestNotesDeletePayload) => Promise<void>;
  canCreate?: boolean;
  showClose?: boolean;
}>(), { showClose: true });

const emit = defineEmits<{
  (e: 'onUpdateNote', v: { size: number }): void;
  (e: 'close'): void;
}>();

const isLoading = ref(false);
const notes = ref<PaginatedList<NoteItem> | null>(null);
const currentPage = ref(1);
const size = ref(20);
const showModalUpdate = ref(false);
const editNoteModel = ref<NoteItem | undefined>(undefined);
const editNoteIndex = ref<number | null>(null);

function getNotes(index: number) {
  if (!props.onGet) return;
  isLoading.value = true;
  props.onGet({
    userId: props.userId as string,
    requestId: props.requestId as string,
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

function addNote(newNote: NoteFormModel) {
  if (!props.onCreate) return;
  isLoading.value = true;
  props.onCreate({
    userId: props.userId as string,
    requestId: props.requestId as string,
    model: newNote,
  })
    .then(response => {
      isLoading.value = false;
      if (!notes.value) return;
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

function deleteNote(id: string, index: number) {
  if (!props.onDelete) return;
  isLoading.value = true;
  props.onDelete({
    userId: props.userId as string,
    requestId: props.requestId as string,
    id,
  })
    .then(() => {
      isLoading.value = false;
      if (!notes.value) return;
      notes.value.items.splice(index, 1);
      showAlertSuccess('Deleted');
      emit('onUpdateNote', { size: notes.value.items.length });
    })
    .catch(error => {
      isLoading.value = false;
      showAlertError(error);
    });
}

function showModalUpdateNote(item: NoteItem, index: number) {
  showModalUpdate.value = true;
  editNoteModel.value = { ...item };
  editNoteIndex.value = index;
}

function updateNote(model: NoteFormModel) {
  if (!props.onUpdate || !editNoteModel.value) return;
  const id = editNoteModel.value.id;
  isLoading.value = true;
  props.onUpdate({
    userId: props.userId as string,
    requestId: props.requestId as string,
    id,
    model,
  })
    .then(() => {
      isLoading.value = false;
      if (notes.value && editNoteIndex.value !== null) {
        notes.value.items[editNoteIndex.value] = {
          id,
          note: model.note,
          color: model.color,
          createdAt: model.createdAt,
          createdBy: model.createdBy,
        };
        emit('onUpdateNote', { size: notes.value.items.length });
      }
      showModalUpdate.value = false;
      editNoteIndex.value = null;
      editNoteModel.value = undefined;
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
.notes-panel {
  min-width: 0;
}

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
    gap: 6px;
    margin-bottom: 10px;

    > div:first-child {
      min-width: 0;
      overflow-wrap: anywhere;
    }

    &:hover {
      background: #f5f5f5;
    }
  }
}

.note-actions {
  display: flex;
  flex-shrink: 0;
}
</style>
