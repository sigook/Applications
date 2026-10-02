<template>
  <section class="approval-status">
    <div class="approval-status-header">
      <span class="approval-status-icon" :class="props.approved ? 'is-success' : 'is-warning'">
        <b-icon :icon="props.approved ? 'check-circle-outline' : 'clock-outline'" size="is-small" />
      </span>
      <h3>{{ props.approved ? 'Approved to work' : 'Pending approval' }}</h3>
    </div>
    <p v-if="props.approved">You can apply to open job requests.</p>
    <p v-else>Your agency reviews your profile before you can apply to jobs. Complete the pending items to speed it up.</p>
    <router-link v-if="props.approved" to="/worker-requests" class="approval-status-link">See job requests</router-link>
  </section>
</template>

<script setup lang="ts">
const props = defineProps<{ approved: boolean }>();
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.approval-status {
  display: flex;
  flex-direction: column;
  gap: 10px;

  p {
    margin: 0;
    font-size: 0.875rem;
    line-height: 1.5;
    color: $navy;
  }
}

.approval-status-header {
  display: flex;
  align-items: center;
  gap: 10px;

  h3 {
    margin: 0;
    font-weight: 700;
  }
}

.approval-status-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  flex-shrink: 0;

  &.is-success {
    background: rgba($green, 0.18);
    color: $green-text;
  }

  &.is-warning {
    background: rgba($accent, 0.16);
    color: $accent-text;
  }
}

.approval-status-link {
  font-size: 0.875rem;
  font-weight: 600;
  color: $blue;
}
</style>
