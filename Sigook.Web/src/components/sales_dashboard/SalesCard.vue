<template>
  <section class="sd-card">
    <header class="sd-card__head">
      <div class="sd-card__id">
        <span v-if="icon" class="sd-card__chip" :class="toneClass">
          <b-icon :icon="icon" size="is-small"></b-icon>
        </span>
        <div class="sd-card__text">
          <router-link v-if="titleTo" :to="titleTo" class="sd-card__title sd-card__title--link">{{ title }}</router-link>
          <p v-else class="sd-card__title">{{ title }}</p>
          <p v-if="$slots.subtitle || subtitle" class="sd-card__subtitle">
            <slot name="subtitle">{{ subtitle }}</slot>
          </p>
        </div>
      </div>
      <div v-if="$slots.actions || actionIcon" class="sd-card__actions">
        <slot name="actions">
          <button
            type="button"
            class="sd-card__action"
            :class="toneClass"
            :title="actionLabel"
            :aria-label="actionLabel"
            @click="emit('action')"
          >
            <b-icon :icon="actionIcon" size="is-small"></b-icon>
          </button>
        </slot>
      </div>
    </header>

    <div class="sd-card__body">
      <slot></slot>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';

type SalesCardTone = 'primary' | 'green' | 'accent';

const props = withDefaults(
  defineProps<{
    title: string;
    titleTo?: string;
    subtitle?: string;
    icon?: string;
    tone?: SalesCardTone;
    actionIcon?: string;
    actionLabel?: string;
  }>(),
  {
    titleTo: '',
    subtitle: '',
    icon: '',
    tone: 'primary',
    actionIcon: '',
    actionLabel: '',
  }
);

const emit = defineEmits<{ (e: 'action'): void }>();

const toneClass = computed(() => `is-${props.tone}`);
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-card {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 10px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  overflow: hidden;
}

.sd-card__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  padding: 0.8rem 0.95rem;
  border-bottom: 1px solid $gray-bg;
  flex: none;
}

.sd-card__id {
  display: flex;
  align-items: center;
  gap: 0.55rem;
  min-width: 0;
}

.sd-card__chip {
  width: 1.875rem;
  height: 1.875rem;
  border-radius: 8px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: none;

  &.is-primary {
    background: rgba(33, 183, 255, 0.12);
    color: $primary;
  }

  &.is-green {
    background: rgba(62, 184, 0, 0.13);
    color: $green-vivid;
  }

  &.is-accent {
    background: rgba(255, 153, 50, 0.14);
    color: $accent;
  }
}

.sd-card__text {
  min-width: 0;
}

.sd-card__title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #333;
  margin: 0;
  line-height: 1.3;
}

.sd-card__title--link {
  display: inline-block;
  cursor: pointer;
  text-decoration: none;
  transition: color 0.15s ease;

  &:hover {
    color: $primary;
    text-decoration: underline;
  }

  &:focus-visible {
    outline: 2px solid rgba(33, 183, 255, 0.5);
    outline-offset: 2px;
    border-radius: 3px;
  }
}

.sd-card__subtitle {
  font-size: 0.69rem;
  color: $grey-light;
  margin: 0;
  line-height: 1.4;
}

.sd-card__actions {
  flex: none;
  display: flex;
  align-items: center;
}

.sd-card__action {
  width: 1.75rem;
  height: 1.75rem;
  border-radius: 50%;
  border: 1px solid $gray-border;
  background: $white;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  padding: 0;
  transition: background-color 0.15s ease, color 0.15s ease, border-color 0.15s ease;

  &.is-primary {
    color: $primary;

    &:hover {
      background: $primary;
      border-color: $primary;
      color: $white;
    }
  }

  &.is-green {
    color: $green-vivid;

    &:hover {
      background: $green-vivid;
      border-color: $green-vivid;
      color: $white;
    }
  }

  &.is-accent {
    color: $accent;

    &:hover {
      background: $accent;
      border-color: $accent;
      color: $white;
    }
  }

  &:focus-visible {
    outline: 2px solid rgba(33, 183, 255, 0.5);
    outline-offset: 1px;
  }
}

.sd-card__body {
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
}
</style>
