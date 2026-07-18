import { defineStore } from 'pinia';
import mgr from '@/security/securityService';
import type { UserProfile } from '@/types/security';

interface SecurityState {
  userRoles: string[];
  user: UserProfile | null;
  isReady: boolean;
}

let silentRenewPromise: Promise<UserProfile | null> | null = null;

export const useSecurityStore = defineStore('security', {
  state: (): SecurityState => ({
    userRoles: [],
    user: null,
    isReady: false,
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
        if (!current) {
          if (this.user) this.setUser(null);
          return null;
        }
        if (current.expired) {
          if (!current.refresh_token) {
            if (this.user) this.setUser(null);
            return null;
          }
          return await this.silentSignin().catch(async () => {
            await mgr.removeUser();
            this.setUser(null);
            return null;
          });
        }
        const userProfile = current as unknown as UserProfile;
        if (this.user?.access_token !== userProfile.access_token) {
          this.setUser(userProfile);
        }
        return userProfile;
      } catch (error) {
        this.setUser(null);
        throw error;
      } finally {
        this.isReady = true;
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
    silentSignin(): Promise<UserProfile | null> {
      if (!silentRenewPromise) {
        silentRenewPromise = mgr.signinSilent()
          .then((user) => {
            const userProfile = user as unknown as UserProfile | null;
            this.setUser(userProfile);
            return userProfile;
          })
          .finally(() => {
            silentRenewPromise = null;
          });
      }
      return silentRenewPromise;
    },
  },
});
