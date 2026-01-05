<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <div>
      <b-field grouped position="is-right">
        <b-button @click="modalManageWorkers = true">Manage Applicants</b-button>
      </b-field>
      <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
        pagination-rounded :total="totalItems" :per-page="serverParams.pageSize" focuseable default-sort="createdBy"
        :current-page.sync="serverParams.pageIndex" @page-change="onPageChange" @sort="onSortChange"
        @cellclick="onCellClick">
        <template v-slot:empty>
          <p class="container text-center">No records available</p>
        </template>
        <template>
          <b-table-column field="profileImage" width="50" v-slot="props">
            <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image" class="img-30" />
            <default-image v-else :name="props.row.name" class="img-30"></default-image>
          </b-table-column>
          <b-table-column field="name" label="Name" sortable searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered"></b-input>
            </template>
            <template v-slot="props">
              <span class="d-block">
                {{ props.row.name }}
                <b-tooltip label="Candidate" type="is-dark">
                  <b-icon v-if="props.row.candidateId" icon="account-group" size="is-small"></b-icon>
                </b-tooltip>
                <b-tooltip label="Worker" type="is-dark">
                  <b-icon v-if="props.row.workerProfileId" icon="badge-account-outline" size="is-small"></b-icon>
                </b-tooltip>
              </span>
              <i class="fz-2 ellipsis-150 lowercase">
                <a :href="'mailto:' + props.row.email">{{ props.row.email }}</a>
              </i>
            </template>
          </b-table-column>
          <b-table-column field="phoneNumber" label="Phone" searchable>
            <template v-slot:searchable>
              <b-input v-model="serverParams.phone" placeholder="Search..." icon="magnify" size="is-small"
                @keypress.native="onInputEntered" v-cleave="mask"></b-input>
            </template>
            <template v-slot="props">
              <div v-if="props.row.phoneNumber">
                {{ props.row.phoneNumber }}
              </div>
              <div v-else class="op3">Phone</div>
            </template>
          </b-table-column>
          <b-table-column field="createdBy" label="Added By" sortable searchable>
            <template v-slot:searchable>
              <b-field>
                <b-input size="is-small" icon="magnify" placeholder="Created By" v-model="serverParams.createdBy"
                  @keypress.native="onInputEntered"></b-input>
                <b-datepicker size="is-small" :mobile-native="false" placeholder="Created At"
                  :icon-right="createdAtDatesSelected.length > 0 ? 'close-circle' : ''" range
                  v-model="createdAtDatesSelected" icon-right-clickable @icon-right-click="onCreatedAtCleared"
                  @input="onCreatedAtSelected"></b-datepicker>
              </b-field>
            </template>
            <template v-slot="props">
              <div class="capitalize" v-if="props.row.createdBy">
                <p>{{ props.row.createdBy | emailName }}</p>
              </div>
              <div v-else class="op3">Added by</div>
              <i class="fz-2">{{ props.row.createdAt | dateMonth }}</i>
            </template>
          </b-table-column>
          <b-table-column field="comments" label="Comments" v-slot="props">
            <span v-html="props.row.comments"></span>
            <b-button type="is-ghost" icon-right="pencil" @click="showEditModal(props.row, props.row.index)">
            </b-button>
          </b-table-column>
          <b-table-column field="actions" v-slot="props">
            <floating-menu class="text-center">
              <template v-slot:options>
                <button class="floating-menu-item" v-if="props.row.candidateId"
                  @click="convertToWorker(props.row.candidateId)">
                  Convert to Worker
                </button>
                <button class="floating-menu-item" @click="deleteAgencyRequestApplicant(props.row)">
                  Delete
                </button>
              </template>
            </floating-menu>
          </b-table-column>
        </template>
      </b-table>
    </div>

    <!-- custom modal Manage Workers / Applicants-->
    <transition name="modal">
      <div v-if="modalManageWorkers" class="vue-modal header-fixed">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light full-container overflow-initial border-radius">
              <button @click="modalManageWorkers = false" type="button" class="cross-icon">
                close
              </button>
              <manage-tabs @updateApplicants="(args) => postAgencyRequestApplicant(args.model)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- end custom modal  Manage Workers / Applicant-->

    <!-- custom modal TextArea-->
    <b-modal v-model="modalComment" width="500px">
      <edit-textarea v-if="currentItem" :title="'Comments'" subtitle="Comments" :min-length="0" :data="currentItem.comments"
        @updateContent="(data) => updateAgencyRequestApplicant(data)"></edit-textarea>
    </b-modal>
  </div>
</template>
<script>
import phoneMaskMixin from "@/mixins/phoneMaskMixin"

export default {
  props: ["request"],
  data() {
    return {
      isLoading: false,
      currentItem: null,
      createdAtDatesSelected: [],
      modalManageWorkers: false,
      modalComment: false,
      totalItems: 0,
      rows: [],
      serverParams: {
        sortBy: 1,
        requestId: this.$route.params.id,
        pageIndex: 1,
        pageSize: 30,
        isDescending: true,
      }
    };
  },
  mixins: [phoneMaskMixin],
  components: {
    manageTabs: () => import("./ManageApplicantsModal"),
    EditTextarea: () => import("../../components/agency_request/EditTextarea"),
    FloatingMenu: () => import("@/components/FloatingMenuDots")
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getAgencyRequestApplicant();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'name':
          this.serverParams.sortBy = 0;
          break;
        case 'createdBy':
          this.serverParams.sortBy = 1;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getAgencyRequestApplicant();
    },
    onCellClick(row, column) {
      switch (column._props.field) {
        case 'comments':
        case 'actions':
          break;
        default:
          if (row.workerProfileId) {
            this.$router.push(`/agency-workers/worker/${row.workerProfileId}`);
          }
      }
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getAgencyRequestApplicant();
      }
    },
    onCreatedAtCleared() {
      this.createdAtDatesSelected = [];
      this.onCreatedAtSelected();
    },
    onCreatedAtSelected() {
      this.serverParams.createdAtFrom = this.createdAtDatesSelected[0];
      this.serverParams.createdAtTo = this.createdAtDatesSelected[1];
      this.getAgencyRequestApplicant();
    },
    getAgencyRequestApplicant() {
      this.isLoading = true;
      this.$store.dispatch("agency/getAgencyRequestApplicant", this.serverParams)
        .then((response) => {
          this.rows = response.items.map(c => ({ ...c, actions: null }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    postAgencyRequestApplicant(model) {
      this.modalManageWorkers = false;
      this.isLoading = true;
      this.$store.dispatch("agency/postAgencyRequestApplicant", {
        requestId: this.requestId,
        model: model,
      }).then(() => {
        this.isLoading = false;
        this.getAgencyRequestApplicant();
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error);
      });
    },
    deleteAgencyRequestApplicant(item) {
      this.isLoading = true;
      this.$store.dispatch("agency/deleteAgencyRequestApplicant", {
        requestId: this.requestId,
        id: item.id,
      }).then(() => {
        this.isLoading = false;
        this.getAgencyRequestApplicant();
      }).catch((error) => {
        this.isLoading = false;
        this.showAlertError(error);
      });
    },
    showEditModal(item) {
      this.currentItem = item;
      this.modalComment = true;
    },
    updateAgencyRequestApplicant(comment) {
      this.modalComment = false;
      this.isLoading = true;
      this.$store.dispatch("agency/updateAgencyRequestApplicant", {
        requestId: this.requestId,
        id: this.currentItem.id,
        model: { comments: comment },
      })
        .then(() => {
          this.isLoading = false;
          this.getAgencyRequestApplicant();
        })
        .catch((error) => {
          this.isLoading = false;
          this.showAlertError(error);
        });
    },
    convertToWorker(candidateId) {
      this.isLoading = true;
      this.$store.dispatch("agency/convertToWorker", candidateId)
        .then(() => {
          this.isLoading = false;
          this.getAgencyRequestApplicant();
        })
        .catch((error) => {
          this.isLoading = false
          this.showAlertError(error);
        })
    }
  },
  created() {
    this.getAgencyRequestApplicant();
  }
};
</script>