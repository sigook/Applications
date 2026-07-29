<template>
  <div ref="triggerEl" class="notes-popover-trigger" @click="toggle">
    <b-tag icon="note-text" rounded>
      <label v-if="notesCount">{{ notesCount }}</label>
    </b-tag>
  </div>
  <Teleport to="body">
    <div v-if="show" ref="popoverEl" class="notes-tooltip" :style="popoverStyle">
      <ModalNotes :can-create="canCreate" :user-id="userId" :request-id="requestId"
        :on-get="onGet" :on-create="onCreate" :on-update="onUpdate" :on-delete="onDelete"
        @onUpdateNote="(val) => emit('update:count', val.size)" @close="close">
      </ModalNotes>
    </div>
  </Teleport>
</template>
<script setup lang="ts">
import { ref, nextTick, onBeforeUnmount } from 'vue';
import type { PaginatedList } from '@/types/common';
import type {
  NoteItem,
  CreateNoteResponse,
  RequestNotesFetchPayload,
  RequestNotesCreatePayload,
  RequestNotesUpdatePayload,
  RequestNotesDeletePayload
} from '@/types/agency';
import ModalNotes from './ModalNotes.vue';

const props = defineProps<{
  userId: string;
  requestId?: string;
  notesCount?: number;
  canCreate?: boolean;
  onGet: (payload: RequestNotesFetchPayload) => Promise<PaginatedList<NoteItem>>;
  onCreate?: (payload: RequestNotesCreatePayload) => Promise<CreateNoteResponse>;
  onUpdate?: (payload: RequestNotesUpdatePayload) => Promise<void>;
  onDelete?: (payload: RequestNotesDeletePayload) => Promise<void>;
}>();

const emit = defineEmits<{ (e: 'update:count', size: number): void }>();

const POPOVER_WIDTH = 300;

const show = ref(false);
const triggerEl = ref<HTMLElement | null>(null);
const popoverEl = ref<HTMLElement | null>(null);
const popoverStyle = ref<Record<string, string>>({});

let scrollContainer: HTMLElement | null = null;

function getScrollParent(el: HTMLElement | null): HTMLElement | null {
  let node = el?.parentElement ?? null;
  while (node) {
    const { overflowY, overflowX } = getComputedStyle(node);
    if (/(auto|scroll|overlay)/.test(overflowY + overflowX)) return node;
    node = node.parentElement;
  }
  return null;
}

function updatePosition() {
  if (!triggerEl.value) return;
  const rect = triggerEl.value.getBoundingClientRect();
  if (scrollContainer) {
    const bounds = scrollContainer.getBoundingClientRect();
    if (rect.bottom <= bounds.top || rect.top >= bounds.bottom) {
      popoverStyle.value = { display: 'none' };
      return;
    }
  }
  const maxHeight = window.innerHeight * 0.5;
  const left = Math.max(10, rect.right - POPOVER_WIDTH);
  const top = rect.bottom + maxHeight > window.innerHeight
    ? Math.max(10, rect.top - maxHeight)
    : rect.bottom + 5;
  popoverStyle.value = {
    position: 'fixed',
    top: `${top}px`,
    left: `${left}px`,
    right: 'auto',
  };
}

function onDocumentClick(event: MouseEvent) {
  const target = event.target as Node;
  if (triggerEl.value?.contains(target) || popoverEl.value?.contains(target)) return;
  close();
}

function bindListeners() {
  document.addEventListener('mousedown', onDocumentClick);
  window.addEventListener('scroll', updatePosition, true);
  window.addEventListener('resize', updatePosition);
}

function unbindListeners() {
  document.removeEventListener('mousedown', onDocumentClick);
  window.removeEventListener('scroll', updatePosition, true);
  window.removeEventListener('resize', updatePosition);
}

function open() {
  scrollContainer = getScrollParent(triggerEl.value);
  updatePosition();
  show.value = true;
  nextTick(bindListeners);
}

function close() {
  show.value = false;
  unbindListeners();
}

function toggle() {
  show.value ? close() : open();
}

onBeforeUnmount(unbindListeners);
</script>
