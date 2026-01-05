<template>
  <div class="wrapper-job-positions">
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button v-if="isPayrollManager" type="is-ghost" icon-right="plus-circle"
          @click="showModal = true">Add</b-button>
        <b-button v-else type="is-ghost" icon-right="forum" @click="showModalRole = true">Ask for a new
          role</b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated detailed show-detail-icon
        pagination-rounded :per-page="pageSize" detail-transition="fade" :current-page.sync="pageIndex"
        :has-detailed-visible="(row) => row.description">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="value" label="Role" v-slot="props">
            {{ props.row.value }}
          </b-table-column>
          <b-table-column field="rate" label="Agency Rate" :visible="isPayrollManager" v-slot="props">
            {{ props.row.rate | currency }}
          </b-table-column>
          <b-table-column field="workerRate" label="Worker Rate" v-slot="props">
            {{ props.row.workerRate | currency }}
            <div class="is-inline-block">
              <b-tooltip label="Max" type="is-dark">
                <span v-if="props.row.workerRateMax">- {{ props.row.workerRateMax | currency }}
                </span>
              </b-tooltip>
            </div>
          </b-table-column>
          <b-table-column field="createdAt" label="Created" v-slot="props">
            {{ props.row.createdBy | emailName }}
            <i class="fz-2 block">{{ props.row.createdAt | dateMonth }}</i>
          </b-table-column>
          <b-table-column field="displayShift" label="Shift" v-slot="props">
            <roles-shift v-if="props.row.displayShift" :displayShift="props.row.displayShift" :roleId="props.row.id"
              :companyId="profileId" />
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <b-button type="is-info" outlined rounded icon-right="pencil" class="mr-2"
              @click="openEditModal(props.row)"></b-button>
            <b-button type="is-danger" outlined rounded icon-right="delete"
              @click="deleteAgencyCompanyJobPosition(props.row.id)"></b-button>
          </b-table-column>
        </template>
        <template #detail="props">
          <p v-if="props.row.description">{{ props.row.description }}</p>
        </template>
      </b-table>
    </div>

    <!-- Custom modal -->
    <b-modal v-model="showModal" @close="closeVueModal" width="850px">
      <position-form :current-position="currentPosition" :profile-id="profileId"
        @updateContent="onUpdateModal"></position-form>
    </b-modal>

    <!-- Request Role Modal -->
    <b-modal v-model="showModalRole" @close="closeRequestPositionModal" width="500px">
      <request-position-form :profile-id="profileId" @closeModal="closeRequestPositionModal" />
    </b-modal>
  </div>
</template>
<script>
import billingAdminMixin from "@/mixins/billingAdminMixin";

export default {
  data() {
    return {
      isLoading: true,
      totalItems: 0,
      rows: [],
      pageIndex: 1,
      pageSize: 30,
      profileId: this.$route.params.id,
      showModal: false,
      currentPosition: null,
      showModalRole: false,
    };
  },
  mixins: [billingAdminMixin],
  components: {
    PositionForm: () => import("@/components/agency_company/JobPositionForm"),
    RequestPositionForm: () => import("../../components/agency_company/RequestJobPositionForm"),
    RolesShift: () => import("../agency_company/RolesShiftDetail"),
  },
  methods: {
    async getAgencyCompanyJobPosition() {
      this.isLoading = true;
      this.rows = await this.$store.dispatch("agency/getAgencyCompanyJobPositions", this.profileId);
      this.rows = this.rows.map(i => ({ ...i, actions: null }));
      this.isLoading = false;
    },
    openEditModal(item) {
      this.currentPosition = item;
      this.showModal = true;
    },
    async onUpdateModal() {
      this.closeVueModal();
      await this.getAgencyCompanyJobPosition();
    },
    closeVueModal() {
      this.showModal = false;
      this.currentPosition = null;
    },
    deleteAgencyCompanyJobPosition(id) {
      this.showAlertConfirm("Are you sure", "You want to delete this position")
        .then((response) => {
          if (response) {
            this.isLoading = true;
            this.$store.dispatch("agency/deleteAgencyCompanyJobPosition", { profileId: this.profileId, id: id })
              .then(async () => {
                this.isLoading = false;
                this.showAlertSuccess("Deleted");
                await this.getAgencyCompanyJobPosition();
              })
              .catch((error) => {
                this.isLoading = false;
                this.showAlertError(error);
              });
          }
        });
    },
    closeRequestPositionModal() {
      this.showModalRole = false;
    },
  },
  async created() {
    await this.getAgencyCompanyJobPosition();
  },
};
</script>