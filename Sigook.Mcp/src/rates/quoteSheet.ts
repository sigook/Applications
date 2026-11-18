import type { CellValue, WorkbookReader } from '../api/workbookReader.js';
import { parseLocation, roundToCents, type QuoteRate, type QuoteRow, type RateCategory } from './rateComparison.js';

export interface QuoteColumns {
  category: RateCategory;
  payColumn: string;
  billColumn: string;
}

export interface QuoteSheetLayout {
  sheet: string;
  locationColumn: string;
  firstRow: number;
  categories: QuoteColumns[];
}

const toNumber = (value: CellValue): number | undefined => {
  if (typeof value === 'number') return value;
  if (typeof value === 'string' && value.trim() !== '' && !Number.isNaN(Number(value))) return Number(value);
  return undefined;
};

export function readQuote(reader: WorkbookReader, layout: QuoteSheetLayout): { rows: QuoteRow[]; skipped: { row: number; location: string }[] } {
  const rows: QuoteRow[] = [];
  const skipped: { row: number; location: string }[] = [];
  const { rows: sheetRows } = reader.rows(layout.sheet, layout.firstRow, Number.MAX_SAFE_INTEGER);
  for (const { row } of sheetRows) {
    const location = reader.cell(layout.sheet, row, layout.locationColumn);
    if (typeof location !== 'string' || location.trim() === '') continue;
    const rates: Partial<Record<RateCategory, QuoteRate>> = {};
    for (const { category, payColumn, billColumn } of layout.categories) {
      const payRate = toNumber(reader.cell(layout.sheet, row, payColumn));
      const billRate = toNumber(reader.cell(layout.sheet, row, billColumn));
      if (payRate !== undefined && billRate !== undefined && payRate > 0 && billRate > 0) rates[category] = { payRate: roundToCents(payRate), billRate: roundToCents(billRate) };
    }
    const parsed = parseLocation(location);
    if (!parsed) {
      if (Object.keys(rates).length > 0) skipped.push({ row, location });
      continue;
    }
    rows.push({ row, location: location.trim(), ...parsed, rates });
  }
  return { rows, skipped };
}
