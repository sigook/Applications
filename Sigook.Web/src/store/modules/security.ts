import mgr from "../../security/securityService";
import { UserProfile } from "../../types/security";

export interface SecurityState {
  userRoles: string[];
  user?: UserProfile | null;
}

const securityModule = {
  state: (): SecurityState => ({
    userRoles: [],
  }),
  mutations: {
    setUser(state: SecurityState, data: UserProfile | null) {
      let roles: string[] = [];
      if (data) {
        if (typeof (data.profile.role) === 'string') {
          roles = [data.profile.role];
        } else {
          roles = data.profile.role as string[];
        }
      }
      state.userRoles = roles;
      state.user = data;
    }
  },
  actions: {
    getUser(this: any, context: any): Promise<UserProfile> {
      return new Promise((resolve, reject) => {
        const user = this.state.security.user;
        if (user) {
          return resolve(user);
        } else {
          mgr.getUser()
            .then((user) => {
              const userProfile = user as unknown as UserProfile;
              context.commit("setUser", userProfile);
              return resolve(userProfile);
            })
            .catch((error: any) => {
              context.commit("setUser", null);
              return reject(error);
            });
        }
      });
    },
    signIn(): void {
      mgr.signinRedirect().then();
    },
    signOut(context: any): void {
      mgr.signoutRedirect().then(async () => {
        mgr.removeUser();
        context.commit("setUser", null)
      });
    },
    silentSignin(context: any): Promise<void> {
      return new Promise((resolve, reject) => {
        mgr.signinSilent()
          .then((user) => {
            context.commit('setUser', user as unknown as UserProfile);
            resolve();
          })
          .catch((error: any) => reject(error));
      });
    },
  },
};

export default securityModule;
