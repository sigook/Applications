<template>
  <div class="detail-page">
    <p v-if="$slots.alert" class="detail-alert">
      <slot name="alert" />
    </p>
    <section class="detail-header">
      <div class="detail-header-top">
        <router-link v-if="orgLink" :to="orgLink" class="detail-header-logo" :aria-label="orgName">
          <img v-if="logo && !logoFailed" :src="logo" alt="" @error="logoFailed = true" />
          <span v-else>{{ initials }}</span>
        </router-link>
        <b-button v-else-if="logoEditable" class="detail-header-logo is-editable" aria-label="Change logo"
          @click="emit('editLogo')">
          <img v-if="logo && !logoFailed" :src="logo" alt="" @error="logoFailed = true" />
          <span v-else>{{ initials }}</span>
        </b-button>
        <span v-else class="detail-header-logo">
          <img v-if="logo && !logoFailed" :src="logo" alt="" @error="logoFailed = true" />
          <span v-else>{{ initials }}</span>
        </span>
        <div class="detail-header-main">
          <div class="detail-header-title-row">
            <h1 class="detail-header-title">{{ title }}</h1>
            <span v-if="numberId !== undefined || meta || orgName" class="detail-header-meta">
              <template v-if="numberId !== undefined">#{{ numberId }}</template>
              <template v-if="meta"><template v-if="numberId !== undefined"> · </template>{{ meta }}</template>
              <template v-if="orgName">
                ·
                <router-link v-if="orgLink" :to="orgLink">{{ orgName }}</router-link>
                <span v-else>{{ orgName }}</span>
              </template>
            </span>
            <span v-if="statusLabel" class="detail-chip" :class="statusVariant">{{ statusLabel }}</span>
            <span v-for="chip in chips" :key="chip.label" class="detail-chip" :class="chip.variant">{{ chip.label }}</span>
          </div>

          <dl v-if="kpis.length" class="detail-kpis">
            <div v-for="kpi in kpis" :key="kpi.key" class="detail-kpi">
              <dt>{{ kpi.label }}</dt>
              <dd class="detail-kpi-value">
                <slot :name="`kpi-${kpi.key}`" :kpi="kpi">{{ kpi.value }}</slot>
                <progress v-if="kpi.progress !== undefined" class="detail-kpi-progress" :value="kpi.progress"
                  max="100">{{ kpi.progress }}%</progress>
                <span v-if="$slots[`kpi-${kpi.key}-extra`]" class="detail-kpi-extra">
                  <slot :name="`kpi-${kpi.key}-extra`" />
                </span>
                <span v-if="kpi.hint" class="detail-kpi-hint">{{ kpi.hint }}</span>
              </dd>
            </div>
          </dl>
        </div>
        <div v-if="$slots.actions" class="detail-header-actions">
          <slot name="actions" />
        </div>
      </div>
    </section>

    <div class="detail-body">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { RouteLocationRaw } from 'vue-router';
import type { DetailChip, DetailChipVariant, DetailKpi } from '@/shared/detail-page/types';

const props = defineProps<{
  title: string;
  numberId?: number;
  meta?: string;
  orgName?: string;
  orgLink?: RouteLocationRaw;
  logo?: string | null;
  logoEditable?: boolean;
  statusLabel?: string;
  statusVariant?: DetailChipVariant;
  chips?: DetailChip[];
  kpis: DetailKpi[];
}>();

const emit = defineEmits<{ (e: 'editLogo'): void }>();

const logoFailed = ref(false);

const initials = computed(() =>
  (props.orgName || props.title || '#').split(/\s+/).filter(Boolean).slice(0, 2).map((word) => word[0]).join('').toUpperCase());
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

.detail-page {
  padding: 16px 20px;

  @include touch {
    padding: 12px;
  }
}

.detail-header {
  padding: 14px 20px 12px;
  background: $white;
  border: 1px solid $gray-border;
  border-bottom: 0;
  border-radius: 12px 12px 0 0;

  @include mobile {
    padding: 14px 14px 12px;
  }
}

.detail-header-top {
  display: flex;
  align-items: flex-start;
  gap: 14px;

  @include mobile {
    flex-wrap: wrap;
    gap: 12px;
  }
}

.detail-header-logo {
  display: flex;
  flex-shrink: 0;
  align-items: center;
  justify-content: center;
  width: 44px;
  height: 44px;
  padding: 0;
  overflow: hidden;
  border: 1px solid $gray-border;
  border-radius: 10px;
  background: $gray-bg;
  font-family: inherit;
  font-size: 0.95rem;
  font-weight: 700;
  color: $blue;

  &.button.is-editable {
    cursor: pointer;

    &:hover,
    &:focus-visible {
      border-color: $blue;
    }

    :deep(> span) {
      display: flex;
      width: 100%;
      height: 100%;
      align-items: center;
      justify-content: center;
    }
  }

  img {
    width: 100%;
    height: 100%;
    object-fit: contain;
  }
}

.detail-header-main {
  display: flex;
  flex: 1;
  flex-direction: column;
  gap: 10px;
  min-width: 0;

  @include mobile {
    flex-basis: calc(100% - 58px);
  }
}

.detail-header-title-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px 10px;
}

.detail-header-title {
  margin: 0;
  font-size: 1.2rem;
  font-weight: 700;
  line-height: 1.25;
  color: $navy;
  text-transform: capitalize;

  @include mobile {
    flex-basis: 100%;
    font-size: 1.1rem;
  }
}

.detail-header-meta {
  font-size: 0.85rem;
  color: $grey-font;

  a {
    color: $blue;
  }
}

.detail-chip {
  display: inline-flex;
  align-items: center;
  padding: 2px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 600;
  background: $gray-bg;
  color: $grey-font;

  &.is-success {
    background: rgba($green, 0.18);
    color: $green-text;
  }

  &.is-danger {
    background: rgba($danger, 0.1);
    color: $danger-hover;
  }

  &.is-warning {
    background: rgba($accent, 0.16);
    color: $accent-text;
  }

  &.is-info {
    background: rgba($blue, 0.1);
    color: $blue-dark;
  }
}

.detail-header-actions {
  display: flex;
  flex-shrink: 0;
  flex-wrap: wrap;
  gap: 8px;

  :deep(.button) {
    height: 36px;
    font-size: 0.85rem;
  }

  @include mobile {
    flex-basis: 100%;

    :deep(> .button:first-child) {
      flex: 1;
    }
  }
}

.detail-kpis {
  display: flex;
  flex-wrap: wrap;
  row-gap: 8px;
  margin: 0;

  @include mobile {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 10px 12px;
  }
}

.detail-kpi {
  display: flex;
  flex-direction: column;
  gap: 1px;
  min-width: 0;
  padding: 0 18px;
  border-left: 1px solid $gray-border;

  &:first-child {
    padding-left: 0;
    border-left: 0;
  }

  dt {
    font-size: 0.65rem;
    font-weight: 700;
    letter-spacing: 0.06em;
    text-transform: uppercase;
    color: $grey-font;
  }

  dd {
    margin: 0;
  }

  @include mobile {
    padding: 0;
    border-left: 0;
  }
}

.detail-kpi-value {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px 8px;
  font-size: 0.85rem;
  font-weight: 700;
  color: $navy;
  overflow-wrap: anywhere;
}

.detail-kpi-hint {
  font-weight: 400;
  color: $grey-font;
}

.detail-kpi-extra {
  display: inline-flex;
  gap: 2px;

  :deep(.button) {
    width: 24px;
    height: 24px;
    padding: 0;
  }
}

.detail-kpi-progress {
  appearance: none;
  width: 56px;
  height: 5px;
  border: 0;
  border-radius: 3px;
  background: $gray-border;
  overflow: hidden;

  &::-webkit-progress-bar {
    background: $gray-border;
  }

  &::-webkit-progress-value {
    background: $blue;
  }

  &::-moz-progress-bar {
    background: $blue;
  }
}

.detail-body {
  :deep(> .b-tabs > .tabs) {
    margin-bottom: 16px;
    padding: 0 20px;
    background: $white;
    border: 1px solid $gray-border;
    border-top: 1px solid $gray-bg;
    border-radius: 0 0 12px 12px;

    @include mobile {
      padding: 0 8px;
    }
  }

  :deep(> .b-tabs > .tab-content) {
    padding: 0;
  }

  :deep(.detail-tab-count) {
    margin-left: 6px;
    padding: 0 7px;
    border-radius: 999px;
    background: $gray-bg;
    font-size: 0.75rem;
    color: $grey-font;
  }
}

.detail-alert {
  margin: 0 0 16px;
  padding: 12px 16px;
  border-radius: 8px;
  background: rgba($accent, 0.16);
  color: $accent-text;
}
</style>
