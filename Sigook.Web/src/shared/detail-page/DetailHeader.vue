<template>
  <div class="detail-page" :class="{ 'is-header-hidden': headerHidden }">
    <p v-if="$slots.alert" class="detail-alert">
      <slot name="alert" />
    </p>
    <section v-if="!headerHidden" class="detail-header">
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
          <b-icon v-else-if="logoIcon" :icon="logoIcon"></b-icon>
          <span v-else>{{ initials }}</span>
        </span>
        <div class="detail-header-main">
          <div class="detail-header-title-row">
            <h1 class="detail-header-title" :title="title">{{ title }}</h1>
            <span v-if="statusLabel || chips?.length" class="detail-header-chips">
              <span v-if="statusLabel" class="detail-chip" :class="statusVariant">{{ statusLabel }}</span>
              <template v-for="chip in chips" :key="chip.label">
                <b-tooltip v-if="chip.icon" :label="chip.label" position="is-top" type="is-dark">
                  <span class="detail-chip is-icon" :class="chip.variant" tabindex="0" :aria-label="chip.label">
                    <b-icon :icon="chip.icon" size="is-small"></b-icon>
                  </span>
                </b-tooltip>
                <span v-else class="detail-chip" :class="chip.variant">{{ chip.label }}</span>
              </template>
            </span>
          </div>
          <div v-if="numberId !== undefined || meta || orgName || metaChips?.length" class="detail-header-meta">
            <span v-if="numberId !== undefined || meta">
              <template v-if="numberId !== undefined">#{{ numberId }}</template>
              <template v-if="meta"><template v-if="numberId !== undefined"> · </template>{{ meta }}</template>
            </span>
            <template v-if="orgName">
              <router-link v-if="orgLink" :to="orgLink" class="detail-header-org">
                <b-icon icon="domain" size="is-small"></b-icon>{{ orgName }}
              </router-link>
              <span v-else class="detail-header-org">{{ orgName }}</span>
            </template>
            <span v-for="chip in metaChips" :key="chip.label" class="detail-chip is-meta" :class="chip.variant">
              <b-icon v-if="chip.icon" :icon="chip.icon" size="is-small"></b-icon>{{ chip.label }}
            </span>
          </div>

          <slot name="subtitle" />
        </div>
        <div v-if="$slots.actions" class="detail-header-actions">
          <slot name="actions" />
        </div>
      </div>
      <detail-kpis v-if="kpis.length" :kpis="kpis">
        <template v-for="(_, name) in $slots" #[name]="scope">
          <slot :name="name" v-bind="scope ?? {}" />
        </template>
      </detail-kpis>
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
import DetailKpis from '@/shared/detail-page/DetailKpis.vue';

const { orgName, title } = defineProps<{
  title: string;
  numberId?: number;
  meta?: string;
  orgName?: string;
  orgLink?: RouteLocationRaw;
  logo?: string | null;
  logoIcon?: string;
  logoEditable?: boolean;
  headerHidden?: boolean;
  statusLabel?: string;
  statusVariant?: DetailChipVariant;
  chips?: DetailChip[];
  metaChips?: DetailChip[];
  kpis: DetailKpi[];
}>();

const emit = defineEmits<(e: 'editLogo') => void>();

const logoFailed = ref(false);

const initials = computed(() =>
  (orgName || title || '#').split(/\s+/).filter(Boolean).slice(0, 2).map((word) => word[0]).join('').toUpperCase());
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
  background: $white;
  border: 1px solid $gray-border;
  border-bottom: 0;
  border-radius: 12px 12px 0 0;
}

.detail-header-top {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px 20px;

  @include mobile {
    flex-wrap: wrap;
    align-items: flex-start;
    gap: 12px;
    padding: 14px;
  }
}

.detail-header-logo {
  display: flex;
  flex-shrink: 0;
  align-items: center;
  justify-content: center;
  width: 56px;
  height: 56px;
  padding: 0;
  overflow: hidden;
  border: 1px solid $gray-border;
  border-radius: 10px;
  background: $gray-bg;
  font-family: inherit;
  font-size: 18px;
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
  gap: 3px;
  min-width: 0;

  @include mobile {
    flex-basis: calc(100% - 68px);
  }
}

.detail-header-title-row {
  display: flex;
  align-items: center;
  gap: 6px 10px;
  min-width: 0;

  @include mobile {
    flex-wrap: wrap;
  }
}

.detail-header-title {
  flex: 0 1 auto;
  min-width: 0;
  margin: 0;
  overflow: hidden;
  font-size: 24px;
  font-weight: 700;
  line-height: 1.25;
  color: $navy;
  text-overflow: ellipsis;
  text-transform: capitalize;
  white-space: nowrap;

  @include compact-desktop {
    font-size: 22px;
  }

  @include mobile {
    flex-basis: 100%;
    font-size: 21px;
    white-space: normal;
  }
}

.detail-header-chips {
  display: inline-flex;
  flex-shrink: 0;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px;
}

.detail-header-meta {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 6px 10px;
  font-size: 15px;
  color: $grey-font;
}

.detail-header-org {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-weight: 600;
  color: $blue;
  white-space: nowrap;
}

.detail-chip {
  display: inline-flex;
  align-items: center;
  padding: 3px 12px;
  border-radius: 999px;
  font-size: 13.5px;
  font-weight: 600;
  background: $gray-bg;
  color: $grey-font;
  white-space: nowrap;

  &.is-icon {
    justify-content: center;
    width: 32px;
    height: 28px;
    padding: 0;
    cursor: default;

    &:focus-visible {
      outline: 2px solid $blue;
      outline-offset: 2px;
    }
  }

  &.is-meta {
    gap: 4px;
    padding: 2px 10px;
    font-size: 13px;
  }

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
    height: 40px;
    font-size: 15px;
  }

  @include mobile {
    flex-basis: 100%;

    :deep(> .button:first-child) {
      flex: 1;
    }
  }
}

.detail-body {
  :deep(> .b-tabs > .tabs) {
    margin-bottom: 16px;
    padding: 0 20px;
    background: $white;
    border: 1px solid $gray-border;
    border-top: 1px solid $gray-border;
    border-radius: 0 0 12px 12px;
    font-size: 15px;

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
    font-size: 13px;
    color: $grey-font;
  }
}

.is-header-hidden > .detail-body {
  :deep(> .b-tabs > .tabs) {
    border-radius: 12px;
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
