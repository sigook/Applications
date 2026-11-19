import { describe, expect, it } from 'vitest';
import RequestDetailTab from '@/shared/request-detail/RequestDetailTab.vue';
import { ComplianceDocumentTarget } from '@/shared/request-detail/requestApplicant';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestComplianceItem, RequestFact } from '@/shared/request-detail/types';

const fields: RequestFact[] = [
  { label: 'Position', value: 'Forklift operator' },
  { label: 'Rate', value: '$20.00' },
];

const complianceItems: RequestComplianceItem[] = [
  { id: 'c1', name: 'Safety boots', isMandatory: true, documentTarget: ComplianceDocumentTarget.None },
  { id: null, name: 'WHMIS', isMandatory: false, documentTarget: ComplianceDocumentTarget.None },
];

interface SetupOptions {
  description?: string;
  responsibilities?: string;
  requirements?: string;
  internalRequirements?: string;
  incentive?: number | null;
  incentiveDescription?: string;
  skills?: string[];
  showSkills?: boolean;
  complianceItems?: RequestComplianceItem[];
  slots?: Record<string, string>;
}

function setup({ slots, ...props }: SetupOptions = {}) {
  return mountWithPlugins(RequestDetailTab, { props: { fields, ...props }, slots });
}

function sections(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-index-link').map((l) => l.find('span').text());
}

function cardTitles(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.detail-card-title').map((t) => t.text());
}

describe('RequestDetailTab', () => {
  it('shows the job fields and empty states for a minimal request', () => {
    const wrapper = setup();

    expect(wrapper.findAll('.detail-field dt').map((d) => d.text())).toEqual(['Position', 'Rate']);
    expect(cardTitles(wrapper)).toEqual(['Job', 'Description', 'Requirements']);
    expect(wrapper.findAll('.detail-empty').map((e) => e.text())).toEqual(['No description', 'No requirements']);
    expect(sections(wrapper)).toEqual(['Job', 'Description', 'Requirements']);
  });

  it('shows every optional section when filled', () => {
    const wrapper = setup({
      description: '<p>Move pallets</p>',
      responsibilities: '<p>Load trucks</p>',
      requirements: '<p>Licence</p>',
      internalRequirements: '<p>Ask for Bob</p>',
      incentive: 50,
      incentiveDescription: 'per week',
      skills: ['Forklift', 'Bilingual'],
      complianceItems,
    });

    expect(cardTitles(wrapper)).toEqual([
      'Job', 'Skills', 'Description', 'Responsibilities', 'Requirements', 'Internal requirements', 'Incentive', 'Compliance checklist',
    ]);
    expect(sections(wrapper)).toEqual([
      'Job', 'Skills', 'Description', 'Responsibilities & requirements', 'Internal', 'Incentive', 'Compliance',
    ]);
    expect(wrapper.findAll('.detail-tag.is-outlined').map((t) => t.text())).toEqual(['Forklift', 'Bilingual']);
    expect(wrapper.find('#request-section-description .detail-rich-text').html()).toContain('<p>Move pallets</p>');
    expect(wrapper.find('.detail-index-hint').text()).toBe('agency only');
    expect(wrapper.find('#request-section-internal .detail-tag').text()).toBe('Agency only');
    expect(wrapper.find('#request-section-incentive').text()).toContain('$50.00 · per week');
    expect(wrapper.findAll('.request-compliance-item').map((i) => i.text())).toEqual(['Safety boots required', 'WHMIS optional']);
  });

  it('strips unsafe markup from the rich text', () => {
    const wrapper = setup({ description: '<p>Safe</p><img src="x" onerror="alert(1)"><script>alert(2)</script>' });
    const html = wrapper.find('#request-section-description .detail-rich-text').html();

    expect(html).toContain('<p>Safe</p>');
    expect(html).not.toContain('onerror');
    expect(html).not.toContain('<script');
  });

  it('shows the skills section when forced even without skills', () => {
    const wrapper = setup({ showSkills: true, slots: { skills: '<button class="add-skill">Add skill</button>' } });

    expect(cardTitles(wrapper)).toContain('Skills');
    expect(wrapper.find('.add-skill').exists()).toBe(true);
  });

  it('renders the action and rail slots', () => {
    const wrapper = setup({
      slots: {
        'job-actions': '<button class="edit-job">Edit</button>',
        'requirements-actions': '<button class="edit-requirements">Edit</button>',
        rail: '<div class="rail-content" />',
      },
    });

    expect(wrapper.find('#request-section-job .detail-card-actions .edit-job').exists()).toBe(true);
    expect(wrapper.find('.edit-requirements').exists()).toBe(true);
    expect(wrapper.find('.detail-layout-rail .rail-content').exists()).toBe(true);
  });
});
