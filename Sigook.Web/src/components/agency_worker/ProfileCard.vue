<template>
  <section class="profile-card">
    <header class="profile-card-header">
      <h3 class="profile-card-title">{{ props.title }}</h3>
      <div v-if="$slots.actions" class="profile-card-actions">
        <slot name="actions" />
      </div>
    </header>
    <slot />
  </section>
</template>

<script setup lang="ts">
const props = defineProps<{ title: string }>();
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.profile-card {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin: 0;
}

.profile-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.profile-card-title {
  margin: 0;
  font-size: 1.05rem;
  font-weight: 700;
  color: $navy;
}

.profile-card-actions {
  display: flex;
  align-items: center;
  gap: 4px;
}
</style>
