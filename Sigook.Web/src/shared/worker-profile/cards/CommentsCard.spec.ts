import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import CommentsCard from '@/shared/worker-profile/cards/CommentsCard.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerCommentList } from '@/shared/worker-profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/worker-profile/forms/DialogWorkerComment.vue', () => ({
  default: { name: 'DialogComment', emits: ['createComment'], template: '<div class="dialog-comment" />' },
}));

type CreateComment = (workerProfileId: string, model: { comment: string; rate: number }) => Promise<unknown>;

const comments: WorkerCommentList = {
  items: [
    { id: 'c1', comment: 'great worker', rate: 5, numberId: 1, createdAt: '2026-03-10T15:00:00' },
    { id: 'c2', comment: 'late once', rate: 3, numberId: 2, createdAt: '2026-04-02T15:00:00' },
  ],
  totalItems: 2,
};

interface SetupOptions {
  list?: WorkerCommentList;
  readonly?: boolean;
  title?: string;
  createComment?: CreateComment;
}

function setup({ list = comments, readonly, title, createComment }: SetupOptions = {}) {
  return mountWithPlugins(CommentsCard, {
    props: { workerProfileId: 'worker-profile-1', comments: list, pageIndex: 1, pageSize: 10, readonly, title, createComment },
  });
}

async function submitComment(wrapper: ReturnType<typeof setup>, value: { comment: string; rating: number }) {
  await wrapper.find('button').trigger('click');
  await nextTick();
  wrapper.findComponent({ name: 'DialogComment' }).vm.$emit('createComment', value);
}

describe('CommentsCard', () => {
  it('lists the comments with their date and rating', () => {
    const wrapper = setup();
    const rows = wrapper.findAll('.comment-row');

    expect(wrapper.find('.profile-card-title').text()).toBe('Comments & rating');
    expect(rows).toHaveLength(2);
    expect(rows[0].find('.comment-date').text()).toBe('10-Mar-2026');
    expect(rows[0].find('.comment-text').text()).toBe('great worker');
    expect(wrapper.findAllComponents({ name: 'BRate' })[1].props('modelValue')).toBe(3);
  });

  it('shows the empty state with a custom title', () => {
    const wrapper = setup({ list: { items: [], totalItems: 0 }, title: 'Feedback' });

    expect(wrapper.find('.profile-card-title').text()).toBe('Feedback');
    expect(wrapper.find('.profile-empty').text()).toBe('No comments yet');
  });

  it('paginates only when there are more comments than one page', async () => {
    expect(setup().findComponent({ name: 'BPagination' }).exists()).toBe(false);

    const wrapper = setup({ list: { items: comments.items, totalItems: 25 } });
    const pagination = wrapper.findComponent({ name: 'BPagination' });
    expect(pagination.props('total')).toBe(25);

    pagination.vm.$emit('change', 2);
    await nextTick();
    expect(wrapper.emitted('changePage')).toEqual([[2]]);
  });

  it('hides the add action when readonly', () => {
    const wrapper = setup({ readonly: true });

    expect(wrapper.find('button').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'BModal' }).exists()).toBe(false);
  });

  it('creates the comment and emits commentCreated', async () => {
    const createComment = vi.fn<CreateComment>().mockResolvedValue(undefined);
    const wrapper = setup({ createComment });
    await submitComment(wrapper, { comment: 'Good job', rating: 4 });

    await vi.waitFor(() => { expect(wrapper.emitted('commentCreated')).toHaveLength(1); });
    expect(createComment).toHaveBeenCalledExactlyOnceWith('worker-profile-1', { comment: 'Good job', rate: 4 });
    expect(wrapper.emitted('loading')).toEqual([[true]]);
  });

  it('requires both the comment and the rating', async () => {
    const createComment = vi.fn<CreateComment>();
    const wrapper = setup({ createComment });
    await submitComment(wrapper, { comment: 'Good job', rating: 0 });

    expect(showAlertError).toHaveBeenCalledWith('All fields are required');
    expect(createComment).not.toHaveBeenCalled();
  });

  it('stops loading and shows the error when saving fails', async () => {
    const error = new Error('save failed');
    const createComment = vi.fn<CreateComment>().mockRejectedValue(error);
    const wrapper = setup({ createComment });
    await submitComment(wrapper, { comment: 'Good job', rating: 4 });

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('loading')).toEqual([[true], [false]]);
    expect(wrapper.emitted('commentCreated')).toBeUndefined();
  });

  it('opens the dialog through the exposed open method', async () => {
    const wrapper = setup();
    expect(wrapper.findComponent({ name: 'DialogComment' }).exists()).toBe(false);

    (wrapper.vm as unknown as { open: () => void }).open();
    await nextTick();
    expect(wrapper.findComponent({ name: 'DialogComment' }).exists()).toBe(true);
  });
});
