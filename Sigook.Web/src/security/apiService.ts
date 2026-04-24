import axios, { AxiosError } from 'axios';
import pinia from '@/stores';
import { useSecurityStore } from '@/stores/security';
import qs from 'qs';

const apiUrl: string = import.meta.env.VUE_APP_URL_API as string;

const http = axios.create({
  baseURL: apiUrl,
  timeout: 0,
  headers: {
    'Content-Type': 'application/json',
    'accept-language': localStorage.getItem('language') && localStorage.getItem('language') !== 'en' ? localStorage.getItem('language') : 'en-US'
  },
  paramsSerializer: (params: any) => qs.stringify(params, { encodeValuesOnly: true })
});

http.interceptors.request.use(async (config: any) => {
  const securityStore = useSecurityStore(pinia);
  const user: any = await securityStore.getUser();
  if (user) {
    config.headers.Authorization = `${user.token_type} ${user.access_token}`;
  }
  config.headers['accept-language'] = localStorage.getItem('language') && localStorage.getItem('language') !== 'en' ? localStorage.getItem('language') : 'en-US';
  return config;
}, function (error: AxiosError) {
  return Promise.reject(error);
});

// Response interceptor
http.interceptors.response.use(response => response, async (error: AxiosError) => {
  const originalRequest: any = error.config;

  if (error.response?.status === 401 && !originalRequest._retry) {
    originalRequest._retry = true;

    try {
      const securityStore = useSecurityStore(pinia);
      await securityStore.silentSignin();
      const newUser: any = await securityStore.getUser();
      if (newUser) {
        originalRequest.headers.Authorization = `${newUser.token_type} ${newUser.access_token}`;
      }
      originalRequest.headers['accept-language'] = localStorage.getItem('language') && localStorage.getItem('language') !== 'en' ? localStorage.getItem('language') : 'en-US';
      return http(originalRequest);
    } catch (e) {
      const securityStore = useSecurityStore(pinia);
      securityStore.signIn();
      return Promise.reject(e);
    }
  } else {
    switch (error.response?.status) {
      case 403:
        alert('You are not authorized to perform this action');
        break;
      case 500:
        alert('Oops something was wrong please try again later');
        if ((error.response?.config as any)?.responseType === 'blob') {
          const errorMsg = "An error occurred please try again, if the error persists try later one of our engineers will solve it soon";
          return Promise.reject({ data: errorMsg });
        }
        break;
    }
  }

  return Promise.reject(error.response ?? { data: error.message });
});

export default http;
