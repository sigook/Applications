import Oidc from "oidc-client";

const redirectUrl = window.location.origin;
const securityServerUrl = import.meta.env.VUE_APP_SECURITY_SERVER;
const client = import.meta.env.VUE_APP_CLIENT;

if (process.env.NODE_ENV !== "production") {
  Oidc.Log.logger = console;
  Oidc.Log.level = Oidc.Log.ERROR;
}

const mgr = new Oidc.UserManager({
  authority: securityServerUrl,
  client_id: client,
  redirect_uri: redirectUrl + "/callback",
  response_type: "id_token token",
  scope: "openid profile api1 roles",
  post_logout_redirect_uri: redirectUrl + "/callback",
  automaticSilentRenew: true,
  silent_redirect_uri: redirectUrl + "/silent-refresh",
});

export default mgr;
