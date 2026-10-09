<template>
  <div class="detail-layout"
    :class="{ 'has-rail': $slots.rail, 'is-rail-first': railFirst, 'has-index': sections.length > 1 }">
    <nav v-if="sections.length > 1" class="detail-index" :aria-label="ariaLabel ?? 'Page sections'">
      <span class="detail-index-eyebrow">On this page</span>
      <a v-for="section in sections" :key="section.id" :href="`#${section.id}`" class="detail-index-link"
        :class="{ 'is-active': section.id === activeId }" @click.prevent="selectSection(section.id)">
        <span>{{ section.label }}</span>
        <span v-if="section.hint" class="detail-index-hint">{{ section.hint }}</span>
        <span v-else-if="section.count !== undefined" class="detail-index-count">{{ section.count }}</span>
      </a>
    </nav>

    <div class="detail-layout-cards">
      <slot />
    </div>

    <aside v-if="$slots.rail" class="detail-layout-rail">
      <slot name="rail" />
    </aside>
  </div>
</template>

<script setup lang="ts">
import { nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import type { DetailSection } from '@/shared/detail-page/types';

const props = defineProps<{ sections: DetailSection[]; ariaLabel?: string; railFirst?: boolean }>();

const activeId = ref(props.sections[0]?.id ?? '');
let observer: IntersectionObserver | null = null;

function selectSection(id: string) {
  activeId.value = id;
  document.getElementById(id)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

function observeSections() {
  if (!observer) return;
  observer.disconnect();
  nextTick(() => {
    props.sections.forEach((section) => {
      const element = document.getElementById(section.id);
      if (element) observer?.observe(element);
    });
  });
}

onMounted(() => {
  if (typeof IntersectionObserver === 'undefined') return;
  observer = new IntersectionObserver((entries) => {
    const visible = entries.find((entry) => entry.isIntersecting);
    if (visible && props.sections.some((section) => section.id === visible.target.id)) activeId.value = visible.target.id;
  }, { rootMargin: '-80px 0px -70% 0px' });
  observeSections();
});

watch(() => props.sections.map((section) => section.id).join('|'), observeSections);

onBeforeUnmount(() => observer?.disconnect());
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

.detail-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr);
  gap: 24px;
  align-items: start;

  &.has-rail {
    grid-template-columns: minmax(0, 1fr) 320px;
  }

  &.has-index {
    grid-template-columns: 200px minmax(0, 1fr);
  }

  &.has-index.has-rail {
    grid-template-columns: 200px minmax(0, 1fr) 320px;
  }

  @include compact-desktop {
    gap: 16px;

    &.has-index {
      grid-template-columns: minmax(0, 1fr);
    }

    &.has-rail,
    &.has-index.has-rail {
      grid-template-columns: minmax(0, 1fr) 280px;
    }
  }

  @include touch {
    &,
    &.has-rail,
    &.has-index,
    &.has-index.has-rail {
      grid-template-columns: minmax(0, 1fr);
      gap: 12px;
    }
  }
}

.detail-index {
  position: sticky;
  top: 68px;
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 12px;
  background: $white;
  border: 1px solid $gray-border;
  border-radius: 12px;

  @include compact-desktop {
    display: none;
  }

  @include touch {
    display: none;
  }
}

.detail-index-eyebrow {
  padding: 4px 10px 8px;
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: $grey-font;
}

.detail-index-link {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  padding: 8px 10px;
  border-radius: 6px;
  font-size: 0.9rem;
  color: $navy;

  &:hover {
    background: $gray-bg;
  }

  &.is-active {
    background: rgba($blue, 0.08);
    font-weight: 600;
    color: $blue;
  }
}

.detail-index-hint {
  font-size: 0.7rem;
  font-weight: 400;
  color: $grey-font;
}

.detail-index-count {
  padding: 0 7px;
  border-radius: 999px;
  background: $gray-bg;
  font-size: 0.75rem;
  font-weight: 400;
  color: $grey-font;
}

.detail-layout-cards,
.detail-layout-rail {
  display: flex;
  flex-direction: column;
  gap: 20px;
  min-width: 0;

  @include touch {
    gap: 12px;
  }
}

.detail-layout.is-rail-first .detail-layout-rail {
  @include touch {
    order: -1;
  }
}
</style>
