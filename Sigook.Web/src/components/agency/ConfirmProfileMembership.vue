<template>
    <div>
        <transition name="modal">
            <div class="vue-modal payment-support-modal">
                <div class="modal-mask">
                    <div class="modal-wrapper">
                        <div class="modal-container container modal-light">
                            <button @click="close(false)" class="cross-icon">{{$t('Close')}}</button>
                            <div class="bg-gray mBottom">Please confirm your subscription</div>
                            <table class="table" style="width: 100%">
                                <tr>
                                    <th>Membership Price</th>
                                    <td>{{costDetail.membershipPrice | currencyCad}}</td>
                                </tr>
                                <tr v-if="costDetail.credits > 0">
                                    <th>Credits</th>
                                    <td>{{costDetail.credits | currencyCad}}</td>
                                </tr>
                                <tr>
                                    <th>Subtotal</th>
                                    <td>{{costDetail.subTotal | currencyCad}}</td>
                                </tr>
                                <tr>
                                    <th>HST</th>
                                    <td>{{costDetail.hst | currencyCad}}</td>
                                </tr>
                                <tr>
                                    <th>Total</th>
                                    <th>{{costDetail.total | currencyCad}}</th>
                                </tr>
                            </table>
                            <hr/>
                            <div class="confirm-section">
                                <button class="background-btn md-btn orange-button" @click="confirmSubscription">Confirm and proceed with payment</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
    </div>
</template>

<script>
    export default {
        name: "ConfirmProfileMembership",
        props: ["costDetail"],
        methods:{
            close(confirm){
                this.$emit("close-modal", confirm);
            },
            confirmSubscription(){
                this.$emit("close-modal",true)
            }
        }
    }
</script>

<style lang="scss">
    .payment-support-modal {
        .modal-container .mBottom {
            margin-top: -15px;
            margin-left: -15px;
            margin-right: -15px;
            background: #505050;
            font-weight: bold;
            color: whitesmoke;
        }

        .modal-container.modal-light .cross-icon {
            background-color: transparent;
        }
    }
    .confirm-section{
        text-align: right;
    }
</style>