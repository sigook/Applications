<template>
    <div>
        <div class="profile-worker">
            <skills
                    id="skills"
                    :class="{'missing': worker.skills.length === 0}"
                    :worker="worker" @updateProfile="() => updateProfile()" />
            <section id="languages">
                <languages :worker="worker" @updateProfile="() => updateProfile()" />
            </section>

        </div>
    </div>
</template>

<script>
    import toastMixin from "../../mixins/toastMixin";

    export default {
        props: ['worker'],
        mixins: [
            toastMixin
        ],
        components: {
            languages: () => import("../../components/worker/WorkLanguagesDetail"),
            skills: () => import("../../components/worker/WorkSkillsDetail")
        },
        methods: {
            updateProfile(){
                this.isLoading = true;
                this.$store.dispatch('worker/getProfile', this.worker.id)
                    .then(() => {
                        this.isLoading = false;
                    })
                    .catch(error => {
                        this.isLoading = false;
                        this.showAlertError(error);
                    })
            }
        }
    }
</script>


