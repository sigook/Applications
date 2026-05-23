<template>
  <div class="slider-dots" role="tablist" :aria-label="ariaLabel">
    <button
      v-for="i in count"
      :key="i - 1"
      class="slider-dots__dot"
      :class="{ 'slider-dots__dot--active': modelValue === i - 1 }"
      :aria-label="`Slide ${i}`"
      :aria-selected="modelValue === i - 1"
      role="tab"
      type="button"
      @click="$emit('update:modelValue', i - 1)"
    />
  </div>
</template>

<script setup lang="ts">
/**
 * Glass-pill carousel dots used across Hero and Testimonials.
 *
 * Usage:
 *   <SliderDotsV2 v-model="currentIndex" :count="slides.length" aria-label="Carousel" />
 */
withDefaults(defineProps<{
  modelValue: number
  count: number
  ariaLabel?: string
}>(), {
  ariaLabel: 'Carousel navigation',
})

defineEmits<{
  'update:modelValue': [index: number]
}>()
</script>

<style scoped>
.slider-dots {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  background: rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(12px) saturate(150%);
  -webkit-backdrop-filter: blur(12px) saturate(150%);
  border: 1px solid rgba(255, 255, 255, 0.25);
  padding: 8px 14px;
  border-radius: 999px;
}

.slider-dots__dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.50);
  flex-shrink: 0;
  border: none;
  padding: 0;
  cursor: pointer;
  transition: width 0.25s ease, height 0.25s ease, background-color 0.25s ease;
}

.slider-dots__dot--active {
  width: 10px;
  height: 10px;
  background-color: #fff;
}
</style>
