import { useRouter, type RouteLocationRaw } from 'vue-router';

export function useGoBack(): { goBack: (fallback?: RouteLocationRaw | null) => void } {
  const router = useRouter();

  function goBack(fallback?: RouteLocationRaw | null) {
    if (window.history.state?.back) {
      router.back();
    } else if (fallback) {
      router.push(fallback);
    }
  }

  return { goBack };
}
