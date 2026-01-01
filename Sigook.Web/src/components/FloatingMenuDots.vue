<template>
    <div class="floating-menu-container">
        <!--<button class="floating-menu-button"
                @click.stop="onOpenMenu()">-->
        <button class="floating-menu-button">
            <img src="../assets/images/menu-dots.svg" alt="Menu">
        </button>
        <transition name="fade">
            <div class="floating-menu">
                <div class="floating-menu-nav">
                    <header>
                        Options
                        <button class="close-button-light" @click="onCloseMenu()">
                            <img src="../assets/images/delete-x.png" alt="close">
                        </button>
                    </header>
                    <slot name="options"></slot>
                    <!-- TO COPY IN SLOT OPTIONS-->
                    <!--
                    <button class="floating-menu-item">
                        <span> ITEM WITH BUTTON</span>
                    </button>
                    -->
                </div>
            </div>
        </transition>
    </div>
</template>
<script>
export default {
    data (){
        return {
            showMenuTop: false
        }
    },
    methods: {
        onOpenMenu(){
            if (this.showMenuTop) {
                this.showMenuTop = false;
            } else {
                this.showMenuTop = true;
                setTimeout(() => {
                    document.documentElement.addEventListener('click', this.onClickOutside, false)
                }, 200)

            }
        },
        onClickOutside(event){
            if (event){
                this.onCloseMenu()
            }
        },
        onCloseMenu(){
            this.showMenuTop = false;
            document.documentElement.removeEventListener('click', this.onClickOutside, false)
        }
    }
}
</script>