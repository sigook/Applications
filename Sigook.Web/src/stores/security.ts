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
    async getUser(): Promise<UserProfile | null> {
      try {
        const current = await mgr.getUser();
        if (!current || current.expired) {
          if (this.user) this.setUser(null);
          return null;
        }
        const userProfile = current as unknown as UserProfile;
        if (this.user?.access_token !== userProfile.access_token) {
          this.setUser(userProfile);
        }
        return userProfile;
      } catch (error) {
        this.setUser(null);
        throw error;
      }
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
});
