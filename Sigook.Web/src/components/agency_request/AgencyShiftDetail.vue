<template>
    <div class="d-inline-block relative v-top" @mouseleave="showDetail = false">
        <span>{{displayShift}}</span>
        <button v-if="displayShift" @click.stop="getRequestShift" class="no-border" :class="{'up': showDetail}">
            <img src="../../assets/images/arrow-down.svg" alt="button" type="button" width="10px" class="ml-2">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" v-model:is-loading="isLoading" />
    </div>
</template>

<script lang="ts">
import { defineAsyncComponent } from 'vue';
import { showAlertError } from "@/utils/toast";
import { fetchRequestShift } from "@/api/requestApi";
export default {
    props: ['displayShift', 'requestId'],
    data() {
        return {
            shift: null,
            showDetail: false,
            isLoading: false
        }
    },
    components: {
        ShiftDetail: defineAsyncComponent(() => import("../request/ShiftDetail.vue"))
    },
    methods: {
        getRequestShift(){
            if (!this.showDetail){
                this.isLoading = true;
                this.showDetail = true;
                fetchRequestShift(this.requestId)
                        .then(response => {
                            this.isLoading = false;
                            this.shift = response;
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