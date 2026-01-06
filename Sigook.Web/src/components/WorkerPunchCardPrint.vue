<template>
    <section id="qr-card" style="display: none">
        <!-- Print this -->
        <section>
            <h2>{{worker.firstName}} {{worker.lastName}}</h2>
            <img v-if="QrImage" :src="QrImage" alt="QR Worker Id">
            <p>
                Put this <strong>QR CODE</strong> in front of the Sigook scanner to clock in and clock out
            </p>
        </section>
    </section>
</template>
<script>
export default {
    props: ['worker', 'QrImage']
}
</script>