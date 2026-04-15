<template>
    <div class="d-inline-block relative v-top" @mouseleave="showDetail = false">
        <span>{{displayShift}}</span>
        <button v-if="displayShift" @click="getShift" class="no-border pl-4 pr-3" :class="{'up': showDetail}">
            <img src="../../assets/images/arrow-down.svg" alt="button" type="button" width="10px">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" :is-loading.sync="isLoading" />
    </div>
</template>

<script lang="ts">
import { showAlertError } from "@/utils/toast";
import { getAgencyCompanyJobPositionById } from "@/api/agencyCompanyApi";
export default {
    props: ['displayShift', 'roleId', 'companyId'],
    data() {
        return {
            shift: null,
            showDetail: false,
            isLoading: false
        }
    },
    components: {
        ShiftDetail: () => import("../request/ShiftDetail.vue")
    },
    methods: {
        getShift(){
            if (!this.showDetail){
                this.isLoading = true;
                this.showDetail = true;
                getAgencyCompanyJobPositionById(this.companyId, this.roleId)
                        .then(response => {
                            this.isLoading = false;
                            this.shift = response.shift;
                        })
                        .catch(error => {
                            this.isLoading = false;
                            showAlertError(error);
                        })
            } else {
                this.showDetail = false;
            }
        }
    }
}
</script>