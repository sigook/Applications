<template>
  <profile-card title="Comments & rating">
    <template #actions>
      <button type="button" class="profile-link" @click="isModalOpen = true"><b-icon icon="plus" size="is-small" /> Add comment</button>
    </template>

    <ul v-if="props.comments.items.length" class="comment-list">
      <li v-for="item in props.comments.items" :key="item.id" class="comment-row">
        <div class="comment-meta">
          <b-rate :model-value="item.rate" disabled size="is-small" />
          <span class="comment-date">{{ dateMonth(item.createdAt) }}</span>
        </div>
        <p class="comment-text">{{ item.comment }}</p>
      </li>
    </ul>
    <p v-else class="profile-empty">No comments yet</p>

    <b-pagination v-if="props.comments.totalItems > props.pageSize" :model-value="props.pageIndex"
      :total="props.comments.totalItems" :per-page="props.pageSize" size="is-small" rounded
      @change="(page: number) => emit('changePage', page)" />

    <b-modal custom-content-class="card" v-model="isModalOpen" width="500px">
      <dialog-comment @createComment="create" />
    </b-modal>
  </profile-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/utils/toast';
import { dateMonth } from '@/utils/filters';
import { agencyCommentWorker } from '@/api/agencyWorkerApi';
import type { WorkerCommentList } from '@/types/worker';
import ProfileCard from './ProfileCard.vue';
import DialogComment from '@/components/DialogWorkerComment.vue';

const props = defineProps<{
  workerProfileId: string;
  comments: WorkerCommentList;
  pageIndex: number;
  pageSize: number;
}>();
const emit = defineEmits<{
  (e: 'commentCreated'): void;
  (e: 'changePage', page: number): void;
  (e: 'loading', value: boolean): void;
}>();

const isModalOpen = ref(false);

function create(data: { comment: string; rating: number }) {
  isModalOpen.value = false;
  if (!data.comment || !data.rating) {
    showAlertError('All fields are required');
    return;
  }
  emit('loading', true);
  agencyCommentWorker(props.workerProfileId, { comment: data.comment, rate: data.rating })
    .then(() => emit('commentCreated'))
    .catch((error) => {
      emit('loading', false);
      showAlertError(error);
    });
}

function open() {
  isModalOpen.value = true;
}

defineExpose({ open });
</script>

<style lang="scss" scoped>
@import '../../assets/scss/agency-worker-profile';

.comment-list {
  display: flex;
  flex-direction: column;
  margin: 0;
  padding: 0;
  list-style: none;
}

.comment-row {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 12px 0;
  border-top: 1px solid $gray-border;

  &:first-child {
    border-top: 0;
    padding-top: 0;
  }
}

.comment-meta {
  display: flex;
  align-items: center;
  gap: 12px;
}

.comment-date {
  font-size: 0.8rem;
  color: $grey-font;
}

.comment-text {
  margin: 0;
  font-size: 0.9rem;
  color: $navy;
  white-space: pre-line;

  &::first-letter {
    text-transform: uppercase;
  }
}

.profile-empty {
  margin: 0;
}
</style>
