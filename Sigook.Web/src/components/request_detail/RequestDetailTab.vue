<template>
  <detail-layout :sections="sections" aria-label="Request sections">
    <detail-card :id="anchor('job')" title="Job">
      <template v-if="$slots['job-actions']" #actions>
        <slot name="job-actions" />
      </template>
      <detail-fields :fields="fields" />
    </detail-card>

    <detail-card v-if="hasSkills" :id="anchor('skills')" title="Skills">
      <slot name="skills">
        <div class="detail-tags">
          <span v-for="skill in skills" :key="skill" class="detail-tag is-outlined">{{ skill }}</span>
        </div>
      </slot>
    </detail-card>

    <detail-card :id="anchor('description')" title="Description">
      <div v-if="description" class="detail-rich-text" v-html="description"></div>
      <span v-else class="detail-empty">No description</span>
    </detail-card>

    <div :id="anchor('requirements')" class="request-detail-pair">
      <detail-card v-if="responsibilities" title="Responsibilities">
        <div class="detail-rich-text" v-html="responsibilities"></div>
      </detail-card>
      <detail-card title="Requirements">
        <template v-if="$slots['requirements-actions']" #actions>
          <slot name="requirements-actions" />
        </template>
        <div v-if="requirements" class="detail-rich-text" v-html="requirements"></div>
        <span v-else class="detail-empty">No requirements</span>
      </detail-card>
    </div>

    <detail-card v-if="internalRequirements" :id="anchor('internal')" title="Internal requirements" internal>
      <template #badge>
        <span class="detail-tag">Agency only</span>
      </template>
      <div class="detail-rich-text" v-html="internalRequirements"></div>
    </detail-card>

    <detail-card v-if="incentive" :id="anchor('incentive')" title="Incentive">
      <p class="detail-rich-text">
        <strong>{{ currency(incentive) }}</strong>
        <span v-if="incentiveDescription"> · {{ incentiveDescription }}</span>
      </p>
    </detail-card>

    <detail-card v-if="complianceItems?.length" :id="anchor('compliance')" title="Compliance checklist">
      <div class="request-compliance">
        <div v-for="item in complianceItems" :key="item.id ?? item.name" class="request-compliance-item">
          {{ item.name }}
          <span class="request-compliance-meta">{{ item.isMandatory ? 'required' : 'optional' }}</span>
        </div>
      </div>
    </detail-card>

    <template v-if="$slots.rail" #rail>
      <slot name="rail" />
    </template>
  </detail-layout>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { currency } from '@/utils/filters';
import type { RequestComplianceItem } from '@/types/agency';
import type { DetailSection } from '@/types/detailPage';
import type { RequestFact } from '@/types/requestDetail';
import DetailLayout from '@/components/detail_page/DetailLayout.vue';
import DetailCard from '@/components/detail_page/DetailCard.vue';
import DetailFields from '@/components/detail_page/DetailFields.vue';

const props = defineProps<{
  fields: RequestFact[];
  description?: string;
  responsibilities?: string;
  requirements?: string;
  internalRequirements?: string;
  incentive?: number | null;
  incentiveDescription?: string;
  skills?: string[];
  showSkills?: boolean;
  complianceItems?: RequestComplianceItem[];
}>();

const hasSkills = computed(() => props.showSkills || !!props.skills?.length);

function anchor(id: string) {
  return `request-section-${id}`;
}

const sections = computed<DetailSection[]>(() => [
  { id: anchor('job'), label: 'Job' },
  ...(hasSkills.value ? [{ id: anchor('skills'), label: 'Skills' }] : []),
  { id: anchor('description'), label: 'Description' },
  { id: anchor('requirements'), label: props.responsibilities ? 'Responsibilities & requirements' : 'Requirements' },
  ...(props.internalRequirements ? [{ id: anchor('internal'), label: 'Internal', hint: 'agency only' }] : []),
  ...(props.incentive ? [{ id: anchor('incentive'), label: 'Incentive' }] : []),
  ...(props.complianceItems?.length ? [{ id: anchor('compliance'), label: 'Compliance' }] : []),
]);
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';
@import '../../assets/scss/breakpoints';

.request-detail-pair {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 20px;
  scroll-margin-top: 68px;

  @include touch {
    gap: 12px;
  }
}

.request-compliance {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 10px;
}

.request-compliance-item {
  padding: 10px 12px;
  border: 1px solid $gray-border;
  border-radius: 8px;
  font-size: 0.9rem;
  color: $navy;
}

.request-compliance-meta {
  margin-left: 4px;
  font-size: 0.75rem;
  color: $grey-font;
}
</style>
