<template>
    <section class="mb-0">
        <b-loading v-model="isLoading"></b-loading>
        <div class="button-right">
            <h3>Punch Card Id</h3>
            <button type="button" class="outline-btn md-btn orange-button btn-radius" @click="getQRCode(worker.punchCardId)">Print</button>
        </div>
        <worker-qr :worker="worker" :qr-image="QrImage"></worker-qr>
    </section>
</template>
<script>
import qrCodeMixin from "../../mixins/qrCodeMixin"
export default {
    props: ["worker"],
    data() {
        return {
            isLoading: false
        }
    },
    mixins: [qrCodeMixin],
    components: {
        WorkerQr: () => import("../../components/WorkerPunchCardPrint")
    }
}
</script>