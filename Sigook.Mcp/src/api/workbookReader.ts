import ExcelJS from 'exceljs';

export type CellValue = string | number | boolean | null;

export interface WorkbookSheet {
  name: string;
  rowCount: number;
  columnCount: number;
}

export interface SheetRows {
  sheet: string;
  sheets: WorkbookSheet[];
  rows: { row: number; cells: CellValue[] }[];
}

function toCellValue(value: ExcelJS.CellValue): CellValue {
  if (value === null || value === undefined) return null;
  if (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean') return value;
  if (value instanceof Date) return value.toISOString();
  if ('result' in value) return toCellValue(value.result as ExcelJS.CellValue);
  if ('richText' in value) return value.richText.map(part => part.text).join('');
  if ('text' in value) return String(value.text);
  if ('error' in value) return String(value.error);
  return null;
}

export const columnIndex = (column: string): number =>
  column.trim().toUpperCase().split('').reduce((total, letter) => {
    const code = letter.charCodeAt(0) - 64;
    if (code < 1 || code > 26) throw new Error(`Invalid column '${column}'. Use letters such as A or AB.`);
    return total * 26 + code;
  }, 0);

export class WorkbookReader {
  private constructor(private readonly workbook: ExcelJS.Workbook) {}

  static async open(path: string): Promise<WorkbookReader> {
    const workbook = new ExcelJS.Workbook();
    await workbook.xlsx.readFile(path);
    return new WorkbookReader(workbook);
  }

  get sheets(): WorkbookSheet[] {
    return this.workbook.worksheets.map(sheet => ({ name: sheet.name, rowCount: sheet.rowCount, columnCount: sheet.columnCount }));
  }

  private worksheet(sheet?: string | null): ExcelJS.Worksheet {
    const worksheet = sheet ? this.workbook.worksheets.find(item => item.name.trim().toLowerCase() === sheet.trim().toLowerCase()) : this.workbook.worksheets[0];
    if (!worksheet) throw new Error(`Sheet '${sheet}' not found. Sheets: ${this.workbook.worksheets.map(item => item.name).join(', ')}.`);
    return worksheet;
  }

  cell(sheet: string, row: number, column: string): CellValue {
    return toCellValue(this.worksheet(sheet).getRow(row).getCell(columnIndex(column)).value);
  }

  rows(sheet?: string | null, firstRow = 1, take = 50): SheetRows {
    const worksheet = this.worksheet(sheet);
    const rows: SheetRows['rows'] = [];
    for (let row = Math.max(1, firstRow); row <= worksheet.rowCount && rows.length < take; row++) {
      const cells = Array.from({ length: worksheet.columnCount }, (_, index) => toCellValue(worksheet.getRow(row).getCell(index + 1).value));
      while (cells.length > 0 && cells[cells.length - 1] === null) cells.pop();
      if (cells.length > 0) rows.push({ row, cells });
    }
    return { sheet: worksheet.name, sheets: this.sheets, rows };
  }
}
