import axios from "axios";
import qs from "qs";
import type { ResetPasswordWithCodePayload, TokenResponse, UserInfoResponse } from "@/types/security";

const authority = import.meta.env.VUE_APP_SECURITY_SERVER.replace(/\/+$/, "");
const clientId = import.meta.env.VUE_APP_CLIENT;
const scope = "openid profile api1 roles offline_access";

const authHttp = axios.create({ baseURL: authority });

export function requestPasswordToken(email: string, password: string): Promise<TokenResponse> {
  return authHttp
    .post<TokenResponse>(
      "/connect/token",
      qs.stringify({
        grant_type: "password",
        client_id: clientId,
        scope,
        username: email,
        password,
      }),
      { headers: { "Content-Type": "application/x-www-form-urlencoded" } },
    )
    .then((r) => r.data);
}

export function fetchUserInfo(tokenType: string, accessToken: string): Promise<UserInfoResponse> {
  return authHttp
    .get<UserInfoResponse>("/connect/userinfo", {
      headers: { Authorization: `${tokenType} ${accessToken}` },
    })
    .then((r) => r.data);
}

export function requestPasswordResetCode(email: string): Promise<void> {
  return authHttp.post("/Password/forgot", { email }).then(() => undefined);
}

export function resetPasswordWithCode(payload: ResetPasswordWithCodePayload): Promise<void> {
  return authHttp.post("/Password/reset", payload).then(() => undefined);
}

export function resendConfirmationLink(email: string): Promise<void> {
  return authHttp
    .post("/Account/ResendConfirmationLink", undefined, { params: { userName: email } })
    .then(() => undefined);
}
