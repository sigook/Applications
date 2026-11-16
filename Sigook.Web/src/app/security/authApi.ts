import axios from "axios";
import qs from "qs";
import type {
  ConfirmEmailPayload,
  CreatePasswordPayload,
  ResetPasswordWithCodePayload,
  TokenResponse,
  UserInfoResponse,
} from "@/shared/types/security";

const authority = import.meta.env.VUE_APP_SECURITY_SERVER.replace(/\/+$/, "");
const clientId = import.meta.env.VUE_APP_CLIENT;
const scope = "openid profile api1 roles offline_access";

const authHttp = axios.create({ baseURL: authority });

export async function requestPasswordToken(email: string, password: string): Promise<TokenResponse> {
  const response = await authHttp.post<TokenResponse>(
    "/connect/token",
    qs.stringify({
      grant_type: "password",
      client_id: clientId,
      scope,
      username: email,
      password,
    }),
    { headers: { "Content-Type": "application/x-www-form-urlencoded" } },
  );
  return response.data;
}

export async function fetchUserInfo(tokenType: string, accessToken: string): Promise<UserInfoResponse> {
  const response = await authHttp.get<UserInfoResponse>("/connect/userinfo", {
    headers: { Authorization: `${tokenType} ${accessToken}` },
  });
  return response.data;
}

export async function requestPasswordResetCode(email: string): Promise<void> {
  await authHttp.post("/Password/forgot", { email });
}

export async function resetPasswordWithCode(payload: ResetPasswordWithCodePayload): Promise<void> {
  await authHttp.post("/Password/reset", payload);
}

export async function confirmEmail(payload: ConfirmEmailPayload): Promise<void> {
  await authHttp.post("/Account/ConfirmEmail", payload);
}

export async function createPassword(payload: CreatePasswordPayload): Promise<void> {
  await authHttp.post("/Account/CreatePassword", payload);
}

export async function resendConfirmationLink(email: string): Promise<void> {
  await authHttp.post("/Account/ResendConfirmationLink", undefined, { params: { userName: email } });
}
