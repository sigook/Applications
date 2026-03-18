import axios, { AxiosError } from 'axios';
import store from '../store';
import qs from 'qs';

const apiUrl: string = process.env.VUE_APP_URL_API as string;

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
  const user: any = await store.dispatch("getUser");
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
      await store.dispatch('silentSignin');
      const newUser: any = await store.dispatch('getUser');
      if (newUser) {
        originalRequest.headers.Authorization = `${newUser.token_type} ${newUser.access_token}`;
      }
      originalRequest.headers['accept-language'] = localStorage.getItem('language') && localStorage.getItem('language') !== 'en' ? localStorage.getItem('language') : 'en-US';
      return http(originalRequest);
    } catch (e) {
      await store.dispatch('signIn');
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
          return Promise.reject({ response: errorMsg });
        }
        break;
    }
  }

  return Promise.reject(error);
});

export default http;
