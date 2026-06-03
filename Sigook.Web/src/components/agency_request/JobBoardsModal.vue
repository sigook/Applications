<template>
  <div class="card job-boards-modal">
    <header class="card-header">
      <p class="card-header-title">Job Boards — Request #{{ numberId }}</p>
    </header>
    <section class="card-content">
      <p v-if="!availableBoards.length" class="has-text-grey">No job boards available.</p>
      <div v-for="board in availableBoards" :key="board.id" class="board-row">
        <div class="board-row__switch">
          <b-switch v-model="selection[board.id].active">
            {{ board.value }}
          </b-switch>
        </div>
        <div v-if="selection[board.id].active" class="board-row__inputs">
          <b-field label="Published at" label-position="on-border" class="board-row__date">
            <b-datepicker
              v-model="selection[board.id].publishedAt"
              :mobile-native="false"
              placeholder="Optional"
              icon="calendar-today"
              append-to-body />
          </b-field>
          <b-field label="External URL" label-position="on-border" class="board-row__url">
            <b-input v-model="selection[board.id].externalUrl" placeholder="Optional — https://..." />
          </b-field>
        </div>
      </div>
    </section>
    <footer class="card-footer">
      <b-button type="is-primary" expanded @click="onSave">Save</b-button>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { getSourcesForRequests } from '@/api/catalogApi';
import { setAgencyRequestSources } from '@/api/agencyRequestApi';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import type { Source } from '@/types/common';
import type { RequestJobBoard, SetRequestJobBoardItem } from '@/types/agency';

interface BoardSelection {
  active: boolean;
  publishedAt: Date | null;
  externalUrl: string;
}

const props = defineProps<{
  requestId: string;
  numberId: number;
  currentBoards: RequestJobBoard[];
}>();

const emit = defineEmits<{
  (e: 'close'): void;
  (e: 'saved'): void;
}>();

const availableBoards = ref<Source[]>([]);
const selection = reactive<Record<string, BoardSelection>>({});

function hydrateSelection() {
  for (const board of availableBoards.value) {
    const existing = props.currentBoards.find(c => c.sourceId === board.id);
    selection[board.id] = {
      active: !!existing,
      publishedAt: existing?.publishedAt ? new Date(existing.publishedAt) : null,
      externalUrl: existing?.externalUrl ?? '',
    };
  }
}

function onSave() {
  const payload: SetRequestJobBoardItem[] = availableBoards.value
    .filter(b => selection[b.id]?.active)
    .map(b => ({
      sourceId: b.id,
      publishedAt: selection[b.id].publishedAt ? selection[b.id].publishedAt!.toISOString() : null,
      externalUrl: selection[b.id].externalUrl?.trim() || null,
    }));
  setAgencyRequestSources(props.requestId, payload)
    .then(() => {
      showAlertSuccess('Job boards updated');
      emit('saved');
      emit('close');
    })
    .catch(showAlertError);
}

getSourcesForRequests().then((sources) => {
  availableBoards.value = sources;
  hydrateSelection();
});

watch(() => props.currentBoards, hydrateSelection);
</script>

<style scoped lang="scss">
.job-boards-modal {
  background: #fff;
}

.board-row {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 0;
  border-bottom: 1px solid #eee;

  &:last-child { border-bottom: 0; }

  &__switch {
    flex: 0 0 160px;
  }

  &__inputs {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 12px;
  }

  &__date {
    flex: 0 0 180px;
    margin-bottom: 0;
  }

  &__url {
    flex: 1;
    margin-bottom: 0;
  }
}
</style>
