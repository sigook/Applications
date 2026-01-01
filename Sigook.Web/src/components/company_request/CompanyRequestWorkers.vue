<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <b-table :data="rows" narrowed hoverable :mobile-cards="false" paginated backend-pagination backend-sorting
      pagination-rounded :total="totalItems" :per-page="serverParams.pageSize"
      :current-page.sync="serverParams.pageIndex" default-sort="name" @page-change="onPageChange" @sort="onSortChange">
      <template v-slot:empty>
        <p class="container text-center">No records available</p>
      </template>
      <template>
        <b-table-column field="profileImage" width="50" v-slot="props">
          <img v-if="props.row.profileImage" :src="props.row.profileImage" alt="profile image"
            class="img-30 img-rounded" />
          <default-image v-else :name="props.row.fullName" class="img-30"></default-image>
        </b-table-column>
        <b-table-column field="numberId" width="100" label="ID" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.numberId" placeholder="Search..." icon="magnify" size="is-small"
              @keypress.native="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            <span :class="props.row.isSubcontractor ? 'Blue' : ''">{{ props.row.numberId }}</span>
          </template>
        </b-table-column>
        <b-table-column field="name" label="Name" sortable searchable>
          <template v-slot:searchable>
            <b-input v-model="serverParams.name" placeholder="Search..." icon="magnify" size="is-small"
              @keypress.native="onInputEntered"></b-input>
          </template>
          <template v-slot="props">
            {{ props.row.name }}
          </template>
        </b-table-column>
        <b-table-column field="startWorking" label="Start Working" sortable searchable>
          <template v-slot:searchable>
            <b-datepicker size="is-small" :mobile-native="false" placeholder="Search..."
              :icon-right="startWorkingDatesSelected.length > 0 ? 'close-circle' : ''" icon-right-clickable
              @icon-right-click="onStartWorkingCleared" range v-model="startWorkingDatesSelected"
              @input="onStartWorkingSelected">
            </b-datepicker>
          </template>
          <template v-slot="props">
            {{ props.row.startWorking | dateMonth }}
          </template>
        </b-table-column>
        <b-table-column field="status" label="Status" width="250px" sortable searchable>
          <template v-slot:searchable>
            <b-taginput size="is-small" v-model="statusesSelected" autocomplete :data="statuses" open-on-focus
              field="value" icon="label" placeholder="Select Status" @input="onStatusSelected">
            </b-taginput>
          </template>
          <template v-slot="props">
            <span class="uppercase fw-700 fz-1" :class="props.row.status">{{ props.row.status }}</span>
          </template>
        </b-table-column>
        <b-table-column field="actions" v-slot="props">
          <b-tooltip label="Reject" type="is-dark" position="is-top">
            <b-button size="is-small" type="is-danger" outlined rounded icon-right="close"
              v-if="props.row.status === 'Booked'" @click="confirmDelete(props.row)"></b-button>
          </b-tooltip>
        </b-table-column>
      </template>
    </b-table>

    <!-- Reject worker Message -->
    <transition name="modal">
      <div v-if="modalRejectWorker" class="vue-modal">
        <div class="modal-mask">
          <div class="modal-wrapper">
            <div class="modal-container modal-light small-container border-radius">
              <button @click="modalRejectWorker = false" class="cross-icon">{{ $t('Close') }}</button>
              <edit-textarea title="Reject Worker" :subtitle="'Please indicate the reason.'" :min-length="10"
                class="sm-edit-textarea" @updateContent="(data) => rejectCompanyRequestWorker(data)" />
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- Reject worker Message -->
  </div>
</template>

<script>

export default {
  data() {
    return {
      isLoading: false,
      totalItems: 0,
      rows: [],
      statuses: [
        { id: 2, value: 'Rejected' },
        { id: 3, value: 'Booked' },
      ],
      statusesSelected: [],
      startWorkingDatesSelected: [],
      modalRejectWorker: false,
      currentWorker: null,
      serverParams: {
        sortBy: 1,
        requestId: this.$route.params.id,
        pageIndex: 1,
        pageSize: 30
      }
    }
  },
  components: {
    EditTextarea: () => import("../../components/agency_request/EditTextarea"),
  },
  methods: {
    onPageChange(params) {
      this.serverParams.pageIndex = params;
      this.getWorkers();
    },
    onSortChange(field, order) {
      switch (field) {
        case 'numberId':
          this.serverParams.sortBy = 0;
          break;
        case 'name':
          this.serverParams.sortBy = 1;
          break;
        case 'status':
          this.serverParams.sortBy = 2;
          break;
        case 'startWorking':
          this.serverParams.sortBy = 3;
          break;
      }
      this.serverParams.isDescending = order !== 'asc';
      this.getWorkers();
    },
    onInputEntered(event) {
      if (event.key === 'Enter') {
        this.getWorkers();
      }
    },
    onStatusSelected() {
      this.serverParams.statuses = this.statusesSelected.map(ss => ss.id);
      this.getWorkers();
    },
    onStartWorkingSelected() {
      this.serverParams.startWorkingFrom = this.startWorkingDatesSelected[0];
      this.serverParams.startWorkingTo = this.startWorkingDatesSelected[1];
      this.getWorkers();
    },
    onStartWorkingCleared() {
      this.startWorkingDatesSelected = [];
      this.onStartWorkingSelected();
    },
    getWorkers() {
      this.isLoading = true;
      this.$store.dispatch('company/getRequestWorkers', this.serverParams)
        .then((response) => {
          this.rows = response.items.map(i => ({ ...i, actions: null }));
          this.totalItems = response.totalItems;
          this.isLoading = false;
        })
        .catch(error => {
          this.isLoading = false;
          this.showAlertError(error.data);
        })
    },
    confirmDelete(worker) {
      this.currentWorker = worker;
      this.modalRejectWorker = true;
    },
    rejectCompanyRequestWorker(comments) {
      this.modalRejectWorker = false;
      this.isLoading = true;
      this.$store.dispatch('company/rejectCompanyRequestWorker', {
        requestId: this.serverParams.requestId,
        workerId: this.currentWorker.workerId,
        model: { comments: comments }
      }).then(() => {
        this.isLoading = false;
        this.getWorkers();
      }).catch(error => {
        this.isLoading = false;
        this.showAlertError(error.data);
      })
    }
  },
  created() {
    this.getWorkers();
  }
}
</script>
