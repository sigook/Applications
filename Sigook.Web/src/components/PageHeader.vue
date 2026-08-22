<template>
  <div class="page-header">
    <Breadcrumbs :crumbs="crumbs" />
    <div class="page-header-main">
      <a class="page-header-back" v-if="backTo" @click="goBack(backTo)" role="button" aria-label="Go back">
        <b-icon icon="arrow-left"></b-icon>
      </a>
      <h2 class="page-header-title">
        {{ title }}
        <span class="page-header-count" v-if="count !== null">{{ count }}</span>
      </h2>
      <div class="page-header-actions" v-if="$slots.default">
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { RouteLocationRaw } from 'vue-router';
import Breadcrumbs from '@/components/Breadcrumbs.vue';
import { useGoBack } from '@/composables/useGoBack';
import type { PageBreadcrumb } from '@/types/common';

withDefaults(defineProps<{
  title: string;
  crumbs?: PageBreadcrumb[];
  backTo?: RouteLocationRaw | null;
  count?: number | null;
}>(), {
  crumbs: () => [],
  backTo: null,
  count: null,
});

const { goBack } = useGoBack();
</script>
