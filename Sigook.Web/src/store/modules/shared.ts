import http from "../../security/apiService";

const sharedModule = {
    namespaced: true,
    actions: {
        unsubscribe(_context: any, model: any): Promise<any> {
            return new Promise((resolve, reject) => {
                http.post("/api/EmailPreferences/Unsubscribe", model)
                    .then((response: any) => resolve(response.data))
                    .catch((error: any) => reject(error.response));
            });
        },
    }
} as const;

export default sharedModule;
