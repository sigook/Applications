<template>
  <b-modal
    class="sd-modal"
    :model-value="modelValue"
    width="500px"
    :can-cancel="['escape', 'outside']"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div v-if="meta" class="sd-modal__panel">
      <header class="sd-modal__head">
        <div class="sd-modal__id">
          <span class="sd-modal__chip" :style="{ backgroundColor: meta.tint, color: meta.color }">
            <b-icon :icon="meta.icon" size="is-small"></b-icon>
          </span>
          <div class="sd-modal__text">
            <p class="sd-modal__title">{{ isEditing ? editTitle : meta.title }}</p>
            <p class="sd-modal__subtitle">{{ isEditing ? editSubtitle : meta.subtitle }}</p>
          </div>
        </div>
        <b-button class="sd-modal__close" aria-label="Close" @click="close">
          <b-icon icon="close" size="is-small"></b-icon>
        </b-button>
      </header>

      <div class="sd-modal__body">
        <SalesInteractionForm
          v-if="kind === 'interaction'"
          ref="interactionForm"
          :key="openToken"
          :interaction="interaction"
        />
        <SalesClientForm v-else-if="kind === 'client'" ref="clientForm" :key="openToken" />
        <SalesDealForm v-else-if="kind === 'deal'" ref="dealForm" :key="openToken" :deal="deal" />
      </div>

      <footer class="sd-modal__foot">
        <b-button class="sd-modal__cancel" outlined @click="close">Cancel</b-button>
        <b-button
          v-if="isEditing"
          class="sd-modal__delete"
          outlined
          type="is-danger"
          :loading="isDeleting"
          @click="onDelete"
        >
          <b-icon icon="trash-can-outline" size="is-small"></b-icon>
        </b-button>
        <b-button
          class="sd-modal__cta"
          expanded
          :loading="isSaving"
          @click="onSubmit"
        >
          {{ isEditing ? 'Save changes' : meta.cta }}
        </b-button>
      </footer>
    </div>
  </b-modal>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import type { SalesCreateKind } from '@/types/sales';
import type { CompanyInteraction, Deal } from '@/types/company';
import { deleteCompanyInteraction, deleteDeal } from '@/api/companyApi';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/utils/toast';
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
  interaction?: CompanyInteraction | null;
  deal?: Deal | null;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'saved'): void;
}>();

const interactionForm = ref<InstanceType<typeof SalesInteractionForm> | null>(null);
const dealForm = ref<InstanceType<typeof SalesDealForm> | null>(null);
const clientForm = ref<InstanceType<typeof SalesClientForm> | null>(null);
const isSaving = ref(false);
const isDeleting = ref(false);

const isEditing = computed(() =>
  (props.kind === 'interaction' && !!props.interaction) || (props.kind === 'deal' && !!props.deal)
);

const editTitle = computed(() => (props.kind === 'deal' ? 'Edit deal' : 'Edit interaction'));
const editSubtitle = computed(() => (props.kind === 'deal' ? 'Update this deal' : 'Update this interaction'));

const openToken = ref(0);
watch(
  () => props.modelValue,
  (open) => {
    if (open) openToken.value += 1;
  }
);

const SALES_CREATE_META: Record<SalesCreateKind, SalesCreateMeta> = {
  interaction: {
    icon: 'message-text-outline',
    color: '#7957d5',
    tint: 'rgba(121, 87, 213, 0.13)',
    title: 'Log interaction',
    subtitle: 'Record a call, email or meeting',
    cta: 'Log interaction',
  },
  client: {
    icon: 'domain',
    color: '#7957d5',
    tint: 'rgba(121, 87, 213, 0.13)',
    title: 'New client',
    subtitle: 'Add a business to your book',
    cta: 'Create client',
  },
  deal: {
    icon: 'handshake-outline',
    color: '#7957d5',
    tint: 'rgba(121, 87, 213, 0.13)',
    title: 'New deal',
    subtitle: 'Add an opportunity to the pipeline',
    cta: 'Create deal',
  },
};

const meta = computed<SalesCreateMeta | null>(() =>
  props.kind ? SALES_CREATE_META[props.kind] : null
);

const close = (): void => emit('update:modelValue', false);

async function runSubmit(action: () => Promise<boolean> | undefined): Promise<void> {
  isSaving.value = true;
  const saved = await action();
  isSaving.value = false;
  if (saved) {
    emit('saved');
    close();
  }
}

async function onSubmit(): Promise<void> {
  if (props.kind === 'interaction') {
    await runSubmit(() => interactionForm.value?.submit());
    return;
  }
  if (props.kind === 'deal') {
    await runSubmit(() => dealForm.value?.submit());
    return;
  }
  if (props.kind === 'client') {
    await runSubmit(() => clientForm.value?.submit());
    return;
  }
  close();
}

async function onDelete(): Promise<void> {
  const isDeal = props.kind === 'deal';
  const target = isDeal ? props.deal : props.interaction;
  if (!target) return;
  const confirmed = await showAlertConfirm(
    isDeal ? 'Delete deal' : 'Delete interaction',
    'This action cannot be undone.',
    'Delete'
  );
  if (!confirmed) return;
  isDeleting.value = true;
  try {
    if (isDeal) {
      await deleteDeal(target.id);
    } else {
      await deleteCompanyInteraction(target.id);
    }
    showAlertSuccess(isDeal ? 'Deal deleted' : 'Interaction deleted');
    emit('saved');
    close();
  } catch (error) {
    await showAlertError(error);
  } finally {
    isDeleting.value = false;
  }
}
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-modal {
  :deep(.animation-content) {
    overflow: visible;
  }
}

.sd-modal__panel {
  display: flex;
  flex-direction: column;
  width: 500px;
  max-width: 100%;
  max-height: 86vh;
  min-height: 0;
  background: $white;
  border-radius: 14px;
  overflow: hidden;
  box-shadow: 0 18px 50px rgba(0, 0, 0, 0.22);
}

.sd-modal__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  padding: 1.06rem 1.25rem;
  border-bottom: 1px solid #eef0f2;
  flex: none;
}

.sd-modal__id {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  min-width: 0;
}

.sd-modal__chip {
  width: 34px;
  height: 34px;
  border-radius: 9px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: none;
}

.sd-modal__text {
  min-width: 0;
}

.sd-modal__title {
  margin: 0;
  font-size: 0.94rem;
  font-weight: 700;
  color: #1f2733;
  line-height: 1.3;
}

.sd-modal__subtitle {
  margin: 0;
  font-size: 0.72rem;
  color: #9aa1ab;
  line-height: 1.4;
}

.sd-modal__close {
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
    outline: 2px solid rgba($primary, 0.5);
    outline-offset: 1px;
  }
}

.sd-modal__body {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 1.13rem 1.25rem;
}

.sd-modal__foot {
  display: flex;
  gap: 0.56rem;
  padding: 0.875rem 1.25rem;
  border-top: 1px solid #eef0f2;
  flex: none;
}

.sd-modal__cancel {
  flex: none;
  color: $grey-font;
  border-color: $gray-border;
}

.sd-modal__delete {
  flex: none;
}

.sd-modal__cta {
  color: $white;
  font-weight: 600;
  background-color: $primary;
  border-color: $primary;

  &:hover,
  &:focus {
    color: $white;
    background-color: $primary;
    border-color: $primary;
    filter: brightness(0.95);
  }
}
</style>
