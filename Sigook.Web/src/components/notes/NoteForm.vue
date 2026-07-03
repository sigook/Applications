<template>
  <div class="tooltip-form tooltip-form-multiline form-notes" @keydown="onPressEnter">
    <div>
      <span :style="styleNote" class="note-color-icon relative top-2 fz-2 fw-normal color-gray-light"
        :class="{ 'border': styleNote.background === '#fefefe' }">Note:
      </span>
      <label>
        <textarea class="input-block input-border bg-transparent fz-0 min-h-3em" v-on:change="onEditingNote"
          v-model="newNote.note"></textarea>
      </label>
    </div>
    <div class="container-flex d-flex align-items-center justify-content-between mt-3">
      <div>
        <color-picker @onSelectColor="(color) => changeColor(color)"></color-picker>
      </div>
      <button type="button" class="sm-btn fz-1 background-btn orange-button mt-0 btn-radius" @click="addNote()"> Save
      </button>
    </div>
  </div>
</template>
<script setup lang="ts">
import { reactive } from 'vue';
import ColorPicker from "./ColorPicker.vue";

const props = defineProps<{ currentNote?: any; currentIndex?: number }>();
const emit = defineEmits<{ (e: 'onSave', note: any): void }>();

const keyLocalstorage = "sigook_current_note_editing";

const styleNote = reactive<{ background: string }>({ background: '#fefefe' });
const newNote = reactive<any>({
  color: "#fefefe",
  note: "",
});

function addNote() {
  if (newNote.note === null || newNote.note === "") return;
  emit('onSave', {
    id: newNote.id,
    color: newNote.color,
    note: newNote.note,
    createdAt: newNote.createdAt,
    createdBy: newNote.createdBy,
  });
  setTimeout(() => {
    styleNote.background = '#fefefe';
    newNote.note = "";
    newNote.color = "#fefefe";
    localStorage.removeItem(keyLocalstorage);
  }, 200);
}

function changeColor(color: string) {
  styleNote.background = color;
  newNote.color = color;
}

function onPressEnter(event: KeyboardEvent) {
  if (event.key === "Enter") {
    event.preventDefault();
    addNote();
  }
}

function onEditingNote() {
  localStorage.setItem(keyLocalstorage, newNote.note);
}

if (props.currentNote) {
  newNote.id = props.currentNote.id;
  newNote.color = props.currentNote.color;
  newNote.note = props.currentNote.note;
  newNote.createdAt = props.currentNote.createdAt;
  newNote.createdBy = props.currentNote.createdBy;
  styleNote.background = props.currentNote.color;
} else {
  const currentNoteEditing = localStorage.getItem(keyLocalstorage);
  if (currentNoteEditing) {
    newNote.note = currentNoteEditing;
  }
}
</script>

<style scoped lang="scss">
.top-2 {
  top: 2px;
}

.form-notes {
  box-shadow: 0 0 6px #bfbfbf;
}
</style>
