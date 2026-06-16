<template>
  <div class="detail-content">
    <div class="detail-header">
      <h2 class="detail-title">
        {{ job.title }}
        <span class="detail-id">#{{ job.numberId }}</span>
      </h2>
      <div class="detail-meta">
        <span class="meta-item location">{{ job.location }}</span>
        <span class="meta-item salary" v-if="job.salary !== '$0.00'">{{ job.salary }} / hr</span>
        <span class="meta-item type">{{ job.type }}</span>
      </div>
    </div>

    <div class="apply-container">
      <button class="btn-apply-large" @click="$emit('apply', job)">APPLY NOW</button>
    </div>

    <hr class="divider" />

    <div class="detail-body">
      <div class="content-block description" v-if="job.description">
        <div v-html="job.description"></div>
      </div>

      <div class="content-block" v-if="job.shift">
        <h4>Schedule</h4>
        <p>{{ job.shift }}</p>
      </div>

      <div class="content-block" v-if="job.responsibilities">
        <h4>Responsibilities</h4>
        <div class="html-content" v-html="job.responsibilities"></div>
      </div>

      <div class="content-block" v-if="job.requirements">
        <h4>Requirements</h4>
        <div class="html-content" v-html="job.requirements"></div>
      </div>
    </div>

    <div class="detail-footer">
      <button class="btn-apply-large" @click="$emit('apply', job)">APPLY NOW</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Job } from '@/services/types/job.types'

defineProps<{ job: Job }>()
defineEmits<{ (e: 'apply', job: Job): void }>()
</script>

<style scoped>
.detail-title {
  font-size: 1.8rem;
  font-weight: 800;
  color: #05162d;
  margin-bottom: 10px;
}

.detail-id {
  font-size: 1.2rem;
  color: #888;
  font-weight: 600;
  margin-left: 10px;
  vertical-align: middle;
}

.detail-meta {
  font-size: 0.95rem;
  color: #555;
  margin-bottom: 20px;
  font-weight: 600;
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.apply-container {
  margin-bottom: 30px;
}

.btn-apply-large {
  background-color: #32d26a;
  color: white;
  width: 100%;
  border: none;
  padding: 15px;
  font-weight: 800;
  border-radius: 999px;
  font-size: 1rem;
  cursor: pointer;
  transition: background 0.3s;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.btn-apply-large:hover {
  background-color: #28a755;
}

.divider {
  border: 0;
  height: 1px;
  background: #ccc;
  margin: 30px 0;
}

.detail-body {
  color: #333;
  line-height: 1.6;
}

.content-block {
  margin-bottom: 30px;
}

.content-block h4 {
  font-size: 1.1rem;
  font-weight: 700;
  margin-bottom: 10px;
  color: #05162d;
}

:deep(.html-content ul), :deep(.description ul) {
  padding-left: 20px;
  margin-bottom: 15px;
}

:deep(.html-content li), :deep(.description li) {
  margin-bottom: 8px;
  font-size: 0.95rem;
}

:deep(.description p) {
  margin-bottom: 15px;
}
</style>
