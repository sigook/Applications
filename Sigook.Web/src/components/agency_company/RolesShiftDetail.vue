<template>
    <div class="d-inline-block relative v-top" @mouseleave="showDetail = false">
        <span>{{displayShift}}</span>
        <button v-if="displayShift" @click="getShift" class="no-border pl-4 pr-3" :class="{'up': showDetail}">
            <img src="../../assets/images/arrow-down.svg" alt="button" type="button" width="10px">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" :isLoading="isLoading" />
    </div>
</template>

<script>
import toastMixin from "@/mixins/toastMixin";
export default {
    props: ['displayShift', 'roleId', 'companyId'],
    data() {
        return {
            shift: null,
            showDetail: false,
            isLoading: false
        }
    },
    mixins: [toastMixin],
    components: {
        ShiftDetail: () => import("../request/ShiftDetail")
    },
    methods: {
        getShift(){
            if (!this.showDetail){
                this.isLoading = true;
                this.showDetail = true;
                this.$store.dispatch('agency/getAgencyCompanyJobPositionById', {profileId: this.companyId, id: this.roleId})
                        .then(response => {
                            this.isLoading = false;
                            this.shift = response.shift;
                        })
                        .catch(error => {
                            this.isLoading = false;
                            this.showAlertError(error);
                        })
            } else {
                this.showDetail = false;
            }
        }
    }
}
</script>