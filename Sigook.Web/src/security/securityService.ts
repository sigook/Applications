import { UserManager, WebStorageStateStore, Log, User } from "oidc-client-ts";
import { fetchUserInfo, requestPasswordToken } from "@/api/authApi";

const redirectUrl = window.location.origin;
const securityServerUrl = import.meta.env.VUE_APP_SECURITY_SERVER;
const client = import.meta.env.VUE_APP_CLIENT;

if (import.meta.env.MODE !== "production") {
  Log.setLogger(console);
  Log.setLevel(Log.ERROR);
}

const mgr = new UserManager({
  authority: securityServerUrl,
  client_id: client,
  redirect_uri: redirectUrl + "/callback",
  response_type: "code",
  scope: "openid profile api1 roles offline_access",
  post_logout_redirect_uri: redirectUrl + "/",
  automaticSilentRenew: true,
  silent_redirect_uri: redirectUrl + "/silent-refresh",
  loadUserInfo: true,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
});

export async function signInWithPassword(email: string, password: string): Promise<void> {
  const tokens = await requestPasswordToken(email, password);
  const profile = await fetchUserInfo(tokens.token_type, tokens.access_token);
  const user = new User({
    access_token: tokens.access_token,
    refresh_token: tokens.refresh_token,
    token_type: tokens.token_type,
    scope: tokens.scope,
    expires_at: Math.floor(Date.now() / 1000) + tokens.expires_in,
    profile: profile as User["profile"],
  });
  await mgr.storeUser(user);
  await mgr.events.load(user);
}

export function signInWithMicrosoft(): Promise<void> {
  return mgr.signinRedirect({ acr_values: "idp:oidc" });
}

export default mgr;
