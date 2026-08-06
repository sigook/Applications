<template>
  <b-dropdown class="notification-bell" position="is-bottom-left" :mobile-modal="false" append-to-body aria-role="menu">
    <template #trigger>
      <div class="notification-bell-trigger" title="Notifications">
        <b-icon icon="bell-outline" size="is-medium"></b-icon>
        <span v-if="hasNotifications" class="notification-bell-dot"></span>
      </div>
    </template>

    <div class="notification-bell-header">Notifications</div>

    <b-dropdown-item v-if="!hasNotifications" custom aria-role="menuitem">
      <span class="notification-bell-empty">Nothing to review</span>
    </b-dropdown-item>

    <b-dropdown-item v-for="group in grouped" :key="group.type" aria-role="menuitem" @click="goTo(group.route)">
      <div class="notification-bell-summary">
        <span class="notification-bell-label">{{ group.label }}</span>
        <span class="notification-bell-count">{{ group.items.length }}</span>
      </div>
    </b-dropdown-item>
  </b-dropdown>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useNotifications } from '@/composables/useNotifications';

const router = useRouter();
const { grouped, hasNotifications, load } = useNotifications();

function goTo(route: string) {
  router.push(route);
}

load();
</script>

<style lang="scss" scoped>
.notification-bell-trigger {
  position: relative;
  display: flex;
  align-items: center;
  cursor: pointer;
  color: #555;
}

.notification-bell-dot {
  position: absolute;
  top: 0;
  right: 0;
  width: 10px;
  height: 10px;
  border-radius: 999px;
  background-color: #ff3860;
  border: 2px solid #fff;
}

.notification-bell-header {
  padding: 8px 16px;
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  color: #888;
}

.notification-bell-empty {
  font-size: 13px;
  color: #999;
}

.notification-bell-summary {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  min-width: 220px;
}

.notification-bell-label {
  font-weight: 600;
}

.notification-bell-count {
  min-width: 22px;
  height: 22px;
  padding: 0 7px;
  border-radius: 999px;
  background-color: #00adef;
  color: #fff;
  font-size: 12px;
  font-weight: 700;
  line-height: 22px;
  text-align: center;
}
</style>
