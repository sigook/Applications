import { beforeEach, describe, expect, it, vi } from 'vitest';
import { downloadFile, downloadPDF, openFileUrl } from '@/shared/utils/downloadFile';

interface ClickedLink {
  href: string;
  download: string;
  hidden: boolean;
  attached: boolean;
}

let clicked: ClickedLink[] = [];

beforeEach(() => {
  clicked = [];
  vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(function (this: HTMLAnchorElement) {
    clicked.push({
      href: this.href,
      download: this.download,
      hidden: this.style.display === 'none',
      attached: document.body.contains(this),
    });
  });
  vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:http://localhost/abc');
});

describe('downloadFile', () => {
  it('downloads the blob as an xlsx file through a hidden link', () => {
    const blob = new Blob(['data']);
    downloadFile(blob, 'Workers');
    expect(URL.createObjectURL).toHaveBeenCalledWith(blob);
    expect(clicked).toEqual([{ href: 'blob:http://localhost/abc', download: 'Workers.xlsx', hidden: true, attached: true }]);
  });

  it('removes the link after clicking it', () => {
    downloadFile(new Blob(['data']), 'Workers');
    expect(document.querySelector('a')).toBeNull();
  });
});

describe('downloadPDF', () => {
  it('downloads the blob as a pdf file', () => {
    downloadPDF(new Blob(['pdf']), 'PayStub-001');
    expect(clicked[0]?.download).toBe('PayStub-001.pdf');
  });
});

describe('openFileUrl', () => {
  it('opens the url without a download name or object url', () => {
    openFileUrl('https://files.sigook.com/doc.pdf');
    expect(URL.createObjectURL).not.toHaveBeenCalled();
    expect(clicked).toEqual([{ href: 'https://files.sigook.com/doc.pdf', download: '', hidden: true, attached: true }]);
    expect(document.querySelector('a')).toBeNull();
  });
});
