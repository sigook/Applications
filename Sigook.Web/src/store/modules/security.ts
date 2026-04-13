import type { ActionContext, Store } from "vuex";
import mgr from "../../security/securityService";
import { UserProfile } from "../../types/security";
import type { RootState } from "@/store";

export interface SecurityState {
  userRoles: string[];
  user?: UserProfile | null;
}

type SecurityActionContext = ActionContext<SecurityState, RootState>;

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
    getUser(this: Store<RootState>, context: SecurityActionContext): Promise<UserProfile> {
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
            .catch((error: unknown) => {
              context.commit("setUser", null);
              return reject(error);
            });
        }
      });
    },
    signIn(): void {
      mgr.signinRedirect().then();
    },
    signOut(context: SecurityActionContext): void {
      mgr.signoutRedirect().then(async () => {
        mgr.removeUser();
        context.commit("setUser", null);
      });
    },
    silentSignin(context: SecurityActionContext): Promise<void> {
      return new Promise((resolve, reject) => {
        mgr.signinSilent()
          .then((user) => {
            context.commit('setUser', user as unknown as UserProfile);
            resolve();
          })
          .catch((error: unknown) => reject(error));
      });
    },
  },
};

export default securityModule;
