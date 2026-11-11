<template>
  <span v-if="value" class="copy-link">
    <a :href="`${KIND[kind].scheme}:${value}`" class="copy-link-text" @click.stop>
      <b-icon v-if="icon" :icon="KIND[kind].icon" size="is-small" />{{ value }}<template v-if="ext"> Ext. {{ ext }}</template>
    </a>
    <b-tooltip :label="copied ? 'Copied' : KIND[kind].copyLabel" type="is-dark" append-to-body>
      <button type="button" class="copy-link-copy" :aria-label="`Copy ${value}`" @click.stop.prevent="copy">
        <b-icon :icon="copied ? 'check' : 'content-copy'" size="is-small" />
      </button>
    </b-tooltip>
  </span>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/shared/utils/toast';
import type { CopyLinkKind } from '@/shared/types/common';

const KIND: Record<CopyLinkKind, { scheme: string; icon: string; copyLabel: string }> = {
  email: { scheme: 'mailto', icon: 'email-outline', copyLabel: 'Copy email' },
  phone: { scheme: 'tel', icon: 'cellphone', copyLabel: 'Copy phone' },
};

const { value, kind, icon, ext } = defineProps<{
  kind: CopyLinkKind;
  value?: string | null;
  ext?: string | number | null;
  icon?: boolean;
}>();

const copied = ref(false);
let timer: ReturnType<typeof setTimeout> | undefined;

async function copy() {
  if (!value) {
    return;
  }
  try {
    await navigator.clipboard.writeText(value);
    copied.value = true;
    clearTimeout(timer);
    timer = setTimeout(() => (copied.value = false), 1500);
  } catch {
    showAlertError('Could not copy to clipboard');
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.copy-link {
  display: inline-flex;
  align-items: center;
  gap: 2px;
  max-width: 100%;
  min-width: 0;
}

.copy-link-text {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.copy-link-copy {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  padding: 0;
  border: 0;
  border-radius: 4px;
  background: transparent;
  color: $grey-font;
  opacity: 0.55;
  cursor: pointer;
  flex-shrink: 0;

  &:hover,
  &:focus-visible {
    opacity: 1;
    background: $gray-bg;
    color: $navy;
  }
}
</style>
