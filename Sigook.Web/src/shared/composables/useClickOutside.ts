import { watch, onScopeDispose, type Ref } from 'vue';

export function useClickOutside(
  root: Ref<HTMLElement | null>,
  active: Ref<boolean>,
  onOutside: () => void,
  ignore: Ref<HTMLElement | null>[] = [],
): void {
  function onDocumentClick(event: MouseEvent): void {
    const target = event.target as Node;
    if (root.value?.contains(target)) return;
    if (ignore.some((el) => el.value?.contains(target))) return;
    onOutside();
  }

  watch(active, (open) => {
    if (open) {
      document.addEventListener('click', onDocumentClick, true);
    } else {
      document.removeEventListener('click', onDocumentClick, true);
    }
  });

  onScopeDispose(() => document.removeEventListener('click', onDocumentClick, true));
}
