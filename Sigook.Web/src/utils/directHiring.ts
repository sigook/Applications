interface RequestWithSalary {
  workerSalary?: number | null;
}

export function isDirectHiring(request: RequestWithSalary | null | undefined): boolean {
  return !!(request && request.workerSalary);
}
