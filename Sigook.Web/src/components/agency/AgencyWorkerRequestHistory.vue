<template>
    <div>
        <b-loading v-model="isLoading"></b-loading>
        <div v-if="data">
            <container-request
                    v-for="request in data.items"
                    v-bind:key="request.id"
                    :data="request">
            </container-request>
        </div>

        <pagination v-if="data"
                    :total-pages="data.totalPages"
                    :index-page="data.pageIndex"
                    :size-page="this.size"
                    @changePage="(index) => loadRequestHistory(index)">
        </pagination>
    </div>
</template>

<script lang="ts">
import { showAlertError } from "@/utils/toast";
    import { getAgencyWorkerProfileRequestHistory } from '@/api/agencyWorkerApi';
    export default {
        props: ['workerId'],
        data() {
            return {
                size: 10,
                currentPage: 1,
                data: null,
                isLoading: false
            }
        },
        methods: {
            loadRequestHistory(page){
                this.isLoading = true;
                getAgencyWorkerProfileRequestHistory(this.workerId, {size: this.size, page: page})
                .then(response => {
                    this.data = response;
                    this.isLoading = false;
                })
                .catch(error => {
                    showAlertError(error);
                    this.isLoading = false;
                })
            }
        },
        created() {
            this.loadRequestHistory(this.currentPage)
        },
        components: {
            Pagination: () => import("../../components/Paginator.vue"),
            ContainerRequest: () => import("../agency/AgencyWorkerRequestHistoryContainer.vue")
        }
    }
</script>