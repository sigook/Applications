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
FROM "WorkerProfiles" wp
JOIN "WorkerRequests" wr ON wp."Id" = wr."WorkerProfileId"
JOIN "TimeSheets" ts ON wr."Id" = ts."WorkerRequestId"
LEFT JOIN "TimeSheetTotals" tst ON ts."Id" = tst."TimeSheetId"
JOIN "Requests" r ON wr."RequestId" = r."Id"
JOIN "CompanyProfiles" cp ON r."CompanyProfileId" = cp."Id"
ORDER BY ts."Date" DESC;