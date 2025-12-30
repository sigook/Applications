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
                    @changePage="(index) => getAgencyWorkerProfileRequestHistory(index)">
        </pagination>
    </div>
</template>

<script>
    import toast from '../../mixins/toastMixin';
    export default {
        props: ['workerId'],
        mixins: [toast],
        data() {
            return {
                size: 10,
                currentPage: 1,
                data: null,
                isLoading: false
            }
        },
        methods: {
            getAgencyWorkerProfileRequestHistory(page){
                this.isLoading = true;
                this.$store.dispatch('agency/getAgencyWorkerProfileRequestHistory', {pagination: {size: this.size, page: page}, workerId: this.workerId})
                .then(response => {
                    this.data = response;
                    this.isLoading = false;
                })
                .catch(error => {
                    this.showAlertError(error);
                    this.isLoading = false;
                })
            }
        },
        created() {
            this.getAgencyWorkerProfileRequestHistory(this.currentPage)
        },
        components: {
            Pagination: () => import("../../components/Paginator"),
            ContainerRequest: () => import("../agency/AgencyWorkerRequestHistoryContainer")
        }
    }
</script>