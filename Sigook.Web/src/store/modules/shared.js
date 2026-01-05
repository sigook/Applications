import http from "../../security/apiService";

export default  {
    namespaced: true,
    actions:{
        unsubscribe(context, model){
            return new Promise((resolve, reject) => {
                http.post("/api/EmailPreferences/Unsubscribe", model)
                    .then(response => resolve(response.data))
                    .catch(error => reject(error.response));
            });
        },
    }
}