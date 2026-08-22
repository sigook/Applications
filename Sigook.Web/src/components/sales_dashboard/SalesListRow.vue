<template>
  <div class="sd-row">
    <span v-if="$slots.avatar" class="sd-row__avatar">
      <slot name="avatar"></slot>
    </span>
    <span class="sd-row__main">
      <span class="sd-row__title">
        <slot name="title"></slot>
      </span>
      <span class="sd-row__meta">
        <slot name="meta"></slot>
      </span>
    </span>
    <span v-if="$slots.trailing" class="sd-row__trailing">
      <slot name="trailing"></slot>
    </span>
  </div>
</template>

<script setup lang="ts"></script>

<style scoped lang="scss">
.sd-row {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.56rem 0.5rem;
  border-radius: 8px;

  &:hover {
    background: #f6f7f9;
  }
}

.sd-row__avatar {
  display: flex;
  align-items: center;
  justify-content: center;
  flex: none;
  width: 30px;
  height: 30px;
  border-radius: 50%;
}

.sd-row__main {
  display: flex;
  flex-direction: column;
  min-width: 0;
  flex: 1;
}

.sd-row__title {
  font-size: 0.8125rem;
  color: #333;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sd-row__meta {
  display: flex;
  align-items: center;
  gap: 6px;
  min-width: 0;
  font-size: 0.72rem;
  color: #9a9a9a;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sd-row__trailing {
  flex: none;
  white-space: nowrap;
}
</style>
