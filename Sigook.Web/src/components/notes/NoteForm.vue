<template>
  <div class="tooltip-form tooltip-form-multiline form-notes" @keydown="onPressEnter">
    <div>
      <span :style="styleNote" class="note-color-icon relative top-2 fz-2 fw-400 color-gray-light"
        :class="{ 'border': styleNote.background === '#fefefe' }">Note:
      </span>
      <label>
        <textarea class="input-block input-border bg-transparent fz-0 min-h-3em" v-on:change="onEditingNote"
          v-model="newNote.note"></textarea>
      </label>
    </div>
    <div class="container-flex button-right align-center mt-3">
      <div>
        <color-picker @onSelectColor="(color) => changeColor(color)"></color-picker>
      </div>
      <button type="button" class="sm-btn fz-1 background-btn orange-button mt-0 btn-radius" @click="addNote()"> Save
      </button>
    </div>
  </div>
</template>
<script>
export default {
  props: ['currentNote', 'currentIndex'],
  data() {
    return {
      keyLocalstorage: "sigook_current_note_editing",
      styleNote: {
        background: '#fefefe',
      },
      newNote: {
        color: "#fefefe",
        note: ""
      },
    }
  },
  components: {
    ColorPicker: () => import("./ColorPicker"),
  },
  methods: {
    addNote() {
      if (this.newNote.note === null || this.newNote.note === "") return;
      this.$emit('onSave', {
        id: this.newNote.id,
        color: this.newNote.color,
        note: this.newNote.note,
        createdAt: this.newNote.createdAt,
        createdBy: this.newNote.createdBy
      });
      setTimeout(() => {
        this.styleNote = { background: '#fefefe' }
        this.newNote.note = "";
        this.newNote.color = "#fefefe";
        localStorage.removeItem(this.keyLocalstorage);
      }, 200)
    },
    changeColor(color) {
      this.styleNote.background = color;
      this.newNote.color = color;
    },
    onPressEnter(event) {
      if (event.key === "Enter") {
        event.preventDefault();
        this.addNote()
      }
    },
    onEditingNote() {
      localStorage.setItem(this.keyLocalstorage, this.newNote.note);
    }
  },
  created() {
    if (this.currentNote) {
      this.newNote = {
        id: this.currentNote.id,
        color: this.currentNote.color,
        note: this.currentNote.note,
        createdAt: this.currentNote.createdAt,
        createdBy: this.currentNote.createdBy
      }
      this.styleNote.background = this.currentNote.color
    } else {
      let currentNoteEditing = localStorage.getItem(this.keyLocalstorage);
      if (currentNoteEditing) {
        this.newNote.note = currentNoteEditing;
      }
    }
  }
}
</script>