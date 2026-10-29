<template>
  <div class="request-group">
    <b-collapse v-model="open" animation="slide">
      <template #trigger="props">
        <div class="request-group-header" role="button">
          <b-icon :icon="props.open ? 'chevron-down' : 'chevron-right'" size="is-small"></b-icon>
          <span class="request-group-info">
            <span class="request-group-title">
              {{ group.numberId }} &middot; {{ group.companyFullName }} &middot; {{ group.jobTitle }}
            </span>
            <span class="request-group-meta">
              {{ countLabel }}
              <template v-if="group.city">&middot; {{ group.city }}, {{ group.provinceName }}</template>
              <template v-if="group.displayShift">&middot; {{ group.displayShift }}</template>
            </span>
          </span>
          <span class="request-group-tags">
            <span v-if="group.isAsap || group.isDirectHiring" class="request-flags">
              <span v-if="group.isAsap" class="request-flag request-flag--asap">Asap</span>
              <span v-if="group.isDirectHiring" class="request-flag request-flag--dh">DH</span>
            </span>
            <b-tag v-if="group.startAt" :type="isStartingSoon(group.startAt) ? 'is-warning' : ''">
              <b-icon icon="calendar-blank-outline" size="is-small"></b-icon>
              Starts {{ dateMonth(group.startAt) }}
            </b-tag>
            <slot name="tags"></slot>
            <b-tag v-if="fill" class="request-group-fill" :type="fill.value >= fill.total ? 'is-success' : 'is-warning'">
              {{ fill.value }}/{{ fill.total }} {{ fill.label }}
            </b-tag>
          </span>
          <span v-if="$slots.menu" class="request-group-actions" @click.stop>
            <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
              <template #trigger>
                <b-button icon-right="dots-vertical" size="is-medium" type="is-text" />
              </template>
              <slot name="menu"></slot>
            </b-dropdown>
          </span>
        </div>
      </template>
      <slot></slot>
    </b-collapse>
  </div>
</template>

<script setup lang="ts">
import { dateMonth } from '@/shared/format';
import type { RequestGroupFill, RequestGroupSummary } from '@/modules/agency/shared/request-groups/types';

defineProps<{
  group: RequestGroupSummary;
  countLabel: string;
  fill?: RequestGroupFill | null;
}>();

const open = defineModel<boolean>('open', { default: false });

function daysUntil(startAt: string): number {
  const start = new Date(startAt);
  const today = new Date();
  start.setHours(0, 0, 0, 0);
  today.setHours(0, 0, 0, 0);
  return Math.round((start.getTime() - today.getTime()) / 86400000);
}

function isStartingSoon(startAt: string): boolean {
  return daysUntil(startAt) <= 2;
}
</script>

<style scoped lang="scss">
@import "@/assets/scss/variables";
@import "@/assets/scss/breakpoints";

.request-group + .request-group {
  margin-top: 12px;
}

.request-group-header {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;
  min-width: 0;
  flex: 1 1 auto;
  padding: 6px 10px;
  background: $gray-bg;
  border-bottom: 1px solid $gray-border;
  cursor: pointer;
  overflow: hidden;
}

.request-group-info {
  display: flex;
  flex-direction: column;
  flex: 1 1 auto;
  min-width: 0;
  line-height: 1.25;
}

.request-group-title {
  min-width: 0;
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.request-group-meta {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 0.8em;
  color: $grey-dark;
}

.request-group-tags,
.request-group-actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex: 0 0 auto;
}

.request-group-tags {
  margin-left: auto;

  :deep(.tag) {
    white-space: nowrap;
  }
}

.request-group-fill {
  min-width: 118px;
  justify-content: center;
}

@include touch {
  .request-group-header {
    flex-wrap: wrap;
  }

  .request-group-tags {
    margin-left: 0;
  }
}
</style>
