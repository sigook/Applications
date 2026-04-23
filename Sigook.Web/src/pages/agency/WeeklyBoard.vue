<template>
    <div class="body-top-on-pagination white-container-mobile">
        <b-loading v-model="isLoading"></b-loading>
        <h2 v-if="data" class="fz1 pt-3 pb-3 col-8 col-md-6 col-sm-12">
            <ul class="tabs-basic tabs-home d-inline-block">
                <li>
                    <router-link to="/agency-requests">
                        Orders
                    </router-link>
                </li>
                <li class="active">
                    Weekly Board
                    <span class="fw-100 fz-1">({{data.totalItems}})</span>
                </li>
            </ul>
        </h2>
        <div v-if="data" class="scroll-desktop">
            <table class="bordered-cells main-table">
                <col width="7%">
                <col width="10%">
                <col width="19%">
                <col width="19%">
                <col width="5%">
                <col width="15%">
                <col width="12%">
                <col width="10%">
                <col width="6%">
                <col width="3%">
                <col width="3%">
                <td></td>
                <thead>
                    <tr>
                        <td>Order Id</td>
                        <td class="min-120">Start date</td>
                        <td>Client</td>
                        <td>Role</td>
                        <td>Rate</td>
                        <td>Worker</td>
                        <td>Phone Number</td>
                        <td>Recruiter</td>
                        <td></td>
                        <td></td>
                    </tr>
                </thead>
                <tbody v-for="(item, index) in data.items" :key="item.id">
                    <tr v-if="addBreak(index)">
                        <td colspan="10" style="border-left: 1px solid white; border-right: 1px solid white;">
                            <div class="fz-14 fw-700 color-primary pt-3">Week {{dateMonth(item.weekStartWorking)}}</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <router-link :to="'/agency-request/' + item.requestId">
                                {{item.numberId}}
                            </router-link>
                        </td>
                        <td>{{dateMonth(item.startWorking)}}</td>
                        <td>
                            <router-link :to="'/agency-companies/company/' + item.companyProfileId">
                                {{item.companyFullName}}
                            </router-link>
                            <span class="fz-2 d-block">
                                {{item.location}}
                                <span v-if="item.entrance"> - {{item.entrance}}</span>
                            </span>
                        </td>
                        <td>
                            <router-link :to="'/agency-request/' + item.requestId">
                                {{item.jobTitle}}
                            </router-link>
                            <agency-shift class="pl-0 fz-2 d-block"
                                          :requestId="item.requestId"
                                          :displayShift="item.displayShift" />
                            <span class="fz-2 d-block">{{ DurationTermLabels[item.durationTerm] }}</span>
                        </td>
                        <td>{{currency(item.workerRate)}}</td>
                        <td>
                            <router-link :to="'/agency-workers/worker/' + item.workerProfileId"
                                         :class="workerColor(item)">
                                {{item.firstName}} {{item.lastName}}
                            </router-link>
                            <div class="pl-0 pt-0 line-height-1">
                                <span class="fz-2" v-if="item.socialInsurance">SIN {{item.socialInsurance}}</span>
                                <span v-if="item.socialInsuranceExpire" class="fz-2"> | {{dateMonth(item.dueDate)}}</span>
                            </div>
                            <span class="fz-1 d-block" v-if="item.rejectComments">
                                <span class="orange-dot"></span>
                                {{item.rejectComments}}</span>
                        </td>
                        <td>{{item.mobileNumber}}</td>
                        <td>{{breakWord(item.displayRecruiters)}}</td>
                        <td>
                            <div class="capitalize is-inline-block v-middle w-100 text-right">

                                <b-tooltip :label="RequestStatusLabels[item.requestStatus]" type="is-dark" append-to-body>
                                    <div class="dot-status" :class="'status-' + RequestStatusLabels[item.requestStatus].toLowerCase()"></div>
                                </b-tooltip>

                            </div>
                        </td>
                        <td @mouseleave="hideNotes(index)">
                            <div class="p-0">
                                <div class="relative d-inline-block" @click.stop="showNotesClick(index)">
                                    <button type="button"
                                            class="btn-icon-sm btn-icon-notes margin-0 bg-transparent"
                                            style="position: relative; top: -4px; width: 24px;">
                                        NOTES
                                    </button>
                                    <span v-if="item.notesCount" class="notes-notification">
                                        {{item.notesCount < 10 ? item.notesCount : '+'}}
                                    </span>
                                </div>
                                <div v-if="item.showNotes" class="notes-tooltip">
                                    <modal-notes :request-id="item.requestId"
                                                 :user-id="item.id"
                                                 :on-get="getNotes"
                                                 :on-create="createNote"
                                                 :on-delete="deleteNote"
                                                 :on-update="updateNote"
                                                 @onUpdateNote="(val) => item.notesCount = val.size">
                                    </modal-notes>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            <pagination :total-pages="data.totalPages"
                        :index-page="data.pageIndex"
                        :size-page="size"
                        @changePage="(index) => loadBoard(index)">
            </pagination>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { showAlertError } from '@/utils/toast';
import dayjs from 'dayjs';
import { getAgencyRequestBoard } from '@/api/agencyRequestApi';
import {
  getAgencyRequestWorkerNotes,
  createAgencyRequestWorkerNote,
  updateAgencyRequestWorkerNote,
  deleteAgencyRequestWorkerNote,
} from '@/api/agencyNoteApi';
import type { RequestNotesFetchPayload, RequestNotesCreatePayload, RequestNotesUpdatePayload, RequestNotesDeletePayload } from '@/types/agency';
import { WorkerRequestStatus, DurationTermLabels, RequestStatusLabels } from '@/constants/enums';
import { dateMonth, currency, breakWord } from '@/utils/filters';
import Pagination from '@/components/Paginator.vue';
import ModalNotes from '@/components/notes/ModalNotes.vue';
import AgencyShift from '@/components/agency_request/AgencyShiftDetail.vue';

const size = ref(30);
const currentPage = ref(1);
const data = ref<any>(null);
const isLoading = ref(false);
const momentFormat = 'YYYY-MM-DD';

const getNotes = ({ requestId, userId, pagination }: RequestNotesFetchPayload) => getAgencyRequestWorkerNotes(requestId, userId, pagination);
const createNote = ({ requestId, userId, model }: RequestNotesCreatePayload) => createAgencyRequestWorkerNote(requestId, userId, model);
const updateNote = ({ requestId, userId, id, model }: RequestNotesUpdatePayload) => updateAgencyRequestWorkerNote(requestId, userId, id, model);
const deleteNote = ({ requestId, userId, id }: RequestNotesDeletePayload) => deleteAgencyRequestWorkerNote(requestId, userId, id);

loadBoard(currentPage.value);

function loadBoard(index: number) {
  isLoading.value = true;
  getAgencyRequestBoard({ page: index, size: size.value })
    .then((response: any) => {
      data.value = { ...response, items: response.items.map((i: any) => ({ ...i, showNotes: false, mouseOver: false })) };
      isLoading.value = false;
    })
    .catch((error) => {
      showAlertError(error);
      isLoading.value = false;
    });
}

function addBreak(index: number) {
  if (index === 0) {
    return true;
  }
  if (dayjs(data.value.items[index].weekStartWorking).format(momentFormat) === dayjs(data.value.items[index - 1].weekStartWorking).format(momentFormat)) {
    return false;
  }
  return true;
}

function workerColor(worker: any) {
  if (worker.workerRequestStatus === WorkerRequestStatus.Rejected) {
    return 'Rejected';
  } else if (worker.isSubcontractor) {
    return 'Blue';
  }
}

function showNotesClick(index: number) {
  if (!data.value.items[index].showNotes) {
    data.value.items[index].mouseOver = true;
  }
  data.value.items[index].showNotes = true;
}

function hideNotes(index: number) {
  if (data.value.items[index].showNotes) {
    data.value.items[index].mouseOver = false;
    data.value.items[index].showNotes = false;
  }
}
</script>
