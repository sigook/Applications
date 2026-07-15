<template>
  <b-sidebar
    class="sd-drawer"
    :model-value="modelValue"
    right
    overlay
    fullheight
    :can-cancel="['escape', 'outside']"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div v-if="meta" class="sd-drawer__panel">
      <header class="sd-drawer__head">
        <div class="sd-drawer__id">
          <span class="sd-drawer__chip" :style="{ backgroundColor: meta.tint, color: meta.color }">
            <b-icon :icon="meta.icon" size="is-small"></b-icon>
          </span>
          <div class="sd-drawer__text">
            <p class="sd-drawer__title">{{ meta.title }}</p>
            <p class="sd-drawer__subtitle">{{ meta.subtitle }}</p>
          </div>
        </div>
        <button type="button" class="sd-drawer__close" aria-label="Close" @click="close">
          <b-icon icon="close" size="is-small"></b-icon>
        </button>
      </header>

      <div class="sd-drawer__body">
        <SalesInteractionForm v-if="kind === 'interaction'" :key="kind" :clients="clients" />
        <SalesClientForm v-else-if="kind === 'client'" :key="kind" />
        <SalesDealForm v-else-if="kind === 'deal'" :key="kind" :clients="clients" />
      </div>

      <footer class="sd-drawer__foot">
        <b-button class="sd-drawer__cancel" outlined @click="close">Cancel</b-button>
        <b-button
          class="sd-drawer__cta"
          expanded
          :style="{ backgroundColor: meta.color, borderColor: meta.color }"
          @click="close"
        >
          {{ meta.cta }}
        </b-button>
      </footer>
    </div>
  </b-sidebar>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { SalesClient, SalesCreateKind } from '@/types/salesDashboard';
import SalesInteractionForm from './SalesInteractionForm.vue';
import SalesClientForm from './SalesClientForm.vue';
import SalesDealForm from './SalesDealForm.vue';

interface SalesCreateMeta {
  icon: string;
  color: string;
  tint: string;
  title: string;
  subtitle: string;
  cta: string;
}

const props = defineProps<{
  modelValue: boolean;
  kind: SalesCreateKind | null;
  clients: readonly SalesClient[];
}>();

const emit = defineEmits<{ (e: 'update:modelValue', value: boolean): void }>();

const SALES_CREATE_META: Record<SalesCreateKind, SalesCreateMeta> = {
  interaction: {
    icon: 'message-text-outline',
    color: '#21b7ff',
    tint: 'rgba(33, 183, 255, 0.13)',
    title: 'Log interaction',
    subtitle: 'Record a call, email or meeting',
    cta: 'Log interaction',
  },
  client: {
    icon: 'domain',
    color: '#3eb800',
    tint: 'rgba(62, 184, 0, 0.13)',
    title: 'New client',
    subtitle: 'Add a business to your book',
    cta: 'Create client',
  },
  deal: {
    icon: 'handshake-outline',
    color: '#ff9932',
    tint: 'rgba(255, 153, 50, 0.13)',
    title: 'New deal',
    subtitle: 'Add an opportunity to the pipeline',
    cta: 'Create deal',
  },
};

const meta = computed<SalesCreateMeta | null>(() =>
  props.kind ? SALES_CREATE_META[props.kind] : null
);

const close = (): void => emit('update:modelValue', false);
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-drawer {
  :deep(.sidebar-content) {
    width: 366px;
    max-width: 100vw;
    background: $white;
    box-shadow: -6px 0 24px rgba(0, 0, 0, 0.12);
  }
}

.sd-drawer__panel {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;
  background: $white;
}

.sd-drawer__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  padding: 1.06rem 1.25rem;
  border-bottom: 1px solid #eef0f2;
  flex: none;
}

.sd-drawer__id {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  min-width: 0;
}

.sd-drawer__chip {
  width: 34px;
  height: 34px;
  border-radius: 9px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: none;
}

.sd-drawer__text {
  min-width: 0;
}

.sd-drawer__title {
  margin: 0;
  font-size: 0.94rem;
  font-weight: 700;
  color: #1f2733;
  line-height: 1.3;
}

.sd-drawer__subtitle {
  margin: 0;
  font-size: 0.72rem;
  color: #9aa1ab;
  line-height: 1.4;
}

.sd-drawer__close {
  width: 30px;
  height: 30px;
  border-radius: 8px;
  border: 0;
  background: #f2f3f5;
  color: #666;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  padding: 0;
  flex: none;
  transition: background-color 0.15s ease;

  &:hover {
    background: #e7e9ec;
  }

  &:focus-visible {
    outline: 2px solid rgba(33, 183, 255, 0.5);
    outline-offset: 1px;
  }
}

.sd-drawer__body {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 1.13rem 1.25rem;
}

.sd-drawer__foot {
  display: flex;
  gap: 0.56rem;
  padding: 0.875rem 1.25rem;
  border-top: 1px solid #eef0f2;
  flex: none;
}

.sd-drawer__cancel {
  flex: none;
  color: $grey-font;
  border-color: $gray-border;
}

.sd-drawer__cta {
  color: $white;
  font-weight: 600;

  &:hover,
  &:focus {
    color: $white;
    filter: brightness(0.95);
  }
}
</style>
