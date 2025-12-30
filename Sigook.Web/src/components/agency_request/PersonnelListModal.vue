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
          <b-button v-if="item.active" type="is-danger" @click="deleteAgencyRequestRecruiter(item)">Remove</b-button>
          <b-button v-else type="is-primary is-light" @click="postAgencyRequestRecruiter(item)">Add</b-button>
        </li>
      </ul>
    </div>
  </div>
</template>
<script>

export default {
  props: ["request", "recruiters"],
  data() {
    return {
      isLoading: false,
      data: null
    };
  },
  methods: {
    getAgencyPersonnel() {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyPersonnel")
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
    postAgencyRequestRecruiter(item) {
      let model = { recruiterId: item.id };
      this.isLoading = true;
      this.$store.dispatch("agency/postAgencyRequestRecruiter", {
        requestId: this.request.id,
        model: model,
      }).then(() => {
        this.isLoading = false;
        this.$set(item, "active", true);
        this.$set(item, "recruiterId", item.id);
        this.$emit("selectUser", item);
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error);
      });
    },
    deleteAgencyRequestRecruiter(item) {
      this.isLoading = true;
      this.$store.dispatch("agency/deleteAgencyRequestRecruiter", {
        requestId: this.request.id,
        id: item.id,
      }).then(() => {
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
    this.getAgencyPersonnel();
  },
};
</script>
