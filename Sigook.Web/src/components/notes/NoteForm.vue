<template>
  <div class="form-notes" @keydown="onPressEnter">
    <b-field>
      <b-input type="textarea" v-model="newNote.note" :rows="2" placeholder="Write a note..."
        @update:model-value="onEditingNote"></b-input>
    </b-field>
    <div class="container-flex align-items-center justify-content-between">
      <color-picker v-model="newNote.color"></color-picker>
      <b-button type="is-primary" size="is-small" rounded @click="addNote">Save</b-button>
    </div>
  </div>
</template>
<script setup lang="ts">
import { reactive } from 'vue';
import type { NoteFormModel } from '@/types/agency';
import ColorPicker from "./ColorPicker.vue";

const props = defineProps<{ currentNote?: NoteFormModel }>();
const emit = defineEmits<{ (e: 'onSave', note: NoteFormModel): void }>();

const keyLocalstorage = "sigook_current_note_editing";
const defaultColor = "#fefefe";

const newNote = reactive<NoteFormModel & { color: string }>({
  color: defaultColor,
  note: "",
});

function addNote() {
  if (!newNote.note) return;
  emit('onSave', {
    id: newNote.id,
    color: newNote.color,
    note: newNote.note,
    createdAt: newNote.createdAt,
    createdBy: newNote.createdBy,
  });
  setTimeout(() => {
    newNote.note = "";
    newNote.color = defaultColor;
    localStorage.removeItem(keyLocalstorage);
  }, 200);
}

function onPressEnter(event: KeyboardEvent) {
  if (event.key === "Enter") {
    event.preventDefault();
    addNote();
  }
}

function onEditingNote(value: string) {
  localStorage.setItem(keyLocalstorage, value);
}

if (props.currentNote) {
  newNote.id = props.currentNote.id;
  newNote.color = props.currentNote.color ?? defaultColor;
  newNote.note = props.currentNote.note;
  newNote.createdAt = props.currentNote.createdAt;
  newNote.createdBy = props.currentNote.createdBy;
} else {
  const currentNoteEditing = localStorage.getItem(keyLocalstorage);
  if (currentNoteEditing) {
    newNote.note = currentNoteEditing;
  }
}
</script>

<style scoped lang="scss">
.form-notes {
  padding: 10px;
  border-radius: 4px;
  box-shadow: 0 0 6px #bfbfbf;
  margin-bottom: 20px;

  :deep(.field) {
    margin-bottom: 8px;
  }
}
</style>
