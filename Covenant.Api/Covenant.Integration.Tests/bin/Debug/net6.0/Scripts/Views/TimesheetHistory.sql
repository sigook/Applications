CREATE OR REPLACE VIEW "TimesheetHistory" AS
SELECT
    ROW_NUMBER() OVER (ORDER BY ts."Date" DESC) AS "RowNumber",
    r."NumberId",
    cp."FullName" AS "CompanyName",
    r."JobTitle",
    ts."Date",
    ts."IsHoliday",
    tst."RegularHours",
    tst."HolidayHours",
    tst."OvertimeHours",
    ts."MissingHours",
    ts."MissingHoursOvertime",
    wp."Id" AS "WorkerProfileId"
FROM "WorkerProfile" wp
JOIN "WorkerRequest" wr ON wp."WorkerId" = wr."WorkerId"
JOIN "TimeSheet" ts ON wr."Id" = ts."WorkerRequestId"
LEFT JOIN "TimeSheetTotal" tst ON ts."Id" = tst."TimeSheetId"
JOIN "Request" r ON wr."RequestId" = r."Id"
JOIN "CompanyProfile" cp ON r."CompanyId" = cp."CompanyId"
ORDER BY ts."Date" DESC;