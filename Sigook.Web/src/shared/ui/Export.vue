<template>
  <b-field grouped position="is-right">
    <slot name="actions"></slot>
    <b-dropdown aria-role="list" position="is-bottom-left" append-to-body>
      <template #trigger>
        <b-button type="is-ghost" icon-right="chevron-down" icon-left="dots-vertical">Actions</b-button>
      </template>
      <slot name="dropdown-actions"></slot>
      <b-dropdown-item aria-role="listitem" @click="downloadReport">
        <b-icon icon="file-excel"></b-icon>
        <span>Export</span>
      </b-dropdown-item>
    </b-dropdown>
  </b-field>
</template>
<script setup lang="ts">
import { downloadFile } from "@/shared/utils/downloadFile";
import { downloadAgencyReport, type ReportQueryParams } from '@/shared/api/reportApi';
import type { GridParams } from '@/shared/types/common';

const props = defineProps<{
  url: string;
  params?: ReportQueryParams | GridParams;
  fileName?: string;
}>();

const emit = defineEmits<{ (e: 'onDataLoading', loading: boolean): void }>();

function downloadReport() {
  emit('onDataLoading', true);
  downloadAgencyReport(props.url, props.params)
    .then(file => {
      emit('onDataLoading', false);
      downloadFile(file, `${props.fileName}_${new Date().toLocaleDateString()}`);
    })
    .catch(() => emit('onDataLoading', false));
}
</script>
