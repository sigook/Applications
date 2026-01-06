<template>
    <div class="container-toast" id="toast-modal" :class="customClass">
        <img :src="images[type]" alt="icon">
        <h2>
            {{title}}
        </h2>
        <p v-html="message"></p>

        <button class="close" @click="closeModal">x</button>

        <div class="action-buttons">
            <button class="background-btn md-btn blue-light-button btn-radius" @click="accept">{{ confirmButtonText }}</button>
            <button class="background-btn md-btn gray-light-button btn-radius" @click="closeModal">{{ cancelButtonText }}</button>
        </div>
    </div>
</template>


<script>
    export default {
        props: ['type', 'title', 'message', 'customClass', 'confirmButton', 'cancelButton', 'confirmButtonText', 'cancelButtonText'],
        data() {
            return {
                images: {
                    success: require('../assets/images/toast-success.png'),
                    error: require('../assets/images/toast-error.png'),
                    warning: require('../assets/images/toast-warning.png')
                }
            }
        },
        methods: {
            closeModal() {
                this.$emit('response', false);
                let toast = document.getElementById('toast-modal');
                document.getElementById('app').removeChild(toast);
            },
            accept(){
                this.$emit('response', true);
            }
        }
    }
</script>

<style lang="scss">
    .container-toast {
        position: fixed;
        right: 12px;
        bottom: auto;
        padding: 0.625em;
        box-shadow: 0 0 0.625em #d9d9d9;
        overflow-y: hidden;
        background: white;
        min-width: 335px;
        max-width: 340px;
        top: 15px;
        left: auto;
        border-radius: 4px;
        border-left: 5px solid gray;
        &:not(.big-modal){
            animation: swal2-show .5s;
        }
        @keyframes  swal2-show {
            0% {
                -webkit-transform: scale(0.7);
                transform: scale(0.7); }
            45% {
                -webkit-transform: scale(1.05);
                transform: scale(1.05); }
            80% {
                -webkit-transform: scale(0.95);
                transform: scale(0.95); }
            100% {
                -webkit-transform: scale(1);
                transform: scale(1); }
        }

        h2 {
            font-size: 16px;
            padding: 5px 10px;
            color: #595959;
            display: inline-block;
            vertical-align: middle;
            margin: 0;
        }
        img {
            display: inline-block;
            width: 20px;
            vertical-align: middle;
        }
        p {
            padding-left: 34px;
            padding-right: 10px;
            margin: 0;
            font-size: 16px;
            padding-bottom: 5px;
            font-weight: 400;
        }

        button.close {
            position: absolute;
            right: 18px;
            top: 15px;
            border: 0;
            text-transform: lowercase;
            color: #cccccc;
            font-size: 15px;
            font-family: inherit;
            &:hover {
                color: #f27473;
            }
        }



        &.toast-error{
            border-color: #f27473;
        }

        &.toast-warning{
            border-color: #f8bc86;
        }

        &.toast-success{
            border-color: #a5dd86;
        }

        .action-buttons {
            display: none;
        }


        /*modal grande con buttons*/

        &.big-modal {
            position: fixed;
            border-left: 0;
            min-width: 500px;
            border-radius: 0;
            left: 50%;
            right: auto;
            top: 50%;
            bottom: auto;
            transform: translate(-50%, -50%);
            padding: 20px 30px;
            z-index: 9;
            animation: swal2-big-show .8s;
            .action-buttons {
                text-align: center;
                display: block;
                button {
                    margin:10px  5px;
                    padding: 10px 20px;
                    font-size: 16px;
                }
            }

            img {
                display: block;
                width: 80px;
                margin: 14px auto;
            }

            h2 {
                text-align: center;
                display: block;
                font-size: 24px;
            }
            p {
                padding: 10px 0 20px;
                text-align: center;
                font-size: 16px;
            }
        }

        @keyframes swal2-big-show {
            0% {
                -webkit-transform: scale(0.7);
                transform: translate(-50%, -50%) scale(0.7); }
            45% {
                -webkit-transform: scale(1.05);
                transform: translate(-50%, -50%) scale(1.05); }
            80% {
                -webkit-transform: scale(0.95);
                transform: translate(-50%, -50%) scale(0.95); }
            100% {
                -webkit-transform: scale(1);
                transform: translate(-50%, -50%) scale(1); }
        }
    }

    body.alert-open #app:before {
        content: '';
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, 0.48);
        position: fixed;
        z-index: 6;
    }


    @media(max-width: 767px){
        .container-toast {
            width: calc(100% - 30px);
            min-width: 0 !important;
        }
    }
</style>