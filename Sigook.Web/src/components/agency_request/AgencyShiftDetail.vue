<template>
    <div class="d-inline-block relative v-top" @mouseleave="showDetail = false">
        <span>{{displayShift}}</span>
        <button v-if="displayShift" @click.stop="getRequestShift" class="no-border" :class="{'up': showDetail}">
            <img src="../../assets/images/arrow-down.svg" alt="button" type="button" width="10px" class="ml-2">
        </button>
        <shift-detail v-if="showDetail" :shift="shift" :isLoading="isLoading" />
    </div>
</template>

<script>
import toastMixin from "@/mixins/toastMixin";
export default {
    props: ['displayShift', 'requestId'],
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
        getRequestShift(){
            if (!this.showDetail){
                this.isLoading = true;
                this.showDetail = true;
                this.$store.dispatch('getRequestShift', this.requestId)
                        .then(response => {
                            this.isLoading = false;
                            this.shift = response;
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