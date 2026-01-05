<template>
    <div>
        <b-loading v-model="isLoading"></b-loading>

        <h2 class="text-center main-title">Manage</h2>
        <div class="submenu-modal-tabs">
            <ul>
                <li @click="changeTab('candidates')"
                    :class="currentTab === 'candidates' ? 'active' : ''">
                    Candidates
                </li>
                <li @click="changeTab('workers')"
                    :class="currentTab === 'workers' ? 'active' : ''">
                    Workers
                </li>
            </ul>
        </div>

        <!-- Candidates -->
        <transition>
            <div v-if="currentTab === 'candidates'">
                <div class="scroll-worker-container scroll-top-on-pagination" v-if="candidates">
                    <div class="button-right pt-3 pb-2">
                        <searcher :placeholder="'Name'"
                                  class="ml-0 mr-0 mb-2 d-inline-flex"
                                  @filterResults="(value) => filterResultsCandidates(value)"
                                  ref="searcher">
                        </searcher>
                        <button class="ml-3 outline-btn md-btn orange-button btn-radius d-inline-block"
                                style="height: 32px; line-height: 1;"
                                @click="createCandidate = true"> + {{$t('Create')}}</button>
                    </div>

                    <div v-if="candidates" class="scroll-desktop">
                        <table class="bordered-cells table-candidates main-table sm-table-no-image">
                            <col width="6">
                            <col width="20%">
                            <col width="14%">
                            <col width="14%">
                            <col width="20%">
                            <col width="9%">
                            <col width="9%">
                            <col width="8%">
                            <thead>
                            <tr>
                                <td></td>
                                <td>Name</td>
                                <td>Email</td>
                                <td>Phone</td>
                                <td>Skills</td>
                                <td>Status</td>
                                <td>Recruiter</td>
                                <td></td>
                            </tr>
                            </thead>
                            <tbody>
                            <tr v-for="(candidate, index) in candidates.items"
                                :key="'candidatesContainer' + index">
                                <td>
                                    <div class="valign-middle">
                                        <div class="container-image-candidate img-30" v-if="candidate.profileImage">
                                            <img :src="candidate.profileImage"
                                                 alt="profile image" />
                                        </div>
                                        <default-image
                                                v-else
                                                :name="candidate.name" class="img-30"></default-image>
                                    </div>
                                </td>
                                <td>
                                    <div class="d-block">{{ candidate.name }}</div>
                                    <detail-address v-if="candidate.postalCode || candidate.address" :user="candidate"></detail-address>
                                </td>
                                <td>
                                    <div>
                                        <a :href="'mailto:' + candidate.email" class="ellipsis-150">{{ candidate.email }}</a>
                                    </div>
                                </td>
                                <td>
                                    <div class="capitalize is-inline-block v-middle pr-0" v-if="candidate.phoneNumbers.length > 0">{{ candidate.phoneNumbers[0].phoneNumber }}</div>
                                </td>
                                <td>
                                        <span :key="'skill' + index"
                                              v-for="(skill, index) in candidate.skills"
                                              class="tag-sm-gray mb-1 mr-1 ellipsis-full">
                                              {{ skill.skill }}
                                        </span>
                                </td>
                                <td><div>{{candidate.residencyStatus}}</div></td>
                                <td>
                                    <div class="capitalize is-inline-block v-middle pr-0" v-if="candidate.recruiter">{{ candidate.recruiter | emailName}}</div>
                                </td>
                                <td>
                                    <button class="fz-1 sm-btn outline-btn orange-button btn-radius button-col" @click="addCandidateToList(candidate)">Add</button>
                                </td>
                            </tr>
                            </tbody>
                        </table>
                    </div>
                    <pagination :total-pages="candidates.totalPages"
                                :index-page="candidates.pageIndex"
                                :size-page="this.size"
                                @changePage="(index) => getCandidates(index)" class="mt-3">
                    </pagination>
                </div>
            </div>
        </transition>

        <!-- Workers -->
        <transition>
            <div v-if="currentTab === 'workers'">
                <div class="scroll-worker-container scroll-top-on-pagination" v-if="workers">
                    <searcher :placeholder="'Name'"
                              @filterResults="(value) => filterResultsWorkers(value)"
                              ref="searcher"
                              class="mb-3 mt-3">
                    </searcher>
                    <table class="bordered-cells table-candidates main-table sm-table-no-image">
                        <col width="6%">
                        <col width="23%">
                        <col width="18%">
                        <col width="14%">
                        <col width="14%">
                        <col width="15%">
                        <col width="10%">
                        <thead>
                        <tr>
                            <td></td>
                            <td>Name</td>
                            <td>Phone</td>
                            <td>Social Insurance</td>
                            <td>Expire</td>
                            <td>Status</td>
                            <td></td>
                        </tr>
                        </thead>
                        <tbody>
                        <tr v-for="(worker, index) in workers.items"
                            :key="'workerContainer' + index">
                            <td>
                                <div class="valign-middle">
                                    <router-link :to="'/agency-workers/worker/' + worker.workerProfileId">
                                        <div class="container-image-candidate img-30" v-if="worker.profileImage">
                                            <img :src="worker.profileImage"
                                                 alt="profile image" class="img-30 img-rounded" />
                                        </div>
                                        <default-image
                                                v-else
                                                :name="worker.name" class="img-30"></default-image>
                                    </router-link>
                                </div>
                            </td>
                            <td>
                                <div class="d-block">{{ worker.name }}</div>
                                <detail-address v-if="worker.address" :user="worker"></detail-address>
                            </td>
                            <td>{{ worker.mobileNumber }}</td>
                            <td>{{ worker.socialInsurance }}</td>
                            <td>{{worker.dueDate | dateMonth}}</td>
                            <td>
                                    <span class="fz-1 fw-700 uppercase" :class="worker.status" v-if="worker.status !== $statusDecline">
                                        {{ $t(worker.status) }}
                                    </span>
                            </td>
                            <td>
                                <div v-if="!worker.approvedToWork" class="w-100">
                                    <button class="color-gray-light fz-1 sm-btn btn-no-border button-col">Not Approved</button>
                                </div>
                                <button v-else class="fz-1 sm-btn outline-btn orange-button btn-radius button-col" @click="addWorkerToList(worker)">Add</button>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </div>
                <pagination v-if="workers" :total-pages="workers.totalPages"
                            :index-page="workers.pageIndex"
                            :size-page="this.size"
                            @changePage="(index) => getWorkers(index)"
                            class="mt-3">
                </pagination>
            </div>
        </transition>

        <!-- custom modal TextArea-->
        <transition name="modal">
            <div v-if="modalMessage" class="vue-modal header-fixed">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-container small-container modal-light overflow-initial border-radius">
                            <button @click="modalMessage = false" type="button" class="cross-icon">close</button>
                            <edit-textarea :title="'Comments'"
                                           subtitle="Comments"
                                           :min-length="0"
                                           class="sm-edit-textarea"
                                           @updateContent="(data) => addUserToList(data)" />
                        </div>
                    </div>
                </div>
            </div>
        </transition>
        <!-- end custom modal TextArea-->
         <b-modal v-model="createCandidate" width="500px">
            <create-candidate @onClose="getCandidates(1)"></create-candidate>
        </b-modal>
    </div>
</template>
<script>
export default {
    data() {
        return {
            isLoading: false,
            requestId: this.$route.params.id,
            currentTab: 'candidates',
            workers: null,
            candidates: null,
            currentPage: 1,
            size: 30,
            modalMessage: false,
            currentUser:null,
            model: {
                workerProfileId: null,
                candidateId: null,
                comments: null
            },
            workersFilter: '',
            candidatesFilter: '',
            createCandidate: false
        }
    },
    components: {
        Pagination: () => import("../../components/Paginator"),
        EditTextarea: () => import("../../components/agency_request/EditTextarea"),
        Searcher: () => import("../../components/Searcher"),
        DetailAddress: () => import("../../components/candidate/DetailAddress"),
        CreateCandidate: () => import("../candidate/CreateCandidate")
    },
    methods: {
        filterResultsWorkers(data){
            this.workersFilter = data;
            this.getWorkers(this.currentPage)
        },
        getWorkers(index) {
            this.isLoading = true;
            this.$store.dispatch('agency/getAgencyRequestWorkersAll', {
                requestId: this.requestId,
                pagination: {page: index, size: this.size},
                filter: this.workersFilter
            })
                    .then(response => {
                        this.workers = response;
                        this.isLoading = false;
                    })
                    .catch(error => {
                        this.isLoading = false;
                        this.showAlertError(error.data);
                    })
        },
        filterResultsCandidates(data){
            this.candidatesFilter = data;
            this.getCandidates(this.currentPage)
        },
        getCandidates(index) {
            this.createCandidate = false;
            this.isLoading = true;
            this.$store.dispatch('agency/getAgencyCandidates',
                    {
                        pagination: {page: index, size: this.size},
                        filter: this.candidatesFilter
                    })
                    .then(response => {
                        this.isLoading = false;
                        this.candidates = response;
                    })
                    .catch(error => {
                        this.isLoading = false;
                        this.showAlertError(error)
                    })
        },
        changeTab(tab){
            this.currentTab = tab
            this.workersFilter = ''
            this.candidatesFilter = ''
            if (tab === 'candidates'){
                this.getCandidates(this.currentPage);
            } else {
                this.getWorkers(this.currentPage);
            }
        },
        addCandidateToList(item){
            this.model = {
                candidateId: item.id,
                workerProfileId: null
            }
            this.currentUser = item;
            this.modalMessage = true;
        },
        addWorkerToList(item){
            this.model = {
                workerProfileId: item.workerProfileId,
                candidateId: null
            }
            this.currentUser = item;
            this.modalMessage = true;
        },
        addUserToList(comment){
            this.model.comments = comment;
            this.$emit('updateApplicants', {model:this.model, item: this.currentUser})
        }
    },
    created() {
        this.getCandidates(this.currentPage);
    }
}
</script>