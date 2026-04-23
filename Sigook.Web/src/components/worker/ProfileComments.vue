<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <section class="worker-comments">
      <comments v-if="commentsData" :user-id="props.worker.workerId" :data="commentsData" :size-comments="commentSize"
        only-view="true" @newComment="() => updateComments()" @changePage="page => changePageComments(page)"></comments>
    </section>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue';
import { getCommentsWorker } from '@/api/workerApi';
import Comments from '../../components/Comments.vue';

const props = defineProps<{ worker?: any }>();

const isLoading = ref(true);
const commentSize = 10;
const commentPageIndex = ref(1);
const commentsData = ref<any>([]);

function updateComments() {
  isLoading.value = true;
  getCommentsWorker({ workerId: props.worker.workerId, size: commentSize, pageIndex: commentPageIndex.value })
    .then((data) => {
      commentsData.value = data;
      isLoading.value = false;
    });
}

function changePageComments(page: number) {
  commentPageIndex.value = page;
  updateComments();
}

updateComments();
</script>

<style lang="scss">
@import '../../assets/scss/detail-worker';
</style>
