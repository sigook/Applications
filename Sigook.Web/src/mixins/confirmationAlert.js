export default {
    data(){
        return {
            unsavedChanges: false
        }
    },
    beforeRouteLeave (to, from, next) {
        if(this.unsavedChanges){
            const answer = window.confirm('Do you really want to leave? you have unsaved changes!');
            if (answer) {
                next()
            } else {
                next(false)
            }
        }else {
            next()
        }
    }
}