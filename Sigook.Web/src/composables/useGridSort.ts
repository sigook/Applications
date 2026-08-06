import { computed, isRef, type ComputedRef, type Ref } from 'vue';

export interface SortableParams {
  sortBy: number;
  isDescending: boolean;
}

export interface UseGridSort {
  defaultSort: ComputedRef<[string, string]>;
  onSortChange: (field: string, order: string) => void;
}

export function useGridSort<T extends SortableParams>(
  params: T | Ref<T>,
  fields: Record<string, number>,
  reload: () => void,
): UseGridSort {
  const current = (): T => (isRef(params) ? params.value : params);

  const defaultSort = computed<[string, string]>(() => [
    Object.keys(fields).find((field) => fields[field] === current().sortBy) ?? '',
    current().isDescending ? 'desc' : 'asc',
  ]);

  function onSortChange(field: string, order: string): void {
    if (field in fields) {
      current().sortBy = fields[field];
    }
    current().isDescending = order !== 'asc';
    reload();
  }

  return { defaultSort, onSortChange };
}
