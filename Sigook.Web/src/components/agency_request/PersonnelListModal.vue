<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div class="p-3">
      <h2 class="text-center fz1">Recruiters</h2>
      <ul v-if="data">
        <li v-for="item in data" :key="item.id" class="list-item-border-bottom content-flex-between align-center mb-0">
          <div>
            {{ item.email }}
          </div>
          <b-button v-if="item.active" type="is-danger" @click="removeRequestRecruiter(item)">Remove</b-button>
          <b-button v-else type="is-primary is-light" @click="addRequestRecruiter(item)">Add</b-button>
        </li>
      </ul>
    </div>
  </div>
</template>
<script lang="ts">
import { getAgencyPersonnel } from "@/api/agencyApi";
import { postAgencyRequestRecruiter, deleteAgencyRequestRecruiter } from "@/api/agencyRequestApi";

export default {
  props: ["request", "recruiters"],
  data() {
    return {
      isLoading: false,
      data: null
    };
  },
  methods: {
    loadAgencyPersonnel() {
      this.isLoading = true;
      getAgencyPersonnel()
        .then((response) => {
          this.isLoading = false;
          this.data = response;
          this.updateRecruiters(this.recruiters);
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    updateRecruiters(items) {
      for (let i = 0; i < items.length; i++) {
        for (let j = 0; j < this.data.length; j++) {
          if (items[i].toLowerCase() === this.data[j].name.toLowerCase()) {
            this.$set(this.data[j], "active", true);
          }
        }
      }
    },
    addRequestRecruiter(item) {
      this.isLoading = true;
      postAgencyRequestRecruiter(this.request.id, { recruiterId: item.id }).then(() => {
        this.isLoading = false;
        this.$set(item, "active", true);
        this.$set(item, "recruiterId", item.id);
        this.$emit("selectUser", item);
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error);
      });
    },
    removeRequestRecruiter(item) {
      this.isLoading = true;
      deleteAgencyRequestRecruiter(this.request.id, item.id).then(() => {
        this.isLoading = false;
        item.active = false;
        this.$emit("removeUser", item);
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error);
      });
    },
  },
  created() {
    this.loadAgencyPersonnel();
  },
};
</script>
