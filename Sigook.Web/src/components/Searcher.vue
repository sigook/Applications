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
      :class="{ disabled: !this.filter || this.filter.length < 3 }"
      @click="filterResults(filter)"
    >
      {{ $t("Search") }}
    </button>
    <button @click="showAll()">{{ $t("All") }}</button>
    <transition name="fade">
      <span class="help is-danger no-margin tooltip2" v-if="error"
        >Please write at least 3 characters</span
      >
    </transition>
  </div>
</template>

<script>
export default {
  props: ["placeholder", "defaultFilter", "onShowAll"],
  data() {
    return {
      filter: "",
      error: false,
    };
  },
  created() {
    if (this.defaultFilter) {
      this.filter = this.defaultFilter;
    }
  },
  methods: {
    filterResults(value) {
      this.error = false;
      if (this.filter && this.filter.length >= 2) {
        this.$emit("filterResults", value);
      } else if (this.filter && this.filter.length <= 1) {
        this.error = true;
      } else {
        this.filter = "";
        this.$emit("filterResults", "");
      }
    },
    showAll() {
      this.filter = "";
      if (this.onShowAll) {
        this.$emit("showAllResults", "");
      } else {
        this.$emit("filterResults", "");
      }
    },
    changeError() {
      if (this.error) {
        if (this.filter && this.filter.length >= 2) {
          this.error = false;
        }
      }
    },
    clear() {
      this.filter = "";
    },
  },
};
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