<template>
  <div class="company-wrapper white-container-mobile">
    <b-loading v-model="isLoading"></b-loading>

    <section class="company-top" v-if="agency">
      <div class="hover-actions">
        <h2 class="capitalize fz1 fw-700">
          <span class="fw-400 fz-1" v-if="agency.numberId">{{ agency.numberId }} |
          </span>
          {{ lowercase(agency.fullName) }}
        </h2>
      </div>
    </section>

    <b-tabs v-model="currentTab" @input="changeTab" v-if="agency">
      <b-tab-item label="Orders" value="Orders">
        <agency-requests v-if="visitedTabs.includes('Orders')" :agency="agency" class="p-2" />
      </b-tab-item>
    </b-tabs>
  </div>
</template>

<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { getAgency } from "@/api/agencyApi";
import { lowercase } from '@/utils/filters';

export default {
  data() {
    return {
      currentTab: "Orders",
      visitedTabs: ["Orders"],
      agency: null,
      isLoading: true
    };
  },
  components: {
    AgencyRequests: () => import("@/components/agency/AgencyRequests.vue")
  },
  methods: {
    lowercase,
    changeTab(tab) {
      if (!this.visitedTabs.includes(tab)) {
        this.visitedTabs.push(tab);
      }
      this.$router.push({
        path: `/agency-detail/${this.$route.params.id}`,
        query: {
          tab: tab
        }
      });
    },
    loadAgency() {
      this.isLoading = false;
      getAgency(this.$route.params.id)
        .then((agency) => {
          this.agency = agency;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          showAlertError(error);
        });
    }
  },
  created() {
    this.loadAgency();
    if (this.$route.query && this.$route.query.tab) {
      this.currentTab = this.$route.query.tab;
      if (!this.visitedTabs.includes(this.$route.query.tab)) {
        this.visitedTabs.push(this.$route.query.tab);
      }
    }
  }
};
</script>
