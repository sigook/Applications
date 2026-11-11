import { describe, it, expect } from 'vitest';
import { defineComponent, nextTick, ref } from 'vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import {
  buildJobPosting,
  jobPostingUrl,
  parseJobLocation,
  parseSalaryAmount,
  useJobPostingSchema,
} from '@/modules/landing/composables/useJobPostingSchema';
import type { JobViewModel } from '@/modules/landing/types';

function makeJob(overrides: Partial<JobViewModel> = {}): JobViewModel {
  return {
    id: '00000000-0000-0000-0000-000000000000',
    requestId: 'b5d1c2a4-0000-0000-0000-000000000001',
    numberId: '2418',
    title: 'CNC Operator ',
    salary: '$25.00',
    location: 'WOODBRIDGE ONTARIO',
    type: 'Temporary',
    createdAt: '2026-07-29T17:46:15.734128',
    description: '<p>Operate CNC machines.</p>',
    requirements: '<ul><li>3 years</li></ul>',
    responsibilities: '<ul><li>Daily startup</li></ul>',
    shift: 'Mon-Fri 08:00 to 16:30',
    createdBy: '',
    agencyId: null,
    ...overrides,
  };
}

function readSchema(): Record<string, unknown> | null {
  const script = document.getElementById('job-posting-ld');
  return script?.textContent ? (JSON.parse(script.textContent) as Record<string, unknown>) : null;
}

describe('parseJobLocation', () => {
  it('splits a Canadian city and province', () => {
    expect(parseJobLocation('WOODBRIDGE ONTARIO')).toEqual({ locality: 'Woodbridge', region: 'ON', country: 'CA' });
  });

  it('matches multi-word states before shorter ones', () => {
    expect(parseJobLocation('NEW YORK NEW YORK')).toEqual({ locality: 'New York', region: 'NY', country: 'US' });
    expect(parseJobLocation('Charleston, West Virginia')).toEqual({ locality: 'Charleston', region: 'WV', country: 'US' });
  });

  it('uses the region name as locality when only the region is given', () => {
    expect(parseJobLocation('FLORIDA')).toEqual({ locality: 'Florida', region: 'FL', country: 'US' });
  });

  it('keeps the raw text when no region is recognised', () => {
    expect(parseJobLocation('REMOTE')).toEqual({ locality: 'Remote', region: null, country: null });
  });
});

describe('parseSalaryAmount', () => {
  it('parses currency strings', () => {
    expect(parseSalaryAmount('$54,000.00')).toBe(54000);
    expect(parseSalaryAmount('$17.50')).toBe(17.5);
  });

  it('ignores placeholder and empty amounts', () => {
    expect(parseSalaryAmount('$0.00')).toBeNull();
    expect(parseSalaryAmount('$1.00')).toBeNull();
    expect(parseSalaryAmount('')).toBeNull();
  });
});

describe('buildJobPosting', () => {
  it('maps a Canadian hourly temporary job', () => {
    const schema = buildJobPosting(makeJob());

    expect(schema).toMatchObject({
      '@type': 'JobPosting',
      title: 'CNC Operator',
      datePosted: '2026-07-29',
      url: jobPostingUrl('2418'),
      employmentType: 'TEMPORARY',
      identifier: { value: '2418' },
      jobLocation: { address: { addressLocality: 'Woodbridge', addressRegion: 'ON', addressCountry: 'CA' } },
      baseSalary: { currency: 'CAD', value: { value: 25, unitText: 'HOUR' } },
    });
    expect(schema.description).toBe(
      '<p>Operate CNC machines.</p><h3>Responsibilities</h3><ul><li>Daily startup</li></ul><h3>Requirements</h3><ul><li>3 years</li></ul><p>Shift: Mon-Fri 08:00 to 16:30</p>',
    );
  });

  it('maps a US yearly full time job', () => {
    const schema = buildJobPosting(makeJob({ salary: '$54,000.00', location: 'MEDLEY FLORIDA', type: 'Full Time' }));

    expect(schema.employmentType).toBe('FULL_TIME');
    expect(schema.baseSalary).toEqual({
      '@type': 'MonetaryAmount',
      currency: 'USD',
      value: { '@type': 'QuantitativeValue', value: 54000, unitText: 'YEAR' },
    });
  });

  it('omits salary, region and employment type when they cannot be resolved', () => {
    const schema = buildJobPosting(makeJob({ location: 'REMOTE', type: 'Seasonal', salary: '$0.00' }));

    expect(schema.baseSalary).toBeUndefined();
    expect(schema.employmentType).toBeUndefined();
    expect(schema.jobLocation.address).toEqual({ '@type': 'PostalAddress', addressLocality: 'Remote' });
  });

  it('falls back to the title when the job has no description', () => {
    const schema = buildJobPosting(makeJob({ description: '', requirements: '', responsibilities: '', shift: '' }));

    expect(schema.description).toBe('<p>CNC Operator</p>');
  });
});

describe('useJobPostingSchema', () => {
  function mountWithJob(initial: JobViewModel | undefined) {
    const job = ref(initial);
    const Host = defineComponent({
      setup() {
        useJobPostingSchema(() => job.value);
        return () => null;
      },
    });
    return { job, wrapper: mountWithPlugins(Host) };
  }

  it('injects the JobPosting script and job meta for the selected job', async () => {
    document.head.innerHTML = '<meta name="description" content=""><meta property="og:title" content="">';
    mountWithJob(makeJob({ description: '<p>Operate <b>CNC</b> machines.</p>' }));
    await nextTick();

    expect(readSchema()).toMatchObject({ '@type': 'JobPosting', title: 'CNC Operator' });
    expect(document.title).toBe('CNC Operator - Woodbridge, ON | Sigook');
    expect(document.querySelector('meta[name="description"]')?.getAttribute('content')).toBe('Operate CNC machines.');
    expect(document.querySelector('meta[property="og:title"]')?.getAttribute('content')).toBe('CNC Operator - Woodbridge, ON | Sigook');
  });

  it('truncates long descriptions for the meta tag', async () => {
    document.head.innerHTML = '<meta name="description" content="">';
    mountWithJob(makeJob({ description: `<p>${'word '.repeat(60)}</p>` }));
    await nextTick();

    const content = document.querySelector('meta[name="description"]')?.getAttribute('content') ?? '';
    expect(content.length).toBeLessThanOrEqual(155);
    expect(content.endsWith('…')).toBe(true);
  });

  it('escapes markup that could close the script tag', async () => {
    mountWithJob(makeJob({ description: '<p>x</p></script><script>alert(1)</script>' }));
    await nextTick();

    expect(document.getElementById('job-posting-ld')?.textContent).not.toContain('</script>');
    expect(readSchema()?.description).toContain('</script>');
  });

  it('removes the script when no job is selected and on unmount', async () => {
    const { job, wrapper } = mountWithJob(makeJob());
    await nextTick();
    expect(readSchema()).not.toBeNull();

    job.value = undefined;
    await nextTick();
    expect(readSchema()).toBeNull();

    job.value = makeJob({ numberId: '9' });
    await nextTick();
    expect(readSchema()).toMatchObject({ identifier: { value: '9' } });

    wrapper.unmount();
    expect(readSchema()).toBeNull();
  });

  it('does nothing when there is no job', async () => {
    mountWithJob(undefined);
    await nextTick();

    expect(readSchema()).toBeNull();
  });
});
