<template>
  <div class="page-header" :class="{ 'has-actions': !!$slots.default }">
    <teleport to="#mobile-topbar-title" :disabled="!isTouch" defer>
      <div class="page-header-heading">
        <Breadcrumbs :crumbs="crumbs" :back-to="backTo" />
        <span class="page-header-separator" v-if="crumbs.length">&rsaquo;</span>
        <h2 class="page-header-title">
          {{ title }}
          <span class="page-header-count" v-if="count !== null">{{ count }}</span>
        </h2>
      </div>
    </teleport>
    <div class="page-header-actions" v-if="$slots.default">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { RouteLocationRaw } from 'vue-router';
import Breadcrumbs from '@/components/Breadcrumbs.vue';
import { useBreakpoint } from '@/composables/useBreakpoint';
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

const { isTouch } = useBreakpoint();
</script>
