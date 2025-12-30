CREATE OR REPLACE VIEW "PayStubHistory" AS
SELECT
    ROW_NUMBER() OVER (ORDER BY "NumberId" DESC) AS "RowNumber",
    "Id",
    "WorkerProfileId",
    "NumberId",
    "PayStubNumber",
    "WeekEnding",
    "TotalEarnings",
    "Vacations",
    "PublicHolidayPay",
    "TotalPaid",
    "DateWorkBegins",
    "DateWorkEnd"
FROM "PayStub"