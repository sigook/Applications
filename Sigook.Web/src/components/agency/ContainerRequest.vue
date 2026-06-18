<template>
  <div class="container-requests">
    <router-link :to="'/agency-request/' + data.id">
      <div class="container-requests-top">
        <span class="asap" v-if="data.isAsap">{{ 'Asap' }}</span>
        <img :src="data.logo" class="request-logo"/>
        <div class="container-title">
          <h3 class="light-title">
            {{ data.jobTitle }}
          </h3>
          <div class="subtitle">{{ data.companyFullName }}</div>
        </div>
        <div class="text-end container-right">
          <div class="subtitle icon-before-location text-uppercase icon-before"> {{ data.location }}</div>
        </div>
      </div>
      <div class="container-actions">
        <div>
          <b>Id:</b> {{ data.numberId }}

          <i class="icon-time margin-left"></i>
          {{ dateFromNow(data.createdAt) }}

          <i class="icon-workers"></i>
          <span class="color-green fw-normal">{{ data.workersQuantityWorking }} / {{ data.workersQuantity }}</span>

          <i v-if="showFinishAt(data.finishAt)" class="icon-time margin-left"></i>
          <span v-if="showFinishAt(data.finishAt)" v-bind:class="{'color-danger fw-normal' : showFinishWarning(data.finishAt)}">
            Finish: {{dateFromNow(data.finishAt) }}
          </span>
        </div>

        <div class="container-status text-uppercase" :class="'status-' + data.status.toLowerCase()"
             v-status="{status: data.status}"> {{ data.status }}
        </div>
      </div>
    </router-link>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAppStore } from '@/stores/app';
import { dateFromNow } from '@/utils/filters';

defineProps<{ data: any }>();

const appStore = useAppStore();
const now = ref<Date>(new Date());

function showFinishAt(date: any): boolean {
  try {
    if (!date) return false;
    const date1 = new Date(date);
    if (!date1) return false;
    return date1 >= now.value;
  } catch {
    return false;
  }
}

function showFinishWarning(date: any): boolean {
  try {
    if (!showFinishAt(date)) return false;
    const milliseconds = Math.abs(new Date(date).getTime() - now.value.getTime());
    const days = Math.floor(milliseconds / (24 * 60 * 60 * 1000));
    return days <= 7;
  } catch {
    return false;
  }
}

appStore.getCurrentDate().then(response => {
  now.value = response;
});
</script>

<style scoped lang="scss">
.icon-time {
  width: 11px;
  height: 11px;
  margin-right: 5px;
  display: inline-block;
  background-image: url('../../assets/images/icon_time.png');
  background-size: contain;
  opacity: 0.8;
  margin-bottom: -1px;
}

.icon-workers {
  width: 14px;
  height: 15px;
  margin-right: 5px;
  display: inline-block;
  background-image: url('../../assets/images/icon_workers.png');
  background-size: contain;
  opacity: 0.8;
  margin-bottom: -3px;
  margin-left: 20px;
}
</style>
