import { defineStore } from 'pinia';
import mgr from '@/security/securityService';
import type { UserProfile } from '@/types/security';

interface SecurityState {
  userRoles: string[];
  user: UserProfile | null;
}

export const useSecurityStore = defineStore('security', {
  state: (): SecurityState => ({
    userRoles: [],
    user: null,
  }),
  actions: {
    setUser(data: UserProfile | null) {
      let roles: string[] = [];
      if (data) {
        if (typeof data.profile.role === 'string') {
          roles = [data.profile.role];
        } else {
          roles = data.profile.role as string[];
        }
      }
      this.userRoles = roles;
      this.user = data;
    },
    getUser(): Promise<UserProfile> {
      return new Promise((resolve, reject) => {
        if (this.user) {
          return resolve(this.user);
        }
        mgr.getUser()
          .then((user) => {
            const userProfile = user as unknown as UserProfile;
            this.setUser(userProfile);
            resolve(userProfile);
          })
          .catch((error: unknown) => {
            this.setUser(null);
            reject(error);
          });
      });
    },
    signIn(): void {
      mgr.signinRedirect().then();
    },
    async signOut(): Promise<void> {
      await mgr.removeUser();
      this.setUser(null);
      await mgr.signoutRedirect();
    },
    silentSignin(): Promise<void> {
      return new Promise((resolve, reject) => {
        mgr.signinSilent()
          .then((user) => {
            this.setUser(user as unknown as UserProfile);
            resolve();
          })
          .catch((error: unknown) => reject(error));
      });
    },
  },
  persist: {
    key: 'user',
  },
});
