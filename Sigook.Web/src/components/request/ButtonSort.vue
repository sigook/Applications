<template>
  <div class="d-inline-block">
    <button
      class="button-sort"
      :class="{ hidden: current === value && !isDesc }"
      @click="onClickSort(false)"
    >
      <img src="../../assets/images/up-arrow.png" alt="Up" />
    </button>
    <button
      class="button-sort"
      :class="{ hidden: current === value && isDesc }"
      @click="onClickSort(true)"
    >
      <img src="../../assets/images/down-arrow.png" alt="Down" />
    </button>
  </div>
</template>
<script>
export default {
  props: ["current", "value"],
  data() {
    return {
      isDesc: true,
    };
  },
  methods: {
    onClickSort(desc) {
      this.isDesc = desc;
      this.$emit("updateOrderBy", { value: this.value, desc: desc });
    },
  },
};
</script>
<style lang="scss">
.button-sort {
  border: 0;
  width: 10px;
  &:hover {
    transform: scale(1.2, 1.2);
  }
}
</style>