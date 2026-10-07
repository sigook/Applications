<template>
  <section class="detail-card" :class="{ 'is-internal': internal }">
    <header v-if="title || $slots.actions || $slots.badge" class="detail-card-header">
      <h2 v-if="title" class="detail-card-title">{{ title }}</h2>
      <slot name="badge" />
      <div v-if="$slots.actions" class="detail-card-actions">
        <slot name="actions" />
      </div>
    </header>
    <slot />
  </section>
</template>

<script setup lang="ts">
defineProps<{ title?: string; internal?: boolean }>();
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';
@import '../../assets/scss/breakpoints';

.detail-card {
  display: flex;
  flex-direction: column;
  gap: 14px;
  min-width: 0;
  margin: 0;
  padding: 20px 24px;
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 12px;
  scroll-margin-top: 68px;

  &.is-internal {
    background: $gray-bg;
    border-style: dashed;
  }

  @include mobile {
    padding: 16px;
  }
}

.detail-card-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 10px;
}

.detail-card-title {
  margin: 0;
  font-size: 1.05rem;
  font-weight: 700;
  color: $navy;
}

.detail-card-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-left: auto;
}

.detail-card :deep(.detail-rich-text) {
  margin: 0;
  font-size: 0.9rem;
  line-height: 1.6;
  color: $navy;
  overflow-wrap: anywhere;
  white-space: pre-line;

  ul,
  ol {
    margin: 0;
    padding-left: 18px;
    list-style: disc;
  }

  p {
    margin: 0 0 6px;
  }
}

.detail-card :deep(.detail-empty) {
  font-size: 0.9rem;
  color: $grey-light;
}

.detail-card :deep(.detail-muted) {
  font-size: 0.8rem;
  color: $grey-font;
}

.detail-card :deep(.detail-tags) {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.detail-card :deep(.detail-tag) {
  padding: 3px 10px;
  border-radius: 6px;
  background: $white;
  font-size: 0.75rem;
  font-weight: 600;
  color: $grey-font;

  &.is-outlined {
    border: 1px solid $border-input;
    border-radius: 999px;
    font-weight: 400;
    color: $navy;
  }

  &.is-info {
    background: rgba($blue, 0.1);
    color: $blue-dark;
  }
}

.detail-card :deep(.button.detail-link) {
  height: auto;
  padding: 0;
  font-size: 0.8rem;
  font-weight: 600;
  color: $blue;

  &:hover,
  &:focus {
    color: $blue-dark;
    text-decoration: underline;
  }
}

.detail-card :deep(.button.detail-icon-button) {
  width: 32px;
  height: 32px;
  padding: 0;
  color: $grey-font;

  &:hover,
  &:focus {
    background: $gray-bg;
    color: $navy;
  }

  &.is-destructive:hover,
  &.is-destructive:focus {
    color: $danger-hover;
  }
}

.detail-card :deep(.detail-list-item) {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.9rem;
  color: $navy;

  & + .detail-list-item {
    padding-top: 12px;
    border-top: 1px solid $gray-bg;
  }
}
</style>
