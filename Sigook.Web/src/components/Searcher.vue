<template>
  <div class="filter-request">
    <label>
      <button @click="showAll()" v-if="filter" class="clear-filter-button">
        <img src="../assets/images/icon_cross.svg" alt="clear" />
      </button>
      <input
        class="input-border input-block input-search"
        :placeholder="placeholder"
        v-model="filter"
        v-on:keyup.enter="filterResults(filter)"
        v-on:keyup="changeError()"
      />
    </label>
    <button
      :class="{ disabled: !filter || filter.length < 3 }"
      @click="filterResults(filter)"
    >
      {{ "Search" }}
    </button>
    <button @click="showAll()">{{ "All" }}</button>
    <transition name="fade">
      <span class="help is-danger no-margin tooltip2" v-if="error"
        >Please write at least 3 characters</span
      >
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const props = defineProps<{
  placeholder?: string;
  defaultFilter?: string;
  onShowAll?: boolean;
}>();

const emit = defineEmits<{
  (e: 'filterResults', value: string): void;
  (e: 'showAllResults', value: string): void;
}>();

const filter = ref('');
const error = ref(false);

if (props.defaultFilter) {
  filter.value = props.defaultFilter;
}

function filterResults(value: string) {
  error.value = false;
  if (filter.value && filter.value.length >= 2) {
    emit('filterResults', value);
  } else if (filter.value && filter.value.length <= 1) {
    error.value = true;
  } else {
    filter.value = '';
    emit('filterResults', '');
  }
}

function showAll() {
  filter.value = '';
  if (props.onShowAll) {
    emit('showAllResults', '');
  } else {
    emit('filterResults', '');
  }
}

function changeError() {
  if (error.value) {
    if (filter.value && filter.value.length >= 2) {
      error.value = false;
    }
  }
}

// Exposed (preserved from legacy)
function clear() {
  filter.value = '';
}

defineExpose({ clear });
</script>


<style lang="scss" scoped>
.filter-request {
  position: relative;
  transition: 0.4s all ease;
  .help.tooltip2 {
    position: absolute;
    background: #363636;
    color: whitesmoke;
    top: calc(100% + 5px + 2px);
    bottom: auto;
    padding: 0.35rem 0.75rem;
    border-radius: 6px;
    font-size: 0.85rem;
    font-weight: 400;
    -webkit-box-shadow: 0px 1px 2px 1px rgba(0, 1, 0, 0.2);
    box-shadow: 0px 1px 2px 1px rgba(0, 1, 0, 0.2);
    z-index: 888;
    white-space: nowrap;
    &:before {
      content: "";
      border-right: 5px solid transparent;
      border-bottom: 5px solid #363636;
      border-left: 5px solid transparent;
      position: absolute;
      top: -4px;
    }
  }
}

.filter-invoice.margin-0 {
  margin: 0;
}

.clear-filter-button {
  width: 30px;
  height: 30px;
  position: absolute;
  background: white;
  top: 0;
  left: 4px;
  border-radius: 50%;
}
</style>
