<template>
    <div class="pagination" v-if="totalPages > 1">
        <ul>
            <button class="btn-prev"
                    @click="changePage(currentPage - 1)"
                    :disabled="currentPage === 1" ></button>

            <li v-for="(page, index) in totalPages"
                :key="'pagination' + index"
                v-if="Math.abs(page - currentPage) < 3 || page === totalPages - 1 || page === 0"
                :class="[page === currentPage ? 'active' : '']">
                <span @click="changePage(page)">{{page}}</span>
            </li>

            <button class="btn-next"
                    @click="changePage(currentPage + 1)"
                    :disabled="totalPages === currentPage"></button>
        </ul>
    </div>
</template>

<script>
    export default {
        props: ['sizePage', 'indexPage', 'totalPages'],
        methods: {
            changePage(newPage){
                this.$emit("changePage", newPage);
                /*Scroll Top*/
                let containers = document.getElementsByClassName("scroll-top-on-pagination");
                for (let i = 0; i < containers.length; i++){
                    containers[i].scrollTop = 0
                }
                /*Body Top*/
                let bodyTop = document.getElementsByClassName("body-top-on-pagination");
                if (bodyTop.length > 0){
                    window.scrollTo(0, 0)
                }
            }
        },
        computed: {
            currentPage() {
                return this.indexPage;
            }
        }
    }
</script>