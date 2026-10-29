<template>
  <nav class="page-header-breadcrumb" v-if="crumbs.length || backTo">
    <router-link class="page-header-back" v-if="backTo" :to="backTo" aria-label="Go back">
      <b-icon icon="arrow-left" size="is-small"></b-icon>
    </router-link>
    <template v-for="(crumb, index) in crumbs" :key="index">
      <router-link v-if="crumb.to" :to="crumb.to">{{ crumb.label }}</router-link>
      <span v-else>{{ crumb.label }}</span>
      <span class="page-header-separator" v-if="index < crumbs.length - 1">&rsaquo;</span>
    </template>
  </nav>
</template>

<script setup lang="ts">
import type { RouteLocationRaw } from 'vue-router';
import type { PageBreadcrumb } from '@/shared/types/common';
import { registerMobileBack } from '@/shared/composables/useMobileBack';

const props = withDefaults(defineProps<{
  crumbs?: PageBreadcrumb[];
  backTo?: RouteLocationRaw | null;
}>(), {
  crumbs: () => [],
  backTo: null,
});

registerMobileBack(() => props.backTo);
</script>
