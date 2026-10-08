<template>
  <dl class="detail-fields" :class="{ 'is-two': columns === 2 }">
    <div v-for="field in fields" :key="field.label" class="detail-field">
      <dt>{{ field.label }}</dt>
      <dd>
        <a v-if="field.href" :href="field.href" target="_blank" rel="noopener noreferrer">{{ field.value }}</a>
        <template v-else>{{ field.value }}</template>
        <b-button v-if="field.to" tag="router-link" :to="field.to" type="is-ghost" size="is-small"
          class="detail-field-link">
          {{ field.linkLabel ?? 'Edit' }}
        </b-button>
        <b-button v-else-if="field.onAction" type="is-ghost" size="is-small" class="detail-field-link"
          @click="field.onAction">
          {{ field.linkLabel ?? 'Edit' }}
        </b-button>
      </dd>
    </div>
  </dl>
</template>

<script setup lang="ts">
import type { DetailFact } from '@/shared/detail-page/types';

defineProps<{ fields: DetailFact[]; columns?: 2 | 3 }>();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

.detail-fields {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px 24px;
  margin: 0;

  &.is-two {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  @include compact-desktop {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  @include mobile {
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 14px 16px;
  }
}

.detail-field {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;

  dt {
    font-size: 0.75rem;
    color: $grey-font;
  }

  dd {
    margin: 0;
    font-size: 0.9rem;
    color: $navy;
    overflow-wrap: anywhere;

    a {
      color: $blue;
    }
  }
}

.button.detail-field-link {
  height: auto;
  margin-left: 6px;
  padding: 0;
  font-size: 0.8rem;
  font-weight: 600;
  vertical-align: baseline;
  color: $blue;

  &:hover,
  &:focus {
    color: $blue-dark;
    text-decoration: underline;
  }
}
</style>
