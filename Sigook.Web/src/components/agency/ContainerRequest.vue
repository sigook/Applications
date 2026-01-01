<template>
  <div class="container-requests">
    <router-link :to="'/agency-request/' + data.id">
      <div class="container-requests-top">
        <span class="asap" v-if="data.isAsap">{{ $t('Asap') }}</span>
        <img :src="data.logo" class="request-logo"/>
        <div class="container-title">
          <h3 class="light-title">
            {{ data.jobTitle }}
          </h3>
          <div class="subtitle">{{ data.companyFullName }}</div>
        </div>
        <div class="text-right container-right">
          <div class="subtitle icon-before-location uppercase icon-before"> {{ data.location }}</div>
        </div>
      </div>
      <div class="container-actions">
        <div>
          <b>Id:</b> {{ data.numberId }}

          <i class="icon-time margin-left"></i>
          {{ data.createdAt | dateFromNow }}

          <i class="icon-workers"></i>
          <span class="color-green fw-400">{{ data.workersQuantityWorking }} / {{ data.workersQuantity }}</span>

          <i v-if="showFinishAt(data.finishAt)" class="icon-time margin-left"></i>
          <span v-if="showFinishAt(data.finishAt)" v-bind:class="{'color-danger fw-400' : showFinishWarning(data.finishAt)}">
            Finish: {{data.finishAt | dateFromNow }}
          </span>
        </div>

        <div class="container-status uppercase" :class="'status-' + data.status.toLowerCase()"
             v-status="{status: data.status}"> {{ $t(data.status) }}
        </div>
      </div>
    </router-link>
  </div>
</template>

<script>
export default {
  props: ['data'],
  data() {
    return {
      now: new Date()
    }
  },
  methods: {
    showFinishAt(date){
      try {
        if (!date) return false;
        let date1 = new Date(date);
        if (!date1) return false;
        return date1 >= this.now;
      }catch (e) {
        return false;
      }
    },
    showFinishWarning(date) {
      try {
        if(!this.showFinishAt(date)) return false;
        let milliseconds = Math.abs(new Date(date) - this.now);
        let days = Math.floor(milliseconds / (24 * 60 * 60 * 1000));
        return days <= 7;
      } catch (e) {
        return false;
      }
    }
  },
  created() {
    this.$store.dispatch('getCurrentDate').then(response => {
      this.now = response;
    })
  }
}
</script>
