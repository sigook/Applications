<template>
    <div class="pagination" v-if="totalPages > 1">
        <ul>
            <button class="btn-prev"
                    @click="changePage(currentPage - 1)"
                    :disabled="currentPage === 1" ></button>

            <template v-for="(page, index) in totalPages">
                <li v-if="Math.abs(page - currentPage) < 3 || page === totalPages - 1 || page === 0"
                    :key="'pagination' + index"
                    :class="[page === currentPage ? 'active' : '']">
                    <span @click="changePage(page)">{{page}}</span>
                </li>
            </template>

            <button class="btn-next"
                    @click="changePage(currentPage + 1)"
                    :disabled="totalPages === currentPage"></button>
        </ul>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  sizePage?: number;
  indexPage?: number;
  totalPages?: number;
}>();

const emit = defineEmits<{ (e: 'changePage', page: number): void }>();

const currentPage = computed(() => props.indexPage);

function changePage(newPage: number) {
  emit('changePage', newPage);
  const containers = document.getElementsByClassName("scroll-top-on-pagination");
  for (let i = 0; i < containers.length; i++) {
    (containers[i] as HTMLElement).scrollTop = 0;
  }
  const bodyTop = document.getElementsByClassName("body-top-on-pagination");
  if (bodyTop.length > 0) {
    window.scrollTo(0, 0);
  }
}
</script>
