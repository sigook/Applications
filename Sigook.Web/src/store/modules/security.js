import mgr from "../../security/securityService";
import http from "../../security/apiService";

export default {
  state: {
    userRoles: [],
  },
  mutations: {
    setUser(state, data) {
      let roles = [];
      if (data) {
        if (typeof (data.profile.role) === 'string') {
          roles = [data.profile.role];
        } else {
          roles = data.profile.role;
        }
      }
      state.userRoles = roles;
      state.user = data;
    }
  },
  actions: {
    getUser(context) {
      return new Promise((resolve, reject) => {
        const user = this.state.security.user;
        if (user) {
          return resolve(user);
        } else {
          mgr.getUser()
            .then((user) => {
              context.commit("setUser", user);
              return resolve(user);
            })
            .catch((error) => {
              context.commit("setUser", null);
              return reject(error);
            });
        }
      });
    },
    signIn() {
      mgr.signinRedirect().then();
    },
    signOut(context) {
      mgr.signoutRedirect().then(async () => {
        mgr.removeUser();
        context.commit("setUser", null)
      });
    },
    silentSignin(context) {
      return new Promise((resolve, reject) => {
        mgr.signinSilent()
          .then((user) => {
            context.commit('setUser', user);
            resolve();
          })
          .catch((error) => reject(error));
      });
    },
    changeEmail(context, model) {
      return new Promise((resolve, reject) => {
        http.post("/api/Account/ChangeEmail", model)
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    },
    getEmail() {
      return new Promise((resolve, reject) => {
        http.get("/api/Account/GetEmail")
          .then((response) => resolve(response.data))
          .catch((error) => reject(error.response));
      });
    }
  },
};
