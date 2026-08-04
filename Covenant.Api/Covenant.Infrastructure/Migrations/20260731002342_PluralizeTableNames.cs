using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PluralizeTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agency_Agency_AgencyParentId",
                table: "Agency");

            migrationBuilder.DropForeignKey(
                name: "FK_Agency_CovenantFile_LogoId",
                table: "Agency");

            migrationBuilder.DropForeignKey(
                name: "FK_Agency_User_UserId",
                table: "Agency");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyContactInformation_Agency_AgencyId",
                table: "AgencyContactInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyLocation_Agency_AgencyId",
                table: "AgencyLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyLocation_Location_LocationId",
                table: "AgencyLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyPersonnel_Agency_AgencyId",
                table: "AgencyPersonnel");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyPersonnel_User_UserId",
                table: "AgencyPersonnel");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyWsibGroup_Agency_AgencyId",
                table: "AgencyWsibGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyWsibGroup_WsibGroup_WsibGroupId",
                table: "AgencyWsibGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocument_Candidates_CandidateId",
                table: "CandidateDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocument_CovenantFile_DocumentId",
                table: "CandidateDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateNote_Candidates_CandidateId",
                table: "CandidateNote");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateNote_CovenantNote_NoteId",
                table: "CandidateNote");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Agency_AgencyId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Gender_GenderId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_City_Province_ProvinceId",
                table: "City");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_AgencyPersonnel_SalesRepresentativeId",
                table: "CompanyProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_Agency_AgencyId",
                table: "CompanyProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_CompanyProfileIndustry_IndustryId",
                table: "CompanyProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_CovenantFile_LogoId",
                table: "CompanyProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfile_User_CompanyId",
                table: "CompanyProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileContactPerson_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileContactPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileDocument_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileDocument_CovenantFile_DocumentId",
                table: "CompanyProfileDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileIndustry_Industry_IndustryId",
                table: "CompanyProfileIndustry");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileInvoiceNotes_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileInvoiceNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileInvoiceRecipient_CompanyProfile_CompanyProfil~",
                table: "CompanyProfileInvoiceRecipient");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileJobPositionRate_CompanyProfile_CompanyProfile~",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileJobPositionRate_Shift_ShiftId",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileLocation_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileLocation_Location_LocationId",
                table: "CompanyProfileLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileNote_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileNote");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileNote_CovenantNote_NoteId",
                table: "CompanyProfileNote");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_CompanyProfile_CompanyProfileId",
                table: "CompanyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_User_UserId",
                table: "CompanyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_CompanyProfile_CompanyProfileId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetail_InvoiceUSA_UsaInvoiceId",
                table: "InvoiceAdditionalDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetail_Invoice_CanadaInvoiceId",
                table: "InvoiceAdditionalDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalItem_Invoice_InvoiceId",
                table: "InvoiceAdditionalItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDiscount_Invoice_InvoiceId",
                table: "InvoiceDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHoliday_Invoice_InvoiceId",
                table: "InvoiceHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHoliday_WorkerProfile_WorkerProfileId",
                table: "InvoiceHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceTotal_Invoice_InvoiceId",
                table: "InvoiceTotal");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceTotal_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceTotal");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSA_CompanyProfile_CompanyProfileId",
                table: "InvoiceUSA");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSADiscount_InvoiceUSA_InvoiceUSAId",
                table: "InvoiceUSADiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSAItem_InvoiceUSA_InvoiceUSAId",
                table: "InvoiceUSAItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSAItem_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSAItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSATimeSheetTotal_InvoiceUSA_InvoiceUSAId",
                table: "InvoiceUSATimeSheetTotal");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSATimeSheetTotal_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotal");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_City_CityId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationTaxes_Location_LocationId",
                table: "LocationTaxes");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStub_WorkerProfile_WorkerProfileId",
                table: "PayStub");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubItem_PayStub_PayStubId",
                table: "PayStubItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubOtherDeduction_PayStub_PayStubId",
                table: "PayStubOtherDeduction");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubPublicHoliday_PayStub_PayStubId",
                table: "PayStubPublicHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubWageDetail_PayStub_PayStubId",
                table: "PayStubWageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubWageDetail_TimeSheetTotalPayroll_TimeSheetTotalId",
                table: "PayStubWageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Province_Country_CountryId",
                table: "Province");

            migrationBuilder.DropForeignKey(
                name: "FK_ProvinceSettings_Province_ProvinceId",
                table: "ProvinceSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ReasonCancellationRequest_StringResource_ValueId",
                table: "ReasonCancellationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractor_WorkerProfile_WorkerProfileId",
                table: "ReportSubcontractor");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubContractorOtherDeduction_ReportSubcontractor_Repor~",
                table: "ReportSubContractorOtherDeduction");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractorPublicHoliday_ReportSubcontractor_Report~",
                table: "ReportSubcontractorPublicHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractorWageDetail_ReportSubcontractor_ReportSub~",
                table: "ReportSubcontractorWageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractorWageDetail_TimeSheetTotalPayroll_TimeShe~",
                table: "ReportSubcontractorWageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_CompanyProfileJobPositionRate_JobPositionRateId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_CompanyProfile_CompanyProfileId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Location_JobLocationId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Shift_ShiftId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApplicant_Candidates_CandidateId",
                table: "RequestApplicant");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApplicant_Request_RequestId",
                table: "RequestApplicant");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApplicant_WorkerProfile_WorkerProfileId",
                table: "RequestApplicant");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCancellationDetail_ReasonCancellationRequest_ReasonC~",
                table: "RequestCancellationDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestComissions_Request_RequestId",
                table: "RequestComissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCompanyUsers_CompanyUser_CompanyUserId",
                table: "RequestCompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCompanyUsers_Request_RequestId",
                table: "RequestCompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestFinalizationDetail_Request_RequestId",
                table: "RequestFinalizationDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestNote_CovenantNote_NoteId",
                table: "RequestNote");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestNote_Request_RequestId",
                table: "RequestNote");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRecruiter_AgencyPersonnel_RecruiterId",
                table: "RequestRecruiter");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRecruiter_Request_RequestId",
                table: "RequestRecruiter");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestReportTo_CompanyProfileContactPerson_ContactPersonId",
                table: "RequestReportTo");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestReportTo_Request_RequestId",
                table: "RequestReportTo");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRequestedBy_CompanyProfileContactPerson_ContactPerso~",
                table: "RequestRequestedBy");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRequestedBy_Request_RequestId",
                table: "RequestRequestedBy");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestSkill_Request_RequestId",
                table: "RequestSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestSource_Request_RequestId",
                table: "RequestSource");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestSource_Sources_SourceId",
                table: "RequestSource");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerInterviews_User_CreatedBy",
                table: "RunnerInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerInterviews_User_RescheduledBy",
                table: "RunnerInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_RequestRecruiter_RequestRecruiterId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_Request_RequestId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_User_CreatedBy",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_User_UpdatedBy",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_WorkerProfile_WorkerProfileId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerStatusHistories_User_ChangedBy",
                table: "RunnerStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheet_WorkerRequest_WorkerRequestId",
                table: "TimeSheet");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheetTotal_TimeSheet_TimeSheetId",
                table: "TimeSheetTotal");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheetTotalPayroll_TimeSheet_TimeSheetId",
                table: "TimeSheetTotalPayroll");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationType_NotificationType_NotificationTypeId",
                table: "UserNotificationType");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationType_User_UserId",
                table: "UserNotificationType");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_CompanyProfile_CompanyProfileId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComment_WorkerProfile_WorkerProfileId",
                table: "WorkerComment");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_Agency_AgencyId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_CovenantFile_IdentificationType1FileId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_CovenantFile_IdentificationType2FileId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_CovenantFile_PoliceCheckBackGroundId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_CovenantFile_ProfileImageId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_CovenantFile_ResumeId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_CovenantFile_SocialInsuranceFileId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_Gender_GenderId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_IdentificationType_IdentificationType1Id",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_IdentificationType_IdentificationType2Id",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_Lift_LiftId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_Location_LocationId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfile_User_WorkerId",
                table: "WorkerProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailability_Availability_AvailabilityId",
                table: "WorkerProfileAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailability_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileAvailability");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityDay_Day_DayId",
                table: "WorkerProfileAvailabilityDay");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityDay_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileAvailabilityDay");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityTime_AvailabilityTime_Availability~",
                table: "WorkerProfileAvailabilityTime");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityTime_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileAvailabilityTime");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileCertificate_CovenantFile_CertificateId",
                table: "WorkerProfileCertificate");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileCertificate_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileCertificate");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileHoliday_Holiday_HolidayId",
                table: "WorkerProfileHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileHoliday_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileHoliday");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileJobExperience_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileJobExperience");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLanguage_Language_LanguageId",
                table: "WorkerProfileLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLanguage_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileLanguage");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLicense_CovenantFile_LicenseId",
                table: "WorkerProfileLicense");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLicense_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileLicense");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLocationPreference_City_CityId",
                table: "WorkerProfileLocationPreference");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLocationPreference_WorkerProfile_WorkerProfile~",
                table: "WorkerProfileLocationPreference");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileNote_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileNote");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileOtherDocument_CovenantFile_DocumentId",
                table: "WorkerProfileOtherDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileOtherDocument_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileOtherDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileSkill_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileTaxCategories_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileTaxCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequest_Request_RequestId",
                table: "WorkerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequest_WorkerProfile_WorkerProfileId",
                table: "WorkerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequestNote_CovenantNote_NoteId",
                table: "WorkerRequestNote");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequestNote_WorkerRequest_WorkerRequestId",
                table: "WorkerRequestNote");

            migrationBuilder.DropTable(
                name: "CompanyProfileHoliday");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ReasonCancellationRequest",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "ReasonCancellationRequest" r
                SET "Value" = s."En"
                FROM "StringResource" s
                WHERE r."ValueId" = s."Id";
                """);

            migrationBuilder.DropTable(
                name: "StringResource");

            migrationBuilder.DropTable(
                name: "TimeSheetPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WsibGroup",
                table: "WsibGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerRequestNote",
                table: "WorkerRequestNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerRequest",
                table: "WorkerRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileSkill",
                table: "WorkerProfileSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileOtherDocument",
                table: "WorkerProfileOtherDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileNote",
                table: "WorkerProfileNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileLocationPreference",
                table: "WorkerProfileLocationPreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileLicense",
                table: "WorkerProfileLicense");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileLanguage",
                table: "WorkerProfileLanguage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileJobExperience",
                table: "WorkerProfileJobExperience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileHoliday",
                table: "WorkerProfileHoliday");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileCertificate",
                table: "WorkerProfileCertificate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileAvailabilityTime",
                table: "WorkerProfileAvailabilityTime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileAvailabilityDay",
                table: "WorkerProfileAvailabilityDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileAvailability",
                table: "WorkerProfileAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfile",
                table: "WorkerProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerComment",
                table: "WorkerComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotificationType",
                table: "UserNotificationType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSheetTotalPayroll",
                table: "TimeSheetTotalPayroll");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSheetTotal",
                table: "TimeSheetTotal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSheet",
                table: "TimeSheet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkipPayrollNumber",
                table: "SkipPayrollNumber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shift",
                table: "Shift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestSource",
                table: "RequestSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestSkill",
                table: "RequestSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRequestedBy",
                table: "RequestRequestedBy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestReportTo",
                table: "RequestReportTo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRecruiter",
                table: "RequestRecruiter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestNote",
                table: "RequestNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestFinalizationDetail",
                table: "RequestFinalizationDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestCancellationDetail",
                table: "RequestCancellationDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestApplicant",
                table: "RequestApplicant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Request",
                table: "Request");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubcontractorWageDetail",
                table: "ReportSubcontractorWageDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubcontractorPublicHoliday",
                table: "ReportSubcontractorPublicHoliday");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubContractorOtherDeduction",
                table: "ReportSubContractorOtherDeduction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubcontractor",
                table: "ReportSubcontractor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReasonCancellationRequest",
                table: "ReasonCancellationRequest");

            migrationBuilder.DropIndex(
                name: "IX_ReasonCancellationRequest_ValueId",
                table: "ReasonCancellationRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Province",
                table: "Province");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubWageDetail",
                table: "PayStubWageDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubPublicHoliday",
                table: "PayStubPublicHoliday");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubOtherDeduction",
                table: "PayStubOtherDeduction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubItem",
                table: "PayStubItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStub",
                table: "PayStub");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationType",
                table: "NotificationType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lift",
                table: "Lift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Language",
                table: "Language");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSATimeSheetTotal",
                table: "InvoiceUSATimeSheetTotal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSAItem",
                table: "InvoiceUSAItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSADiscount",
                table: "InvoiceUSADiscount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSA",
                table: "InvoiceUSA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceTotal",
                table: "InvoiceTotal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceHoliday",
                table: "InvoiceHoliday");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDiscount",
                table: "InvoiceDiscount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceAdditionalItem",
                table: "InvoiceAdditionalItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceAdditionalDetail",
                table: "InvoiceAdditionalDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Industry",
                table: "Industry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentificationType",
                table: "IdentificationType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Holiday",
                table: "Holiday");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gender",
                table: "Gender");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Day",
                table: "Day");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CovenantNote",
                table: "CovenantNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CovenantFile",
                table: "CovenantFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Country",
                table: "Country");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyUser",
                table: "CompanyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileNote",
                table: "CompanyProfileNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileLocation",
                table: "CompanyProfileLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileJobPositionRate",
                table: "CompanyProfileJobPositionRate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileInvoiceRecipient",
                table: "CompanyProfileInvoiceRecipient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileIndustry",
                table: "CompanyProfileIndustry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileDocument",
                table: "CompanyProfileDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileContactPerson",
                table: "CompanyProfileContactPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfile",
                table: "CompanyProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_City",
                table: "City");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateNote",
                table: "CandidateNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateDocument",
                table: "CandidateDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AvailabilityTime",
                table: "AvailabilityTime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Availability",
                table: "Availability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgencyWsibGroup",
                table: "AgencyWsibGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgencyLocation",
                table: "AgencyLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agency",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "ValueId",
                table: "ReasonCancellationRequest");

            migrationBuilder.RenameTable(
                name: "WsibGroup",
                newName: "WsibGroups");

            migrationBuilder.RenameTable(
                name: "WorkerRequestNote",
                newName: "WorkerRequestNotes");

            migrationBuilder.RenameTable(
                name: "WorkerRequest",
                newName: "WorkerRequests");

            migrationBuilder.RenameTable(
                name: "WorkerProfileSkill",
                newName: "WorkerProfileSkills");

            migrationBuilder.RenameTable(
                name: "WorkerProfileOtherDocument",
                newName: "WorkerProfileOtherDocuments");

            migrationBuilder.RenameTable(
                name: "WorkerProfileNote",
                newName: "WorkerProfileNotes");

            migrationBuilder.RenameTable(
                name: "WorkerProfileLocationPreference",
                newName: "WorkerProfileLocationPreferences");

            migrationBuilder.RenameTable(
                name: "WorkerProfileLicense",
                newName: "WorkerProfileLicenses");

            migrationBuilder.RenameTable(
                name: "WorkerProfileLanguage",
                newName: "WorkerProfileLanguages");

            migrationBuilder.RenameTable(
                name: "WorkerProfileJobExperience",
                newName: "WorkerProfileJobExperiences");

            migrationBuilder.RenameTable(
                name: "WorkerProfileHoliday",
                newName: "WorkerProfileHolidays");

            migrationBuilder.RenameTable(
                name: "WorkerProfileCertificate",
                newName: "WorkerProfileCertificates");

            migrationBuilder.RenameTable(
                name: "WorkerProfileAvailabilityTime",
                newName: "WorkerProfileAvailabilityTimes");

            migrationBuilder.RenameTable(
                name: "WorkerProfileAvailabilityDay",
                newName: "WorkerProfileAvailabilityDays");

            migrationBuilder.RenameTable(
                name: "WorkerProfileAvailability",
                newName: "WorkerProfileAvailabilities");

            migrationBuilder.RenameTable(
                name: "WorkerProfile",
                newName: "WorkerProfiles");

            migrationBuilder.RenameTable(
                name: "WorkerComment",
                newName: "WorkerComments");

            migrationBuilder.RenameTable(
                name: "UserNotificationType",
                newName: "UserNotificationTypes");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "TimeSheetTotalPayroll",
                newName: "TimeSheetTotalPayrolls");

            migrationBuilder.RenameTable(
                name: "TimeSheetTotal",
                newName: "TimeSheetTotals");

            migrationBuilder.RenameTable(
                name: "TimeSheet",
                newName: "TimeSheets");

            migrationBuilder.RenameTable(
                name: "SkipPayrollNumber",
                newName: "SkipPayrollNumbers");

            migrationBuilder.RenameTable(
                name: "Shift",
                newName: "Shifts");

            migrationBuilder.RenameTable(
                name: "RequestSource",
                newName: "RequestSources");

            migrationBuilder.RenameTable(
                name: "RequestSkill",
                newName: "RequestSkills");

            migrationBuilder.RenameTable(
                name: "RequestRequestedBy",
                newName: "RequestRequestedBys");

            migrationBuilder.RenameTable(
                name: "RequestReportTo",
                newName: "RequestReportTos");

            migrationBuilder.RenameTable(
                name: "RequestRecruiter",
                newName: "RequestRecruiters");

            migrationBuilder.RenameTable(
                name: "RequestNote",
                newName: "RequestNotes");

            migrationBuilder.RenameTable(
                name: "RequestFinalizationDetail",
                newName: "RequestFinalizationDetails");

            migrationBuilder.RenameTable(
                name: "RequestCancellationDetail",
                newName: "RequestCancellationDetails");

            migrationBuilder.RenameTable(
                name: "RequestApplicant",
                newName: "RequestApplicants");

            migrationBuilder.RenameTable(
                name: "Request",
                newName: "Requests");

            migrationBuilder.RenameTable(
                name: "ReportSubcontractorWageDetail",
                newName: "ReportSubcontractorWageDetails");

            migrationBuilder.RenameTable(
                name: "ReportSubcontractorPublicHoliday",
                newName: "ReportSubcontractorPublicHolidays");

            migrationBuilder.RenameTable(
                name: "ReportSubContractorOtherDeduction",
                newName: "ReportSubContractorOtherDeductions");

            migrationBuilder.RenameTable(
                name: "ReportSubcontractor",
                newName: "ReportSubcontractors");

            migrationBuilder.RenameTable(
                name: "ReasonCancellationRequest",
                newName: "ReasonCancellationRequests");

            migrationBuilder.RenameTable(
                name: "Province",
                newName: "Provinces");

            migrationBuilder.RenameTable(
                name: "PayStubWageDetail",
                newName: "PayStubWageDetails");

            migrationBuilder.RenameTable(
                name: "PayStubPublicHoliday",
                newName: "PayStubPublicHolidays");

            migrationBuilder.RenameTable(
                name: "PayStubOtherDeduction",
                newName: "PayStubOtherDeductions");

            migrationBuilder.RenameTable(
                name: "PayStubItem",
                newName: "PayStubItems");

            migrationBuilder.RenameTable(
                name: "PayStub",
                newName: "PayStubs");

            migrationBuilder.RenameTable(
                name: "NotificationType",
                newName: "NotificationTypes");

            migrationBuilder.RenameTable(
                name: "Location",
                newName: "Locations");

            migrationBuilder.RenameTable(
                name: "Lift",
                newName: "Lifts");

            migrationBuilder.RenameTable(
                name: "Language",
                newName: "Languages");

            migrationBuilder.RenameTable(
                name: "InvoiceUSATimeSheetTotal",
                newName: "InvoiceUSATimeSheetTotals");

            migrationBuilder.RenameTable(
                name: "InvoiceUSAItem",
                newName: "InvoiceUSAItems");

            migrationBuilder.RenameTable(
                name: "InvoiceUSADiscount",
                newName: "InvoiceUSADiscounts");

            migrationBuilder.RenameTable(
                name: "InvoiceUSA",
                newName: "InvoicesUSA");

            migrationBuilder.RenameTable(
                name: "InvoiceTotal",
                newName: "InvoiceTotals");

            migrationBuilder.RenameTable(
                name: "InvoiceHoliday",
                newName: "InvoiceHolidays");

            migrationBuilder.RenameTable(
                name: "InvoiceDiscount",
                newName: "InvoiceDiscounts");

            migrationBuilder.RenameTable(
                name: "InvoiceAdditionalItem",
                newName: "InvoiceAdditionalItems");

            migrationBuilder.RenameTable(
                name: "InvoiceAdditionalDetail",
                newName: "InvoiceAdditionalDetails");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "Industry",
                newName: "Industries");

            migrationBuilder.RenameTable(
                name: "IdentificationType",
                newName: "IdentificationTypes");

            migrationBuilder.RenameTable(
                name: "Holiday",
                newName: "Holidays");

            migrationBuilder.RenameTable(
                name: "Gender",
                newName: "Genders");

            migrationBuilder.RenameTable(
                name: "Day",
                newName: "Days");

            migrationBuilder.RenameTable(
                name: "CovenantNote",
                newName: "CovenantNotes");

            migrationBuilder.RenameTable(
                name: "CovenantFile",
                newName: "CovenantFiles");

            migrationBuilder.RenameTable(
                name: "Country",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "CompanyUser",
                newName: "CompanyUsers");

            migrationBuilder.RenameTable(
                name: "CompanyProfileNote",
                newName: "CompanyProfileNotes");

            migrationBuilder.RenameTable(
                name: "CompanyProfileLocation",
                newName: "CompanyProfileLocations");

            migrationBuilder.RenameTable(
                name: "CompanyProfileJobPositionRate",
                newName: "CompanyProfileJobPositionRates");

            migrationBuilder.RenameTable(
                name: "CompanyProfileInvoiceRecipient",
                newName: "CompanyProfileInvoiceRecipients");

            migrationBuilder.RenameTable(
                name: "CompanyProfileIndustry",
                newName: "CompanyProfileIndustries");

            migrationBuilder.RenameTable(
                name: "CompanyProfileDocument",
                newName: "CompanyProfileDocuments");

            migrationBuilder.RenameTable(
                name: "CompanyProfileContactPerson",
                newName: "CompanyProfileContactPeople");

            migrationBuilder.RenameTable(
                name: "CompanyProfile",
                newName: "CompanyProfiles");

            migrationBuilder.RenameTable(
                name: "City",
                newName: "Cities");

            migrationBuilder.RenameTable(
                name: "CandidateNote",
                newName: "CandidateNotes");

            migrationBuilder.RenameTable(
                name: "CandidateDocument",
                newName: "CandidateDocuments");

            migrationBuilder.RenameTable(
                name: "AvailabilityTime",
                newName: "AvailabilityTimes");

            migrationBuilder.RenameTable(
                name: "Availability",
                newName: "Availabilities");

            migrationBuilder.RenameTable(
                name: "AgencyWsibGroup",
                newName: "AgencyWsibGroups");

            migrationBuilder.RenameTable(
                name: "AgencyLocation",
                newName: "AgencyLocations");

            migrationBuilder.RenameTable(
                name: "Agency",
                newName: "Agencies");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequestNote_NoteId",
                table: "WorkerRequestNotes",
                newName: "IX_WorkerRequestNotes_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequest_WorkerProfileId",
                table: "WorkerRequests",
                newName: "IX_WorkerRequests_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequest_RequestId_WorkerProfileId",
                table: "WorkerRequests",
                newName: "IX_WorkerRequests_RequestId_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileSkill_WorkerProfileId",
                table: "WorkerProfileSkills",
                newName: "IX_WorkerProfileSkills_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileOtherDocument_WorkerProfileId",
                table: "WorkerProfileOtherDocuments",
                newName: "IX_WorkerProfileOtherDocuments_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileOtherDocument_DocumentId",
                table: "WorkerProfileOtherDocuments",
                newName: "IX_WorkerProfileOtherDocuments_DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileNote_WorkerProfileId",
                table: "WorkerProfileNotes",
                newName: "IX_WorkerProfileNotes_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLocationPreference_CityId",
                table: "WorkerProfileLocationPreferences",
                newName: "IX_WorkerProfileLocationPreferences_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLicense_WorkerProfileId",
                table: "WorkerProfileLicenses",
                newName: "IX_WorkerProfileLicenses_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLicense_LicenseId",
                table: "WorkerProfileLicenses",
                newName: "IX_WorkerProfileLicenses_LicenseId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLanguage_LanguageId",
                table: "WorkerProfileLanguages",
                newName: "IX_WorkerProfileLanguages_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileJobExperience_WorkerProfileId",
                table: "WorkerProfileJobExperiences",
                newName: "IX_WorkerProfileJobExperiences_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileHoliday_WorkerProfileId_HolidayId",
                table: "WorkerProfileHolidays",
                newName: "IX_WorkerProfileHolidays_WorkerProfileId_HolidayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileHoliday_HolidayId",
                table: "WorkerProfileHolidays",
                newName: "IX_WorkerProfileHolidays_HolidayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileCertificate_WorkerProfileId",
                table: "WorkerProfileCertificates",
                newName: "IX_WorkerProfileCertificates_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileCertificate_CertificateId",
                table: "WorkerProfileCertificates",
                newName: "IX_WorkerProfileCertificates_CertificateId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileAvailabilityTime_AvailabilityTimeId",
                table: "WorkerProfileAvailabilityTimes",
                newName: "IX_WorkerProfileAvailabilityTimes_AvailabilityTimeId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileAvailabilityDay_DayId",
                table: "WorkerProfileAvailabilityDays",
                newName: "IX_WorkerProfileAvailabilityDays_DayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileAvailability_AvailabilityId",
                table: "WorkerProfileAvailabilities",
                newName: "IX_WorkerProfileAvailabilities_AvailabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_WorkerId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_SocialInsuranceFileId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_SocialInsuranceFileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_ResumeId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_ProfileImageId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_ProfileImageId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_PoliceCheckBackGroundId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_PoliceCheckBackGroundId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_LocationId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_LiftId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_LiftId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_IdentificationType2Id",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_IdentificationType2Id");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_IdentificationType2FileId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_IdentificationType2FileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_IdentificationType1Id",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_IdentificationType1Id");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_IdentificationType1FileId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_IdentificationType1FileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_GenderId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_GenderId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfile_AgencyId",
                table: "WorkerProfiles",
                newName: "IX_WorkerProfiles_AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComment_WorkerProfileId",
                table: "WorkerComments",
                newName: "IX_WorkerComments_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComment_CompanyProfileId",
                table: "WorkerComments",
                newName: "IX_WorkerComments_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotificationType_UserId_NotificationTypeId",
                table: "UserNotificationTypes",
                newName: "IX_UserNotificationTypes_UserId_NotificationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotificationType_NotificationTypeId",
                table: "UserNotificationTypes",
                newName: "IX_UserNotificationTypes_NotificationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSheetTotalPayroll_TimeSheetId",
                table: "TimeSheetTotalPayrolls",
                newName: "IX_TimeSheetTotalPayrolls_TimeSheetId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSheetTotal_TimeSheetId",
                table: "TimeSheetTotals",
                newName: "IX_TimeSheetTotals_TimeSheetId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSheet_WorkerRequestId",
                table: "TimeSheets",
                newName: "IX_TimeSheets_WorkerRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestSource_SourceId",
                table: "RequestSources",
                newName: "IX_RequestSources_SourceId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestSkill_RequestId",
                table: "RequestSkills",
                newName: "IX_RequestSkills_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestRequestedBy_ContactPersonId",
                table: "RequestRequestedBys",
                newName: "IX_RequestRequestedBys_ContactPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestReportTo_ContactPersonId",
                table: "RequestReportTos",
                newName: "IX_RequestReportTos_ContactPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestRecruiter_RequestId_RecruiterId_WorkDate",
                table: "RequestRecruiters",
                newName: "IX_RequestRecruiters_RequestId_RecruiterId_WorkDate");

            migrationBuilder.RenameIndex(
                name: "IX_RequestRecruiter_RecruiterId",
                table: "RequestRecruiters",
                newName: "IX_RequestRecruiters_RecruiterId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestNote_NoteId",
                table: "RequestNotes",
                newName: "IX_RequestNotes_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestFinalizationDetail_RequestId",
                table: "RequestFinalizationDetails",
                newName: "IX_RequestFinalizationDetails_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestCancellationDetail_ReasonCancellationRequestId",
                table: "RequestCancellationDetails",
                newName: "IX_RequestCancellationDetails_ReasonCancellationRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestApplicant_WorkerProfileId",
                table: "RequestApplicants",
                newName: "IX_RequestApplicants_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestApplicant_RequestId",
                table: "RequestApplicants",
                newName: "IX_RequestApplicants_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestApplicant_CandidateId",
                table: "RequestApplicants",
                newName: "IX_RequestApplicants_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_ShiftId",
                table: "Requests",
                newName: "IX_Requests_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_JobPositionRateId",
                table: "Requests",
                newName: "IX_Requests_JobPositionRateId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_JobLocationId",
                table: "Requests",
                newName: "IX_Requests_JobLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_CompanyProfileId",
                table: "Requests",
                newName: "IX_Requests_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractorWageDetail_TimeSheetTotalId",
                table: "ReportSubcontractorWageDetails",
                newName: "IX_ReportSubcontractorWageDetails_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractorWageDetail_ReportSubcontractorId",
                table: "ReportSubcontractorWageDetails",
                newName: "IX_ReportSubcontractorWageDetails_ReportSubcontractorId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractorPublicHoliday_ReportSubcontractorId",
                table: "ReportSubcontractorPublicHolidays",
                newName: "IX_ReportSubcontractorPublicHolidays_ReportSubcontractorId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubContractorOtherDeduction_ReportSubcontractorId",
                table: "ReportSubContractorOtherDeductions",
                newName: "IX_ReportSubContractorOtherDeductions_ReportSubcontractorId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractor_WorkerProfileId",
                table: "ReportSubcontractors",
                newName: "IX_ReportSubcontractors_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Province_CountryId",
                table: "Provinces",
                newName: "IX_Provinces_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubWageDetail_TimeSheetTotalId",
                table: "PayStubWageDetails",
                newName: "IX_PayStubWageDetails_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubWageDetail_PayStubId",
                table: "PayStubWageDetails",
                newName: "IX_PayStubWageDetails_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubPublicHoliday_PayStubId",
                table: "PayStubPublicHolidays",
                newName: "IX_PayStubPublicHolidays_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubOtherDeduction_PayStubId",
                table: "PayStubOtherDeductions",
                newName: "IX_PayStubOtherDeductions_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubItem_PayStubId",
                table: "PayStubItems",
                newName: "IX_PayStubItems_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStub_WorkerProfileId",
                table: "PayStubs",
                newName: "IX_PayStubs_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_CityId",
                table: "Locations",
                newName: "IX_Locations_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSATimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotals",
                newName: "IX_InvoiceUSATimeSheetTotals_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSAItem_TimeSheetTotalId",
                table: "InvoiceUSAItems",
                newName: "IX_InvoiceUSAItems_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSAItem_InvoiceUSAId",
                table: "InvoiceUSAItems",
                newName: "IX_InvoiceUSAItems_InvoiceUSAId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSADiscount_InvoiceUSAId",
                table: "InvoiceUSADiscounts",
                newName: "IX_InvoiceUSADiscounts_InvoiceUSAId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSA_InvoiceNumberId",
                table: "InvoicesUSA",
                newName: "IX_InvoicesUSA_InvoiceNumberId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSA_InvoiceNumber",
                table: "InvoicesUSA",
                newName: "IX_InvoicesUSA_InvoiceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSA_CompanyProfileId",
                table: "InvoicesUSA",
                newName: "IX_InvoicesUSA_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceTotal_TimeSheetTotalId",
                table: "InvoiceTotals",
                newName: "IX_InvoiceTotals_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceTotal_InvoiceId",
                table: "InvoiceTotals",
                newName: "IX_InvoiceTotals_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceHoliday_WorkerProfileId",
                table: "InvoiceHolidays",
                newName: "IX_InvoiceHolidays_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceHoliday_InvoiceId",
                table: "InvoiceHolidays",
                newName: "IX_InvoiceHolidays_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDiscount_InvoiceId",
                table: "InvoiceDiscounts",
                newName: "IX_InvoiceDiscounts_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceAdditionalItem_InvoiceId",
                table: "InvoiceAdditionalItems",
                newName: "IX_InvoiceAdditionalItems_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceAdditionalDetail_UsaInvoiceId",
                table: "InvoiceAdditionalDetails",
                newName: "IX_InvoiceAdditionalDetails_UsaInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceAdditionalDetail_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails",
                newName: "IX_InvoiceAdditionalDetails_CanadaInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CompanyProfileId",
                table: "Invoices",
                newName: "IX_Invoices_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Holiday_Date",
                table: "Holidays",
                newName: "IX_Holidays_Date");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_UserId",
                table: "CompanyUsers",
                newName: "IX_CompanyUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_CompanyProfileId_UserId",
                table: "CompanyUsers",
                newName: "IX_CompanyUsers_CompanyProfileId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileNote_NoteId",
                table: "CompanyProfileNotes",
                newName: "IX_CompanyProfileNotes_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileLocation_LocationId",
                table: "CompanyProfileLocations",
                newName: "IX_CompanyProfileLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileJobPositionRate_ShiftId",
                table: "CompanyProfileJobPositionRates",
                newName: "IX_CompanyProfileJobPositionRates_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileJobPositionRate_CompanyProfileId",
                table: "CompanyProfileJobPositionRates",
                newName: "IX_CompanyProfileJobPositionRates_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileInvoiceRecipient_CompanyProfileId",
                table: "CompanyProfileInvoiceRecipients",
                newName: "IX_CompanyProfileInvoiceRecipients_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileIndustry_IndustryId",
                table: "CompanyProfileIndustries",
                newName: "IX_CompanyProfileIndustries_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileDocument_CompanyProfileId",
                table: "CompanyProfileDocuments",
                newName: "IX_CompanyProfileDocuments_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileContactPerson_CompanyProfileId",
                table: "CompanyProfileContactPeople",
                newName: "IX_CompanyProfileContactPeople_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfile_SalesRepresentativeId",
                table: "CompanyProfiles",
                newName: "IX_CompanyProfiles_SalesRepresentativeId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfile_LogoId",
                table: "CompanyProfiles",
                newName: "IX_CompanyProfiles_LogoId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfile_IndustryId",
                table: "CompanyProfiles",
                newName: "IX_CompanyProfiles_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfile_CompanyId",
                table: "CompanyProfiles",
                newName: "IX_CompanyProfiles_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfile_AgencyId",
                table: "CompanyProfiles",
                newName: "IX_CompanyProfiles_AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_City_ProvinceId",
                table: "Cities",
                newName: "IX_Cities_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateNote_NoteId",
                table: "CandidateNotes",
                newName: "IX_CandidateNotes_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateDocument_DocumentId",
                table: "CandidateDocuments",
                newName: "IX_CandidateDocuments_DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_AgencyWsibGroup_WsibGroupId",
                table: "AgencyWsibGroups",
                newName: "IX_AgencyWsibGroups_WsibGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_AgencyLocation_LocationId",
                table: "AgencyLocations",
                newName: "IX_AgencyLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Agency_UserId",
                table: "Agencies",
                newName: "IX_Agencies_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Agency_LogoId",
                table: "Agencies",
                newName: "IX_Agencies_LogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Agency_AgencyParentId",
                table: "Agencies",
                newName: "IX_Agencies_AgencyParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WsibGroups",
                table: "WsibGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerRequestNotes",
                table: "WorkerRequestNotes",
                columns: new[] { "WorkerRequestId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerRequests",
                table: "WorkerRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileSkills",
                table: "WorkerProfileSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileOtherDocuments",
                table: "WorkerProfileOtherDocuments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileNotes",
                table: "WorkerProfileNotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileLocationPreferences",
                table: "WorkerProfileLocationPreferences",
                columns: new[] { "WorkerProfileId", "CityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileLicenses",
                table: "WorkerProfileLicenses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileLanguages",
                table: "WorkerProfileLanguages",
                columns: new[] { "WorkerProfileId", "LanguageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileJobExperiences",
                table: "WorkerProfileJobExperiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileHolidays",
                table: "WorkerProfileHolidays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileCertificates",
                table: "WorkerProfileCertificates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileAvailabilityTimes",
                table: "WorkerProfileAvailabilityTimes",
                columns: new[] { "WorkerProfileId", "AvailabilityTimeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileAvailabilityDays",
                table: "WorkerProfileAvailabilityDays",
                columns: new[] { "WorkerProfileId", "DayId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileAvailabilities",
                table: "WorkerProfileAvailabilities",
                columns: new[] { "WorkerProfileId", "AvailabilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfiles",
                table: "WorkerProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerComments",
                table: "WorkerComments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotificationTypes",
                table: "UserNotificationTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSheetTotalPayrolls",
                table: "TimeSheetTotalPayrolls",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSheetTotals",
                table: "TimeSheetTotals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSheets",
                table: "TimeSheets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkipPayrollNumbers",
                table: "SkipPayrollNumbers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestSources",
                table: "RequestSources",
                columns: new[] { "RequestId", "SourceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestSkills",
                table: "RequestSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRequestedBys",
                table: "RequestRequestedBys",
                columns: new[] { "RequestId", "ContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestReportTos",
                table: "RequestReportTos",
                columns: new[] { "RequestId", "ContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRecruiters",
                table: "RequestRecruiters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestNotes",
                table: "RequestNotes",
                columns: new[] { "RequestId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestFinalizationDetails",
                table: "RequestFinalizationDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestCancellationDetails",
                table: "RequestCancellationDetails",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestApplicants",
                table: "RequestApplicants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubcontractorWageDetails",
                table: "ReportSubcontractorWageDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubcontractorPublicHolidays",
                table: "ReportSubcontractorPublicHolidays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubContractorOtherDeductions",
                table: "ReportSubContractorOtherDeductions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubcontractors",
                table: "ReportSubcontractors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReasonCancellationRequests",
                table: "ReasonCancellationRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provinces",
                table: "Provinces",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubWageDetails",
                table: "PayStubWageDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubPublicHolidays",
                table: "PayStubPublicHolidays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubOtherDeductions",
                table: "PayStubOtherDeductions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubItems",
                table: "PayStubItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubs",
                table: "PayStubs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTypes",
                table: "NotificationTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lifts",
                table: "Lifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                table: "Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSATimeSheetTotals",
                table: "InvoiceUSATimeSheetTotals",
                columns: new[] { "InvoiceUSAId", "TimeSheetTotalId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSAItems",
                table: "InvoiceUSAItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSADiscounts",
                table: "InvoiceUSADiscounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoicesUSA",
                table: "InvoicesUSA",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceTotals",
                table: "InvoiceTotals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceHolidays",
                table: "InvoiceHolidays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDiscounts",
                table: "InvoiceDiscounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceAdditionalItems",
                table: "InvoiceAdditionalItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceAdditionalDetails",
                table: "InvoiceAdditionalDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Industries",
                table: "Industries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentificationTypes",
                table: "IdentificationTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holidays",
                table: "Holidays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genders",
                table: "Genders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Days",
                table: "Days",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CovenantNotes",
                table: "CovenantNotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CovenantFiles",
                table: "CovenantFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyUsers",
                table: "CompanyUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileNotes",
                table: "CompanyProfileNotes",
                columns: new[] { "CompanyProfileId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileLocations",
                table: "CompanyProfileLocations",
                columns: new[] { "CompanyProfileId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileJobPositionRates",
                table: "CompanyProfileJobPositionRates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileInvoiceRecipients",
                table: "CompanyProfileInvoiceRecipients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileIndustries",
                table: "CompanyProfileIndustries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileDocuments",
                table: "CompanyProfileDocuments",
                columns: new[] { "DocumentId", "CompanyProfileId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileContactPeople",
                table: "CompanyProfileContactPeople",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfiles",
                table: "CompanyProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cities",
                table: "Cities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateNotes",
                table: "CandidateNotes",
                columns: new[] { "CandidateId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateDocuments",
                table: "CandidateDocuments",
                columns: new[] { "CandidateId", "DocumentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AvailabilityTimes",
                table: "AvailabilityTimes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Availabilities",
                table: "Availabilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgencyWsibGroups",
                table: "AgencyWsibGroups",
                columns: new[] { "AgencyId", "WsibGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgencyLocations",
                table: "AgencyLocations",
                columns: new[] { "AgencyId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agencies",
                table: "Agencies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_Agencies_AgencyParentId",
                table: "Agencies",
                column: "AgencyParentId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_CovenantFiles_LogoId",
                table: "Agencies",
                column: "LogoId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_Users_UserId",
                table: "Agencies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyContactInformation_Agencies_AgencyId",
                table: "AgencyContactInformation",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyLocations_Agencies_AgencyId",
                table: "AgencyLocations",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyLocations_Locations_LocationId",
                table: "AgencyLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyPersonnel_Agencies_AgencyId",
                table: "AgencyPersonnel",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyPersonnel_Users_UserId",
                table: "AgencyPersonnel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyWsibGroups_Agencies_AgencyId",
                table: "AgencyWsibGroups",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyWsibGroups_WsibGroups_WsibGroupId",
                table: "AgencyWsibGroups",
                column: "WsibGroupId",
                principalTable: "WsibGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocuments_Candidates_CandidateId",
                table: "CandidateDocuments",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocuments_CovenantFiles_DocumentId",
                table: "CandidateDocuments",
                column: "DocumentId",
                principalTable: "CovenantFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateNotes_Candidates_CandidateId",
                table: "CandidateNotes",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateNotes_CovenantNotes_NoteId",
                table: "CandidateNotes",
                column: "NoteId",
                principalTable: "CovenantNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Agencies_AgencyId",
                table: "Candidates",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Genders_GenderId",
                table: "Candidates",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provinces_ProvinceId",
                table: "Cities",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileContactPeople_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileContactPeople",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileDocuments_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileDocuments",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileDocuments_CovenantFiles_DocumentId",
                table: "CompanyProfileDocuments",
                column: "DocumentId",
                principalTable: "CovenantFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileIndustries_Industries_IndustryId",
                table: "CompanyProfileIndustries",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileInvoiceNotes_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileInvoiceNotes",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileInvoiceRecipients_CompanyProfiles_CompanyProf~",
                table: "CompanyProfileInvoiceRecipients",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileJobPositionRates_CompanyProfiles_CompanyProfi~",
                table: "CompanyProfileJobPositionRates",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileJobPositionRates_Shifts_ShiftId",
                table: "CompanyProfileJobPositionRates",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileLocations_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileLocations",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileLocations_Locations_LocationId",
                table: "CompanyProfileLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileNotes_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileNotes",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileNotes_CovenantNotes_NoteId",
                table: "CompanyProfileNotes",
                column: "NoteId",
                principalTable: "CovenantNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_Agencies_AgencyId",
                table: "CompanyProfiles",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_AgencyPersonnel_SalesRepresentativeId",
                table: "CompanyProfiles",
                column: "SalesRepresentativeId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_CompanyProfileIndustries_IndustryId",
                table: "CompanyProfiles",
                column: "IndustryId",
                principalTable: "CompanyProfileIndustries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_CovenantFiles_LogoId",
                table: "CompanyProfiles",
                column: "LogoId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_Users_CompanyId",
                table: "CompanyProfiles",
                column: "CompanyId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUsers_CompanyProfiles_CompanyProfileId",
                table: "CompanyUsers",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUsers_Users_UserId",
                table: "CompanyUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetails_InvoicesUSA_UsaInvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "UsaInvoiceId",
                principalTable: "InvoicesUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetails_Invoices_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails",
                column: "CanadaInvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalItems_Invoices_InvoiceId",
                table: "InvoiceAdditionalItems",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDiscounts_Invoices_InvoiceId",
                table: "InvoiceDiscounts",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHolidays_Invoices_InvoiceId",
                table: "InvoiceHolidays",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHolidays_WorkerProfiles_WorkerProfileId",
                table: "InvoiceHolidays",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_CompanyProfiles_CompanyProfileId",
                table: "Invoices",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoicesUSA_CompanyProfiles_CompanyProfileId",
                table: "InvoicesUSA",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceTotals_Invoices_InvoiceId",
                table: "InvoiceTotals",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceTotals_TimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceTotals",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSADiscounts_InvoicesUSA_InvoiceUSAId",
                table: "InvoiceUSADiscounts",
                column: "InvoiceUSAId",
                principalTable: "InvoicesUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSAItems_InvoicesUSA_InvoiceUSAId",
                table: "InvoiceUSAItems",
                column: "InvoiceUSAId",
                principalTable: "InvoicesUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSAItems_TimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceUSAItems",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSATimeSheetTotals_InvoicesUSA_InvoiceUSAId",
                table: "InvoiceUSATimeSheetTotals",
                column: "InvoiceUSAId",
                principalTable: "InvoicesUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSATimeSheetTotals_TimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotals",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Cities_CityId",
                table: "Locations",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationTaxes_Locations_LocationId",
                table: "LocationTaxes",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubItems_PayStubs_PayStubId",
                table: "PayStubItems",
                column: "PayStubId",
                principalTable: "PayStubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubOtherDeductions_PayStubs_PayStubId",
                table: "PayStubOtherDeductions",
                column: "PayStubId",
                principalTable: "PayStubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubPublicHolidays_PayStubs_PayStubId",
                table: "PayStubPublicHolidays",
                column: "PayStubId",
                principalTable: "PayStubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubs_WorkerProfiles_WorkerProfileId",
                table: "PayStubs",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubWageDetails_PayStubs_PayStubId",
                table: "PayStubWageDetails",
                column: "PayStubId",
                principalTable: "PayStubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubWageDetails_TimeSheetTotalPayrolls_TimeSheetTotalId",
                table: "PayStubWageDetails",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotalPayrolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Provinces_Countries_CountryId",
                table: "Provinces",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProvinceSettings_Provinces_ProvinceId",
                table: "ProvinceSettings",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubContractorOtherDeductions_ReportSubcontractors_Rep~",
                table: "ReportSubContractorOtherDeductions",
                column: "ReportSubcontractorId",
                principalTable: "ReportSubcontractors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractorPublicHolidays_ReportSubcontractors_Repo~",
                table: "ReportSubcontractorPublicHolidays",
                column: "ReportSubcontractorId",
                principalTable: "ReportSubcontractors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractors_WorkerProfiles_WorkerProfileId",
                table: "ReportSubcontractors",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractorWageDetails_ReportSubcontractors_ReportS~",
                table: "ReportSubcontractorWageDetails",
                column: "ReportSubcontractorId",
                principalTable: "ReportSubcontractors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractorWageDetails_TimeSheetTotalPayrolls_TimeS~",
                table: "ReportSubcontractorWageDetails",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotalPayrolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApplicants_Candidates_CandidateId",
                table: "RequestApplicants",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApplicants_Requests_RequestId",
                table: "RequestApplicants",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApplicants_WorkerProfiles_WorkerProfileId",
                table: "RequestApplicants",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCancellationDetails_ReasonCancellationRequests_Reaso~",
                table: "RequestCancellationDetails",
                column: "ReasonCancellationRequestId",
                principalTable: "ReasonCancellationRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestComissions_Requests_RequestId",
                table: "RequestComissions",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCompanyUsers_CompanyUsers_CompanyUserId",
                table: "RequestCompanyUsers",
                column: "CompanyUserId",
                principalTable: "CompanyUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCompanyUsers_Requests_RequestId",
                table: "RequestCompanyUsers",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestFinalizationDetails_Requests_RequestId",
                table: "RequestFinalizationDetails",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestNotes_CovenantNotes_NoteId",
                table: "RequestNotes",
                column: "NoteId",
                principalTable: "CovenantNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestNotes_Requests_RequestId",
                table: "RequestNotes",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRecruiters_AgencyPersonnel_RecruiterId",
                table: "RequestRecruiters",
                column: "RecruiterId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRecruiters_Requests_RequestId",
                table: "RequestRecruiters",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestReportTos_CompanyProfileContactPeople_ContactPersonId",
                table: "RequestReportTos",
                column: "ContactPersonId",
                principalTable: "CompanyProfileContactPeople",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestReportTos_Requests_RequestId",
                table: "RequestReportTos",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRequestedBys_CompanyProfileContactPeople_ContactPers~",
                table: "RequestRequestedBys",
                column: "ContactPersonId",
                principalTable: "CompanyProfileContactPeople",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRequestedBys_Requests_RequestId",
                table: "RequestRequestedBys",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_CompanyProfileJobPositionRates_JobPositionRateId",
                table: "Requests",
                column: "JobPositionRateId",
                principalTable: "CompanyProfileJobPositionRates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_CompanyProfiles_CompanyProfileId",
                table: "Requests",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Locations_JobLocationId",
                table: "Requests",
                column: "JobLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Shifts_ShiftId",
                table: "Requests",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSkills_Requests_RequestId",
                table: "RequestSkills",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSources_Requests_RequestId",
                table: "RequestSources",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSources_Sources_SourceId",
                table: "RequestSources",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerInterviews_Users_CreatedBy",
                table: "RunnerInterviews",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerInterviews_Users_RescheduledBy",
                table: "RunnerInterviews",
                column: "RescheduledBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_RequestRecruiters_RequestRecruiterId",
                table: "Runners",
                column: "RequestRecruiterId",
                principalTable: "RequestRecruiters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_Requests_RequestId",
                table: "Runners",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_Users_CreatedBy",
                table: "Runners",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_Users_UpdatedBy",
                table: "Runners",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_WorkerProfiles_WorkerProfileId",
                table: "Runners",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerStatusHistories_Users_ChangedBy",
                table: "RunnerStatusHistories",
                column: "ChangedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheets_WorkerRequests_WorkerRequestId",
                table: "TimeSheets",
                column: "WorkerRequestId",
                principalTable: "WorkerRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheetTotalPayrolls_TimeSheets_TimeSheetId",
                table: "TimeSheetTotalPayrolls",
                column: "TimeSheetId",
                principalTable: "TimeSheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheetTotals_TimeSheets_TimeSheetId",
                table: "TimeSheetTotals",
                column: "TimeSheetId",
                principalTable: "TimeSheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationTypes_NotificationTypes_NotificationTypeId",
                table: "UserNotificationTypes",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationTypes_Users_UserId",
                table: "UserNotificationTypes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComments_CompanyProfiles_CompanyProfileId",
                table: "WorkerComments",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComments_WorkerProfiles_WorkerProfileId",
                table: "WorkerComments",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilities_Availabilities_AvailabilityId",
                table: "WorkerProfileAvailabilities",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilities_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileAvailabilities",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityDays_Days_DayId",
                table: "WorkerProfileAvailabilityDays",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityDays_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileAvailabilityDays",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityTimes_AvailabilityTimes_Availabili~",
                table: "WorkerProfileAvailabilityTimes",
                column: "AvailabilityTimeId",
                principalTable: "AvailabilityTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityTimes_WorkerProfiles_WorkerProfile~",
                table: "WorkerProfileAvailabilityTimes",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileCertificates_CovenantFiles_CertificateId",
                table: "WorkerProfileCertificates",
                column: "CertificateId",
                principalTable: "CovenantFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileCertificates_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileCertificates",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileHolidays_Holidays_HolidayId",
                table: "WorkerProfileHolidays",
                column: "HolidayId",
                principalTable: "Holidays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileHolidays_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileHolidays",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileJobExperiences_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileJobExperiences",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLanguages_Languages_LanguageId",
                table: "WorkerProfileLanguages",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLanguages_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileLanguages",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLicenses_CovenantFiles_LicenseId",
                table: "WorkerProfileLicenses",
                column: "LicenseId",
                principalTable: "CovenantFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLicenses_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileLicenses",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLocationPreferences_Cities_CityId",
                table: "WorkerProfileLocationPreferences",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLocationPreferences_WorkerProfiles_WorkerProfi~",
                table: "WorkerProfileLocationPreferences",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileNotes_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileNotes",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileOtherDocuments_CovenantFiles_DocumentId",
                table: "WorkerProfileOtherDocuments",
                column: "DocumentId",
                principalTable: "CovenantFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileOtherDocuments_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileOtherDocuments",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_Agencies_AgencyId",
                table: "WorkerProfiles",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_IdentificationType1FileId",
                table: "WorkerProfiles",
                column: "IdentificationType1FileId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_IdentificationType2FileId",
                table: "WorkerProfiles",
                column: "IdentificationType2FileId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_PoliceCheckBackGroundId",
                table: "WorkerProfiles",
                column: "PoliceCheckBackGroundId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_ProfileImageId",
                table: "WorkerProfiles",
                column: "ProfileImageId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_ResumeId",
                table: "WorkerProfiles",
                column: "ResumeId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_SocialInsuranceFileId",
                table: "WorkerProfiles",
                column: "SocialInsuranceFileId",
                principalTable: "CovenantFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_Genders_GenderId",
                table: "WorkerProfiles",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_IdentificationTypes_IdentificationType1Id",
                table: "WorkerProfiles",
                column: "IdentificationType1Id",
                principalTable: "IdentificationTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_IdentificationTypes_IdentificationType2Id",
                table: "WorkerProfiles",
                column: "IdentificationType2Id",
                principalTable: "IdentificationTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_Lifts_LiftId",
                table: "WorkerProfiles",
                column: "LiftId",
                principalTable: "Lifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_Locations_LocationId",
                table: "WorkerProfiles",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfiles_Users_WorkerId",
                table: "WorkerProfiles",
                column: "WorkerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileSkills_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileSkills",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileTaxCategories_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileTaxCategories",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequestNotes_CovenantNotes_NoteId",
                table: "WorkerRequestNotes",
                column: "NoteId",
                principalTable: "CovenantNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequestNotes_WorkerRequests_WorkerRequestId",
                table: "WorkerRequestNotes",
                column: "WorkerRequestId",
                principalTable: "WorkerRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequests_Requests_RequestId",
                table: "WorkerRequests",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequests_WorkerProfiles_WorkerProfileId",
                table: "WorkerRequests",
                column: "WorkerProfileId",
                principalTable: "WorkerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_Agencies_AgencyParentId",
                table: "Agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_CovenantFiles_LogoId",
                table: "Agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_Users_UserId",
                table: "Agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyContactInformation_Agencies_AgencyId",
                table: "AgencyContactInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyLocations_Agencies_AgencyId",
                table: "AgencyLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyLocations_Locations_LocationId",
                table: "AgencyLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyPersonnel_Agencies_AgencyId",
                table: "AgencyPersonnel");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyPersonnel_Users_UserId",
                table: "AgencyPersonnel");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyWsibGroups_Agencies_AgencyId",
                table: "AgencyWsibGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_AgencyWsibGroups_WsibGroups_WsibGroupId",
                table: "AgencyWsibGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocuments_Candidates_CandidateId",
                table: "CandidateDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateDocuments_CovenantFiles_DocumentId",
                table: "CandidateDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateNotes_Candidates_CandidateId",
                table: "CandidateNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateNotes_CovenantNotes_NoteId",
                table: "CandidateNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Agencies_AgencyId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Genders_GenderId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provinces_ProvinceId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileContactPeople_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileContactPeople");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileDocuments_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileDocuments_CovenantFiles_DocumentId",
                table: "CompanyProfileDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileIndustries_Industries_IndustryId",
                table: "CompanyProfileIndustries");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileInvoiceNotes_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileInvoiceNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileInvoiceRecipients_CompanyProfiles_CompanyProf~",
                table: "CompanyProfileInvoiceRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileJobPositionRates_CompanyProfiles_CompanyProfi~",
                table: "CompanyProfileJobPositionRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileJobPositionRates_Shifts_ShiftId",
                table: "CompanyProfileJobPositionRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileLocations_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileLocations_Locations_LocationId",
                table: "CompanyProfileLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileNotes_CompanyProfiles_CompanyProfileId",
                table: "CompanyProfileNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfileNotes_CovenantNotes_NoteId",
                table: "CompanyProfileNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_Agencies_AgencyId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_AgencyPersonnel_SalesRepresentativeId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_CompanyProfileIndustries_IndustryId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_CovenantFiles_LogoId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_Users_CompanyId",
                table: "CompanyProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUsers_CompanyProfiles_CompanyProfileId",
                table: "CompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUsers_Users_UserId",
                table: "CompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetails_InvoicesUSA_UsaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalDetails_Invoices_CanadaInvoiceId",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceAdditionalItems_Invoices_InvoiceId",
                table: "InvoiceAdditionalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDiscounts_Invoices_InvoiceId",
                table: "InvoiceDiscounts");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHolidays_Invoices_InvoiceId",
                table: "InvoiceHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceHolidays_WorkerProfiles_WorkerProfileId",
                table: "InvoiceHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_CompanyProfiles_CompanyProfileId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoicesUSA_CompanyProfiles_CompanyProfileId",
                table: "InvoicesUSA");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceTotals_Invoices_InvoiceId",
                table: "InvoiceTotals");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceTotals_TimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceTotals");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSADiscounts_InvoicesUSA_InvoiceUSAId",
                table: "InvoiceUSADiscounts");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSAItems_InvoicesUSA_InvoiceUSAId",
                table: "InvoiceUSAItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSAItems_TimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceUSAItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSATimeSheetTotals_InvoicesUSA_InvoiceUSAId",
                table: "InvoiceUSATimeSheetTotals");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceUSATimeSheetTotals_TimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotals");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Cities_CityId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationTaxes_Locations_LocationId",
                table: "LocationTaxes");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubItems_PayStubs_PayStubId",
                table: "PayStubItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubOtherDeductions_PayStubs_PayStubId",
                table: "PayStubOtherDeductions");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubPublicHolidays_PayStubs_PayStubId",
                table: "PayStubPublicHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubs_WorkerProfiles_WorkerProfileId",
                table: "PayStubs");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubWageDetails_PayStubs_PayStubId",
                table: "PayStubWageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PayStubWageDetails_TimeSheetTotalPayrolls_TimeSheetTotalId",
                table: "PayStubWageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Provinces_Countries_CountryId",
                table: "Provinces");

            migrationBuilder.DropForeignKey(
                name: "FK_ProvinceSettings_Provinces_ProvinceId",
                table: "ProvinceSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubContractorOtherDeductions_ReportSubcontractors_Rep~",
                table: "ReportSubContractorOtherDeductions");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractorPublicHolidays_ReportSubcontractors_Repo~",
                table: "ReportSubcontractorPublicHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractors_WorkerProfiles_WorkerProfileId",
                table: "ReportSubcontractors");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractorWageDetails_ReportSubcontractors_ReportS~",
                table: "ReportSubcontractorWageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportSubcontractorWageDetails_TimeSheetTotalPayrolls_TimeS~",
                table: "ReportSubcontractorWageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApplicants_Candidates_CandidateId",
                table: "RequestApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApplicants_Requests_RequestId",
                table: "RequestApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestApplicants_WorkerProfiles_WorkerProfileId",
                table: "RequestApplicants");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCancellationDetails_ReasonCancellationRequests_Reaso~",
                table: "RequestCancellationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestComissions_Requests_RequestId",
                table: "RequestComissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCompanyUsers_CompanyUsers_CompanyUserId",
                table: "RequestCompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCompanyUsers_Requests_RequestId",
                table: "RequestCompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestFinalizationDetails_Requests_RequestId",
                table: "RequestFinalizationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestNotes_CovenantNotes_NoteId",
                table: "RequestNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestNotes_Requests_RequestId",
                table: "RequestNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRecruiters_AgencyPersonnel_RecruiterId",
                table: "RequestRecruiters");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRecruiters_Requests_RequestId",
                table: "RequestRecruiters");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestReportTos_CompanyProfileContactPeople_ContactPersonId",
                table: "RequestReportTos");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestReportTos_Requests_RequestId",
                table: "RequestReportTos");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRequestedBys_CompanyProfileContactPeople_ContactPers~",
                table: "RequestRequestedBys");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestRequestedBys_Requests_RequestId",
                table: "RequestRequestedBys");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_CompanyProfileJobPositionRates_JobPositionRateId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_CompanyProfiles_CompanyProfileId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Locations_JobLocationId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Shifts_ShiftId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestSkills_Requests_RequestId",
                table: "RequestSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestSources_Requests_RequestId",
                table: "RequestSources");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestSources_Sources_SourceId",
                table: "RequestSources");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerInterviews_Users_CreatedBy",
                table: "RunnerInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerInterviews_Users_RescheduledBy",
                table: "RunnerInterviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_RequestRecruiters_RequestRecruiterId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_Requests_RequestId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_Users_CreatedBy",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_Users_UpdatedBy",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_Runners_WorkerProfiles_WorkerProfileId",
                table: "Runners");

            migrationBuilder.DropForeignKey(
                name: "FK_RunnerStatusHistories_Users_ChangedBy",
                table: "RunnerStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheets_WorkerRequests_WorkerRequestId",
                table: "TimeSheets");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheetTotalPayrolls_TimeSheets_TimeSheetId",
                table: "TimeSheetTotalPayrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSheetTotals_TimeSheets_TimeSheetId",
                table: "TimeSheetTotals");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationTypes_NotificationTypes_NotificationTypeId",
                table: "UserNotificationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotificationTypes_Users_UserId",
                table: "UserNotificationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComments_CompanyProfiles_CompanyProfileId",
                table: "WorkerComments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerComments_WorkerProfiles_WorkerProfileId",
                table: "WorkerComments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilities_Availabilities_AvailabilityId",
                table: "WorkerProfileAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilities_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityDays_Days_DayId",
                table: "WorkerProfileAvailabilityDays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityDays_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileAvailabilityDays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityTimes_AvailabilityTimes_Availabili~",
                table: "WorkerProfileAvailabilityTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileAvailabilityTimes_WorkerProfiles_WorkerProfile~",
                table: "WorkerProfileAvailabilityTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileCertificates_CovenantFiles_CertificateId",
                table: "WorkerProfileCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileCertificates_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileHolidays_Holidays_HolidayId",
                table: "WorkerProfileHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileHolidays_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileJobExperiences_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileJobExperiences");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLanguages_Languages_LanguageId",
                table: "WorkerProfileLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLanguages_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLicenses_CovenantFiles_LicenseId",
                table: "WorkerProfileLicenses");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLicenses_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileLicenses");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLocationPreferences_Cities_CityId",
                table: "WorkerProfileLocationPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileLocationPreferences_WorkerProfiles_WorkerProfi~",
                table: "WorkerProfileLocationPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileNotes_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileOtherDocuments_CovenantFiles_DocumentId",
                table: "WorkerProfileOtherDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileOtherDocuments_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileOtherDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_Agencies_AgencyId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_IdentificationType1FileId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_IdentificationType2FileId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_PoliceCheckBackGroundId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_ProfileImageId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_ResumeId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_CovenantFiles_SocialInsuranceFileId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_Genders_GenderId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_IdentificationTypes_IdentificationType1Id",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_IdentificationTypes_IdentificationType2Id",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_Lifts_LiftId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_Locations_LocationId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfiles_Users_WorkerId",
                table: "WorkerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileSkills_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerProfileTaxCategories_WorkerProfiles_WorkerProfileId",
                table: "WorkerProfileTaxCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequestNotes_CovenantNotes_NoteId",
                table: "WorkerRequestNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequestNotes_WorkerRequests_WorkerRequestId",
                table: "WorkerRequestNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequests_Requests_RequestId",
                table: "WorkerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRequests_WorkerProfiles_WorkerProfileId",
                table: "WorkerRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WsibGroups",
                table: "WsibGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerRequests",
                table: "WorkerRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerRequestNotes",
                table: "WorkerRequestNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileSkills",
                table: "WorkerProfileSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfiles",
                table: "WorkerProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileOtherDocuments",
                table: "WorkerProfileOtherDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileNotes",
                table: "WorkerProfileNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileLocationPreferences",
                table: "WorkerProfileLocationPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileLicenses",
                table: "WorkerProfileLicenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileLanguages",
                table: "WorkerProfileLanguages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileJobExperiences",
                table: "WorkerProfileJobExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileHolidays",
                table: "WorkerProfileHolidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileCertificates",
                table: "WorkerProfileCertificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileAvailabilityTimes",
                table: "WorkerProfileAvailabilityTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileAvailabilityDays",
                table: "WorkerProfileAvailabilityDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerProfileAvailabilities",
                table: "WorkerProfileAvailabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkerComments",
                table: "WorkerComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotificationTypes",
                table: "UserNotificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSheetTotals",
                table: "TimeSheetTotals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSheetTotalPayrolls",
                table: "TimeSheetTotalPayrolls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeSheets",
                table: "TimeSheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkipPayrollNumbers",
                table: "SkipPayrollNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestSources",
                table: "RequestSources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestSkills",
                table: "RequestSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRequestedBys",
                table: "RequestRequestedBys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestReportTos",
                table: "RequestReportTos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRecruiters",
                table: "RequestRecruiters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestNotes",
                table: "RequestNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestFinalizationDetails",
                table: "RequestFinalizationDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestCancellationDetails",
                table: "RequestCancellationDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestApplicants",
                table: "RequestApplicants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubcontractorWageDetails",
                table: "ReportSubcontractorWageDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubcontractors",
                table: "ReportSubcontractors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubcontractorPublicHolidays",
                table: "ReportSubcontractorPublicHolidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportSubContractorOtherDeductions",
                table: "ReportSubContractorOtherDeductions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReasonCancellationRequests",
                table: "ReasonCancellationRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provinces",
                table: "Provinces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubWageDetails",
                table: "PayStubWageDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubs",
                table: "PayStubs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubPublicHolidays",
                table: "PayStubPublicHolidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubOtherDeductions",
                table: "PayStubOtherDeductions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayStubItems",
                table: "PayStubItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTypes",
                table: "NotificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lifts",
                table: "Lifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSATimeSheetTotals",
                table: "InvoiceUSATimeSheetTotals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSAItems",
                table: "InvoiceUSAItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceUSADiscounts",
                table: "InvoiceUSADiscounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceTotals",
                table: "InvoiceTotals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoicesUSA",
                table: "InvoicesUSA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceHolidays",
                table: "InvoiceHolidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDiscounts",
                table: "InvoiceDiscounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceAdditionalItems",
                table: "InvoiceAdditionalItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceAdditionalDetails",
                table: "InvoiceAdditionalDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Industries",
                table: "Industries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentificationTypes",
                table: "IdentificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Holidays",
                table: "Holidays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genders",
                table: "Genders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Days",
                table: "Days");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CovenantNotes",
                table: "CovenantNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CovenantFiles",
                table: "CovenantFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyUsers",
                table: "CompanyUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfiles",
                table: "CompanyProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileNotes",
                table: "CompanyProfileNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileLocations",
                table: "CompanyProfileLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileJobPositionRates",
                table: "CompanyProfileJobPositionRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileInvoiceRecipients",
                table: "CompanyProfileInvoiceRecipients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileIndustries",
                table: "CompanyProfileIndustries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileDocuments",
                table: "CompanyProfileDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyProfileContactPeople",
                table: "CompanyProfileContactPeople");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cities",
                table: "Cities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateNotes",
                table: "CandidateNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidateDocuments",
                table: "CandidateDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AvailabilityTimes",
                table: "AvailabilityTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Availabilities",
                table: "Availabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgencyWsibGroups",
                table: "AgencyWsibGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgencyLocations",
                table: "AgencyLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agencies",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ReasonCancellationRequests");

            migrationBuilder.RenameTable(
                name: "WsibGroups",
                newName: "WsibGroup");

            migrationBuilder.RenameTable(
                name: "WorkerRequests",
                newName: "WorkerRequest");

            migrationBuilder.RenameTable(
                name: "WorkerRequestNotes",
                newName: "WorkerRequestNote");

            migrationBuilder.RenameTable(
                name: "WorkerProfileSkills",
                newName: "WorkerProfileSkill");

            migrationBuilder.RenameTable(
                name: "WorkerProfiles",
                newName: "WorkerProfile");

            migrationBuilder.RenameTable(
                name: "WorkerProfileOtherDocuments",
                newName: "WorkerProfileOtherDocument");

            migrationBuilder.RenameTable(
                name: "WorkerProfileNotes",
                newName: "WorkerProfileNote");

            migrationBuilder.RenameTable(
                name: "WorkerProfileLocationPreferences",
                newName: "WorkerProfileLocationPreference");

            migrationBuilder.RenameTable(
                name: "WorkerProfileLicenses",
                newName: "WorkerProfileLicense");

            migrationBuilder.RenameTable(
                name: "WorkerProfileLanguages",
                newName: "WorkerProfileLanguage");

            migrationBuilder.RenameTable(
                name: "WorkerProfileJobExperiences",
                newName: "WorkerProfileJobExperience");

            migrationBuilder.RenameTable(
                name: "WorkerProfileHolidays",
                newName: "WorkerProfileHoliday");

            migrationBuilder.RenameTable(
                name: "WorkerProfileCertificates",
                newName: "WorkerProfileCertificate");

            migrationBuilder.RenameTable(
                name: "WorkerProfileAvailabilityTimes",
                newName: "WorkerProfileAvailabilityTime");

            migrationBuilder.RenameTable(
                name: "WorkerProfileAvailabilityDays",
                newName: "WorkerProfileAvailabilityDay");

            migrationBuilder.RenameTable(
                name: "WorkerProfileAvailabilities",
                newName: "WorkerProfileAvailability");

            migrationBuilder.RenameTable(
                name: "WorkerComments",
                newName: "WorkerComment");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "UserNotificationTypes",
                newName: "UserNotificationType");

            migrationBuilder.RenameTable(
                name: "TimeSheetTotals",
                newName: "TimeSheetTotal");

            migrationBuilder.RenameTable(
                name: "TimeSheetTotalPayrolls",
                newName: "TimeSheetTotalPayroll");

            migrationBuilder.RenameTable(
                name: "TimeSheets",
                newName: "TimeSheet");

            migrationBuilder.RenameTable(
                name: "SkipPayrollNumbers",
                newName: "SkipPayrollNumber");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "Shift");

            migrationBuilder.RenameTable(
                name: "RequestSources",
                newName: "RequestSource");

            migrationBuilder.RenameTable(
                name: "RequestSkills",
                newName: "RequestSkill");

            migrationBuilder.RenameTable(
                name: "Requests",
                newName: "Request");

            migrationBuilder.RenameTable(
                name: "RequestRequestedBys",
                newName: "RequestRequestedBy");

            migrationBuilder.RenameTable(
                name: "RequestReportTos",
                newName: "RequestReportTo");

            migrationBuilder.RenameTable(
                name: "RequestRecruiters",
                newName: "RequestRecruiter");

            migrationBuilder.RenameTable(
                name: "RequestNotes",
                newName: "RequestNote");

            migrationBuilder.RenameTable(
                name: "RequestFinalizationDetails",
                newName: "RequestFinalizationDetail");

            migrationBuilder.RenameTable(
                name: "RequestCancellationDetails",
                newName: "RequestCancellationDetail");

            migrationBuilder.RenameTable(
                name: "RequestApplicants",
                newName: "RequestApplicant");

            migrationBuilder.RenameTable(
                name: "ReportSubcontractorWageDetails",
                newName: "ReportSubcontractorWageDetail");

            migrationBuilder.RenameTable(
                name: "ReportSubcontractors",
                newName: "ReportSubcontractor");

            migrationBuilder.RenameTable(
                name: "ReportSubcontractorPublicHolidays",
                newName: "ReportSubcontractorPublicHoliday");

            migrationBuilder.RenameTable(
                name: "ReportSubContractorOtherDeductions",
                newName: "ReportSubContractorOtherDeduction");

            migrationBuilder.RenameTable(
                name: "ReasonCancellationRequests",
                newName: "ReasonCancellationRequest");

            migrationBuilder.RenameTable(
                name: "Provinces",
                newName: "Province");

            migrationBuilder.RenameTable(
                name: "PayStubWageDetails",
                newName: "PayStubWageDetail");

            migrationBuilder.RenameTable(
                name: "PayStubs",
                newName: "PayStub");

            migrationBuilder.RenameTable(
                name: "PayStubPublicHolidays",
                newName: "PayStubPublicHoliday");

            migrationBuilder.RenameTable(
                name: "PayStubOtherDeductions",
                newName: "PayStubOtherDeduction");

            migrationBuilder.RenameTable(
                name: "PayStubItems",
                newName: "PayStubItem");

            migrationBuilder.RenameTable(
                name: "NotificationTypes",
                newName: "NotificationType");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "Location");

            migrationBuilder.RenameTable(
                name: "Lifts",
                newName: "Lift");

            migrationBuilder.RenameTable(
                name: "Languages",
                newName: "Language");

            migrationBuilder.RenameTable(
                name: "InvoiceUSATimeSheetTotals",
                newName: "InvoiceUSATimeSheetTotal");

            migrationBuilder.RenameTable(
                name: "InvoiceUSAItems",
                newName: "InvoiceUSAItem");

            migrationBuilder.RenameTable(
                name: "InvoiceUSADiscounts",
                newName: "InvoiceUSADiscount");

            migrationBuilder.RenameTable(
                name: "InvoiceTotals",
                newName: "InvoiceTotal");

            migrationBuilder.RenameTable(
                name: "InvoicesUSA",
                newName: "InvoiceUSA");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameTable(
                name: "InvoiceHolidays",
                newName: "InvoiceHoliday");

            migrationBuilder.RenameTable(
                name: "InvoiceDiscounts",
                newName: "InvoiceDiscount");

            migrationBuilder.RenameTable(
                name: "InvoiceAdditionalItems",
                newName: "InvoiceAdditionalItem");

            migrationBuilder.RenameTable(
                name: "InvoiceAdditionalDetails",
                newName: "InvoiceAdditionalDetail");

            migrationBuilder.RenameTable(
                name: "Industries",
                newName: "Industry");

            migrationBuilder.RenameTable(
                name: "IdentificationTypes",
                newName: "IdentificationType");

            migrationBuilder.RenameTable(
                name: "Holidays",
                newName: "Holiday");

            migrationBuilder.RenameTable(
                name: "Genders",
                newName: "Gender");

            migrationBuilder.RenameTable(
                name: "Days",
                newName: "Day");

            migrationBuilder.RenameTable(
                name: "CovenantNotes",
                newName: "CovenantNote");

            migrationBuilder.RenameTable(
                name: "CovenantFiles",
                newName: "CovenantFile");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "Country");

            migrationBuilder.RenameTable(
                name: "CompanyUsers",
                newName: "CompanyUser");

            migrationBuilder.RenameTable(
                name: "CompanyProfiles",
                newName: "CompanyProfile");

            migrationBuilder.RenameTable(
                name: "CompanyProfileNotes",
                newName: "CompanyProfileNote");

            migrationBuilder.RenameTable(
                name: "CompanyProfileLocations",
                newName: "CompanyProfileLocation");

            migrationBuilder.RenameTable(
                name: "CompanyProfileJobPositionRates",
                newName: "CompanyProfileJobPositionRate");

            migrationBuilder.RenameTable(
                name: "CompanyProfileInvoiceRecipients",
                newName: "CompanyProfileInvoiceRecipient");

            migrationBuilder.RenameTable(
                name: "CompanyProfileIndustries",
                newName: "CompanyProfileIndustry");

            migrationBuilder.RenameTable(
                name: "CompanyProfileDocuments",
                newName: "CompanyProfileDocument");

            migrationBuilder.RenameTable(
                name: "CompanyProfileContactPeople",
                newName: "CompanyProfileContactPerson");

            migrationBuilder.RenameTable(
                name: "Cities",
                newName: "City");

            migrationBuilder.RenameTable(
                name: "CandidateNotes",
                newName: "CandidateNote");

            migrationBuilder.RenameTable(
                name: "CandidateDocuments",
                newName: "CandidateDocument");

            migrationBuilder.RenameTable(
                name: "AvailabilityTimes",
                newName: "AvailabilityTime");

            migrationBuilder.RenameTable(
                name: "Availabilities",
                newName: "Availability");

            migrationBuilder.RenameTable(
                name: "AgencyWsibGroups",
                newName: "AgencyWsibGroup");

            migrationBuilder.RenameTable(
                name: "AgencyLocations",
                newName: "AgencyLocation");

            migrationBuilder.RenameTable(
                name: "Agencies",
                newName: "Agency");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequests_WorkerProfileId",
                table: "WorkerRequest",
                newName: "IX_WorkerRequest_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequests_RequestId_WorkerProfileId",
                table: "WorkerRequest",
                newName: "IX_WorkerRequest_RequestId_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerRequestNotes_NoteId",
                table: "WorkerRequestNote",
                newName: "IX_WorkerRequestNote_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileSkills_WorkerProfileId",
                table: "WorkerProfileSkill",
                newName: "IX_WorkerProfileSkill_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_WorkerId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_SocialInsuranceFileId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_SocialInsuranceFileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_ResumeId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_ProfileImageId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_ProfileImageId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_PoliceCheckBackGroundId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_PoliceCheckBackGroundId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_LocationId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_LiftId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_LiftId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_IdentificationType2Id",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_IdentificationType2Id");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_IdentificationType2FileId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_IdentificationType2FileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_IdentificationType1Id",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_IdentificationType1Id");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_IdentificationType1FileId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_IdentificationType1FileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_GenderId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_GenderId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfiles_AgencyId",
                table: "WorkerProfile",
                newName: "IX_WorkerProfile_AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileOtherDocuments_WorkerProfileId",
                table: "WorkerProfileOtherDocument",
                newName: "IX_WorkerProfileOtherDocument_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileOtherDocuments_DocumentId",
                table: "WorkerProfileOtherDocument",
                newName: "IX_WorkerProfileOtherDocument_DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileNotes_WorkerProfileId",
                table: "WorkerProfileNote",
                newName: "IX_WorkerProfileNote_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLocationPreferences_CityId",
                table: "WorkerProfileLocationPreference",
                newName: "IX_WorkerProfileLocationPreference_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLicenses_WorkerProfileId",
                table: "WorkerProfileLicense",
                newName: "IX_WorkerProfileLicense_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLicenses_LicenseId",
                table: "WorkerProfileLicense",
                newName: "IX_WorkerProfileLicense_LicenseId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileLanguages_LanguageId",
                table: "WorkerProfileLanguage",
                newName: "IX_WorkerProfileLanguage_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileJobExperiences_WorkerProfileId",
                table: "WorkerProfileJobExperience",
                newName: "IX_WorkerProfileJobExperience_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileHolidays_WorkerProfileId_HolidayId",
                table: "WorkerProfileHoliday",
                newName: "IX_WorkerProfileHoliday_WorkerProfileId_HolidayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileHolidays_HolidayId",
                table: "WorkerProfileHoliday",
                newName: "IX_WorkerProfileHoliday_HolidayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileCertificates_WorkerProfileId",
                table: "WorkerProfileCertificate",
                newName: "IX_WorkerProfileCertificate_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileCertificates_CertificateId",
                table: "WorkerProfileCertificate",
                newName: "IX_WorkerProfileCertificate_CertificateId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileAvailabilityTimes_AvailabilityTimeId",
                table: "WorkerProfileAvailabilityTime",
                newName: "IX_WorkerProfileAvailabilityTime_AvailabilityTimeId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileAvailabilityDays_DayId",
                table: "WorkerProfileAvailabilityDay",
                newName: "IX_WorkerProfileAvailabilityDay_DayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerProfileAvailabilities_AvailabilityId",
                table: "WorkerProfileAvailability",
                newName: "IX_WorkerProfileAvailability_AvailabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComments_WorkerProfileId",
                table: "WorkerComment",
                newName: "IX_WorkerComment_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerComments_CompanyProfileId",
                table: "WorkerComment",
                newName: "IX_WorkerComment_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "User",
                newName: "IX_User_Email");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotificationTypes_UserId_NotificationTypeId",
                table: "UserNotificationType",
                newName: "IX_UserNotificationType_UserId_NotificationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotificationTypes_NotificationTypeId",
                table: "UserNotificationType",
                newName: "IX_UserNotificationType_NotificationTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSheetTotals_TimeSheetId",
                table: "TimeSheetTotal",
                newName: "IX_TimeSheetTotal_TimeSheetId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSheetTotalPayrolls_TimeSheetId",
                table: "TimeSheetTotalPayroll",
                newName: "IX_TimeSheetTotalPayroll_TimeSheetId");

            migrationBuilder.RenameIndex(
                name: "IX_TimeSheets_WorkerRequestId",
                table: "TimeSheet",
                newName: "IX_TimeSheet_WorkerRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestSources_SourceId",
                table: "RequestSource",
                newName: "IX_RequestSource_SourceId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestSkills_RequestId",
                table: "RequestSkill",
                newName: "IX_RequestSkill_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_ShiftId",
                table: "Request",
                newName: "IX_Request_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_JobPositionRateId",
                table: "Request",
                newName: "IX_Request_JobPositionRateId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_JobLocationId",
                table: "Request",
                newName: "IX_Request_JobLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_CompanyProfileId",
                table: "Request",
                newName: "IX_Request_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestRequestedBys_ContactPersonId",
                table: "RequestRequestedBy",
                newName: "IX_RequestRequestedBy_ContactPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestReportTos_ContactPersonId",
                table: "RequestReportTo",
                newName: "IX_RequestReportTo_ContactPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestRecruiters_RequestId_RecruiterId_WorkDate",
                table: "RequestRecruiter",
                newName: "IX_RequestRecruiter_RequestId_RecruiterId_WorkDate");

            migrationBuilder.RenameIndex(
                name: "IX_RequestRecruiters_RecruiterId",
                table: "RequestRecruiter",
                newName: "IX_RequestRecruiter_RecruiterId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestNotes_NoteId",
                table: "RequestNote",
                newName: "IX_RequestNote_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestFinalizationDetails_RequestId",
                table: "RequestFinalizationDetail",
                newName: "IX_RequestFinalizationDetail_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestCancellationDetails_ReasonCancellationRequestId",
                table: "RequestCancellationDetail",
                newName: "IX_RequestCancellationDetail_ReasonCancellationRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestApplicants_WorkerProfileId",
                table: "RequestApplicant",
                newName: "IX_RequestApplicant_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestApplicants_RequestId",
                table: "RequestApplicant",
                newName: "IX_RequestApplicant_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestApplicants_CandidateId",
                table: "RequestApplicant",
                newName: "IX_RequestApplicant_CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractorWageDetails_TimeSheetTotalId",
                table: "ReportSubcontractorWageDetail",
                newName: "IX_ReportSubcontractorWageDetail_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractorWageDetails_ReportSubcontractorId",
                table: "ReportSubcontractorWageDetail",
                newName: "IX_ReportSubcontractorWageDetail_ReportSubcontractorId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractors_WorkerProfileId",
                table: "ReportSubcontractor",
                newName: "IX_ReportSubcontractor_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubcontractorPublicHolidays_ReportSubcontractorId",
                table: "ReportSubcontractorPublicHoliday",
                newName: "IX_ReportSubcontractorPublicHoliday_ReportSubcontractorId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportSubContractorOtherDeductions_ReportSubcontractorId",
                table: "ReportSubContractorOtherDeduction",
                newName: "IX_ReportSubContractorOtherDeduction_ReportSubcontractorId");

            migrationBuilder.RenameIndex(
                name: "IX_Provinces_CountryId",
                table: "Province",
                newName: "IX_Province_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubWageDetails_TimeSheetTotalId",
                table: "PayStubWageDetail",
                newName: "IX_PayStubWageDetail_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubWageDetails_PayStubId",
                table: "PayStubWageDetail",
                newName: "IX_PayStubWageDetail_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubs_WorkerProfileId",
                table: "PayStub",
                newName: "IX_PayStub_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubPublicHolidays_PayStubId",
                table: "PayStubPublicHoliday",
                newName: "IX_PayStubPublicHoliday_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubOtherDeductions_PayStubId",
                table: "PayStubOtherDeduction",
                newName: "IX_PayStubOtherDeduction_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_PayStubItems_PayStubId",
                table: "PayStubItem",
                newName: "IX_PayStubItem_PayStubId");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_CityId",
                table: "Location",
                newName: "IX_Location_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSATimeSheetTotals_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotal",
                newName: "IX_InvoiceUSATimeSheetTotal_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSAItems_TimeSheetTotalId",
                table: "InvoiceUSAItem",
                newName: "IX_InvoiceUSAItem_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSAItems_InvoiceUSAId",
                table: "InvoiceUSAItem",
                newName: "IX_InvoiceUSAItem_InvoiceUSAId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceUSADiscounts_InvoiceUSAId",
                table: "InvoiceUSADiscount",
                newName: "IX_InvoiceUSADiscount_InvoiceUSAId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceTotals_TimeSheetTotalId",
                table: "InvoiceTotal",
                newName: "IX_InvoiceTotal_TimeSheetTotalId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceTotals_InvoiceId",
                table: "InvoiceTotal",
                newName: "IX_InvoiceTotal_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoicesUSA_InvoiceNumberId",
                table: "InvoiceUSA",
                newName: "IX_InvoiceUSA_InvoiceNumberId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoicesUSA_InvoiceNumber",
                table: "InvoiceUSA",
                newName: "IX_InvoiceUSA_InvoiceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_InvoicesUSA_CompanyProfileId",
                table: "InvoiceUSA",
                newName: "IX_InvoiceUSA_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_CompanyProfileId",
                table: "Invoice",
                newName: "IX_Invoice_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceHolidays_WorkerProfileId",
                table: "InvoiceHoliday",
                newName: "IX_InvoiceHoliday_WorkerProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceHolidays_InvoiceId",
                table: "InvoiceHoliday",
                newName: "IX_InvoiceHoliday_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDiscounts_InvoiceId",
                table: "InvoiceDiscount",
                newName: "IX_InvoiceDiscount_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceAdditionalItems_InvoiceId",
                table: "InvoiceAdditionalItem",
                newName: "IX_InvoiceAdditionalItem_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceAdditionalDetails_UsaInvoiceId",
                table: "InvoiceAdditionalDetail",
                newName: "IX_InvoiceAdditionalDetail_UsaInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceAdditionalDetails_CanadaInvoiceId",
                table: "InvoiceAdditionalDetail",
                newName: "IX_InvoiceAdditionalDetail_CanadaInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Holidays_Date",
                table: "Holiday",
                newName: "IX_Holiday_Date");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUsers_UserId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUsers_CompanyProfileId_UserId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_CompanyProfileId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfiles_SalesRepresentativeId",
                table: "CompanyProfile",
                newName: "IX_CompanyProfile_SalesRepresentativeId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfiles_LogoId",
                table: "CompanyProfile",
                newName: "IX_CompanyProfile_LogoId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfiles_IndustryId",
                table: "CompanyProfile",
                newName: "IX_CompanyProfile_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfiles_CompanyId",
                table: "CompanyProfile",
                newName: "IX_CompanyProfile_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfiles_AgencyId",
                table: "CompanyProfile",
                newName: "IX_CompanyProfile_AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileNotes_NoteId",
                table: "CompanyProfileNote",
                newName: "IX_CompanyProfileNote_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileLocations_LocationId",
                table: "CompanyProfileLocation",
                newName: "IX_CompanyProfileLocation_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileJobPositionRates_ShiftId",
                table: "CompanyProfileJobPositionRate",
                newName: "IX_CompanyProfileJobPositionRate_ShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileJobPositionRates_CompanyProfileId",
                table: "CompanyProfileJobPositionRate",
                newName: "IX_CompanyProfileJobPositionRate_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileInvoiceRecipients_CompanyProfileId",
                table: "CompanyProfileInvoiceRecipient",
                newName: "IX_CompanyProfileInvoiceRecipient_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileIndustries_IndustryId",
                table: "CompanyProfileIndustry",
                newName: "IX_CompanyProfileIndustry_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileDocuments_CompanyProfileId",
                table: "CompanyProfileDocument",
                newName: "IX_CompanyProfileDocument_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyProfileContactPeople_CompanyProfileId",
                table: "CompanyProfileContactPerson",
                newName: "IX_CompanyProfileContactPerson_CompanyProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_ProvinceId",
                table: "City",
                newName: "IX_City_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateNotes_NoteId",
                table: "CandidateNote",
                newName: "IX_CandidateNote_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateDocuments_DocumentId",
                table: "CandidateDocument",
                newName: "IX_CandidateDocument_DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_AgencyWsibGroups_WsibGroupId",
                table: "AgencyWsibGroup",
                newName: "IX_AgencyWsibGroup_WsibGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_AgencyLocations_LocationId",
                table: "AgencyLocation",
                newName: "IX_AgencyLocation_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Agencies_UserId",
                table: "Agency",
                newName: "IX_Agency_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Agencies_LogoId",
                table: "Agency",
                newName: "IX_Agency_LogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Agencies_AgencyParentId",
                table: "Agency",
                newName: "IX_Agency_AgencyParentId");

            migrationBuilder.AddColumn<long>(
                name: "ValueId",
                table: "ReasonCancellationRequest",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WsibGroup",
                table: "WsibGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerRequest",
                table: "WorkerRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerRequestNote",
                table: "WorkerRequestNote",
                columns: new[] { "WorkerRequestId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileSkill",
                table: "WorkerProfileSkill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfile",
                table: "WorkerProfile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileOtherDocument",
                table: "WorkerProfileOtherDocument",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileNote",
                table: "WorkerProfileNote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileLocationPreference",
                table: "WorkerProfileLocationPreference",
                columns: new[] { "WorkerProfileId", "CityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileLicense",
                table: "WorkerProfileLicense",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileLanguage",
                table: "WorkerProfileLanguage",
                columns: new[] { "WorkerProfileId", "LanguageId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileJobExperience",
                table: "WorkerProfileJobExperience",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileHoliday",
                table: "WorkerProfileHoliday",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileCertificate",
                table: "WorkerProfileCertificate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileAvailabilityTime",
                table: "WorkerProfileAvailabilityTime",
                columns: new[] { "WorkerProfileId", "AvailabilityTimeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileAvailabilityDay",
                table: "WorkerProfileAvailabilityDay",
                columns: new[] { "WorkerProfileId", "DayId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerProfileAvailability",
                table: "WorkerProfileAvailability",
                columns: new[] { "WorkerProfileId", "AvailabilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkerComment",
                table: "WorkerComment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotificationType",
                table: "UserNotificationType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSheetTotal",
                table: "TimeSheetTotal",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSheetTotalPayroll",
                table: "TimeSheetTotalPayroll",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeSheet",
                table: "TimeSheet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkipPayrollNumber",
                table: "SkipPayrollNumber",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shift",
                table: "Shift",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestSource",
                table: "RequestSource",
                columns: new[] { "RequestId", "SourceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestSkill",
                table: "RequestSkill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Request",
                table: "Request",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRequestedBy",
                table: "RequestRequestedBy",
                columns: new[] { "RequestId", "ContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestReportTo",
                table: "RequestReportTo",
                columns: new[] { "RequestId", "ContactPersonId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRecruiter",
                table: "RequestRecruiter",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestNote",
                table: "RequestNote",
                columns: new[] { "RequestId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestFinalizationDetail",
                table: "RequestFinalizationDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestCancellationDetail",
                table: "RequestCancellationDetail",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestApplicant",
                table: "RequestApplicant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubcontractorWageDetail",
                table: "ReportSubcontractorWageDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubcontractor",
                table: "ReportSubcontractor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubcontractorPublicHoliday",
                table: "ReportSubcontractorPublicHoliday",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportSubContractorOtherDeduction",
                table: "ReportSubContractorOtherDeduction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReasonCancellationRequest",
                table: "ReasonCancellationRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Province",
                table: "Province",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubWageDetail",
                table: "PayStubWageDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStub",
                table: "PayStub",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubPublicHoliday",
                table: "PayStubPublicHoliday",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubOtherDeduction",
                table: "PayStubOtherDeduction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayStubItem",
                table: "PayStubItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationType",
                table: "NotificationType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lift",
                table: "Lift",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Language",
                table: "Language",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSATimeSheetTotal",
                table: "InvoiceUSATimeSheetTotal",
                columns: new[] { "InvoiceUSAId", "TimeSheetTotalId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSAItem",
                table: "InvoiceUSAItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSADiscount",
                table: "InvoiceUSADiscount",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceTotal",
                table: "InvoiceTotal",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceUSA",
                table: "InvoiceUSA",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceHoliday",
                table: "InvoiceHoliday",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDiscount",
                table: "InvoiceDiscount",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceAdditionalItem",
                table: "InvoiceAdditionalItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceAdditionalDetail",
                table: "InvoiceAdditionalDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Industry",
                table: "Industry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentificationType",
                table: "IdentificationType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holiday",
                table: "Holiday",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gender",
                table: "Gender",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Day",
                table: "Day",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CovenantNote",
                table: "CovenantNote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CovenantFile",
                table: "CovenantFile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Country",
                table: "Country",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyUser",
                table: "CompanyUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfile",
                table: "CompanyProfile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileNote",
                table: "CompanyProfileNote",
                columns: new[] { "CompanyProfileId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileLocation",
                table: "CompanyProfileLocation",
                columns: new[] { "CompanyProfileId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileJobPositionRate",
                table: "CompanyProfileJobPositionRate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileInvoiceRecipient",
                table: "CompanyProfileInvoiceRecipient",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileIndustry",
                table: "CompanyProfileIndustry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileDocument",
                table: "CompanyProfileDocument",
                columns: new[] { "DocumentId", "CompanyProfileId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyProfileContactPerson",
                table: "CompanyProfileContactPerson",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_City",
                table: "City",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateNote",
                table: "CandidateNote",
                columns: new[] { "CandidateId", "NoteId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidateDocument",
                table: "CandidateDocument",
                columns: new[] { "CandidateId", "DocumentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AvailabilityTime",
                table: "AvailabilityTime",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Availability",
                table: "Availability",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgencyWsibGroup",
                table: "AgencyWsibGroup",
                columns: new[] { "AgencyId", "WsibGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgencyLocation",
                table: "AgencyLocation",
                columns: new[] { "AgencyId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agency",
                table: "Agency",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CompanyProfileHoliday",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    HolidayId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatPaidCompany = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfileHoliday", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfileHoliday_CompanyProfile_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "CompanyProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProfileHoliday_Holiday_HolidayId",
                        column: x => x.HolidayId,
                        principalTable: "Holiday",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StringResource",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    En = table.Column<string>(type: "text", nullable: true),
                    Es = table.Column<string>(type: "text", nullable: true),
                    Fr = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSheetPhoto",
                columns: table => new
                {
                    TimeSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheetPhoto", x => new { x.TimeSheetId, x.PhotoId });
                    table.ForeignKey(
                        name: "FK_TimeSheetPhoto_CovenantFile_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "CovenantFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeSheetPhoto_TimeSheet_TimeSheetId",
                        column: x => x.TimeSheetId,
                        principalTable: "TimeSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReasonCancellationRequest_ValueId",
                table: "ReasonCancellationRequest",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileHoliday_CompanyProfileId",
                table: "CompanyProfileHoliday",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileHoliday_HolidayId",
                table: "CompanyProfileHoliday",
                column: "HolidayId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetPhoto_PhotoId",
                table: "TimeSheetPhoto",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agency_Agency_AgencyParentId",
                table: "Agency",
                column: "AgencyParentId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agency_CovenantFile_LogoId",
                table: "Agency",
                column: "LogoId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agency_User_UserId",
                table: "Agency",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyContactInformation_Agency_AgencyId",
                table: "AgencyContactInformation",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyLocation_Agency_AgencyId",
                table: "AgencyLocation",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyLocation_Location_LocationId",
                table: "AgencyLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyPersonnel_Agency_AgencyId",
                table: "AgencyPersonnel",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyPersonnel_User_UserId",
                table: "AgencyPersonnel",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyWsibGroup_Agency_AgencyId",
                table: "AgencyWsibGroup",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgencyWsibGroup_WsibGroup_WsibGroupId",
                table: "AgencyWsibGroup",
                column: "WsibGroupId",
                principalTable: "WsibGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocument_Candidates_CandidateId",
                table: "CandidateDocument",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateDocument_CovenantFile_DocumentId",
                table: "CandidateDocument",
                column: "DocumentId",
                principalTable: "CovenantFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateNote_Candidates_CandidateId",
                table: "CandidateNote",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateNote_CovenantNote_NoteId",
                table: "CandidateNote",
                column: "NoteId",
                principalTable: "CovenantNote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Agency_AgencyId",
                table: "Candidates",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Gender_GenderId",
                table: "Candidates",
                column: "GenderId",
                principalTable: "Gender",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_City_Province_ProvinceId",
                table: "City",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_AgencyPersonnel_SalesRepresentativeId",
                table: "CompanyProfile",
                column: "SalesRepresentativeId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_Agency_AgencyId",
                table: "CompanyProfile",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_CompanyProfileIndustry_IndustryId",
                table: "CompanyProfile",
                column: "IndustryId",
                principalTable: "CompanyProfileIndustry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_CovenantFile_LogoId",
                table: "CompanyProfile",
                column: "LogoId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfile_User_CompanyId",
                table: "CompanyProfile",
                column: "CompanyId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileContactPerson_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileContactPerson",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileDocument_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileDocument",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileDocument_CovenantFile_DocumentId",
                table: "CompanyProfileDocument",
                column: "DocumentId",
                principalTable: "CovenantFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileIndustry_Industry_IndustryId",
                table: "CompanyProfileIndustry",
                column: "IndustryId",
                principalTable: "Industry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileInvoiceNotes_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileInvoiceNotes",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileInvoiceRecipient_CompanyProfile_CompanyProfil~",
                table: "CompanyProfileInvoiceRecipient",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileJobPositionRate_CompanyProfile_CompanyProfile~",
                table: "CompanyProfileJobPositionRate",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileJobPositionRate_Shift_ShiftId",
                table: "CompanyProfileJobPositionRate",
                column: "ShiftId",
                principalTable: "Shift",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileLocation_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileLocation",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileLocation_Location_LocationId",
                table: "CompanyProfileLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileNote_CompanyProfile_CompanyProfileId",
                table: "CompanyProfileNote",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfileNote_CovenantNote_NoteId",
                table: "CompanyProfileNote",
                column: "NoteId",
                principalTable: "CovenantNote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_CompanyProfile_CompanyProfileId",
                table: "CompanyUser",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_User_UserId",
                table: "CompanyUser",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_CompanyProfile_CompanyProfileId",
                table: "Invoice",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetail_InvoiceUSA_UsaInvoiceId",
                table: "InvoiceAdditionalDetail",
                column: "UsaInvoiceId",
                principalTable: "InvoiceUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalDetail_Invoice_CanadaInvoiceId",
                table: "InvoiceAdditionalDetail",
                column: "CanadaInvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceAdditionalItem_Invoice_InvoiceId",
                table: "InvoiceAdditionalItem",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDiscount_Invoice_InvoiceId",
                table: "InvoiceDiscount",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHoliday_Invoice_InvoiceId",
                table: "InvoiceHoliday",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceHoliday_WorkerProfile_WorkerProfileId",
                table: "InvoiceHoliday",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceTotal_Invoice_InvoiceId",
                table: "InvoiceTotal",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceTotal_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceTotal",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSA_CompanyProfile_CompanyProfileId",
                table: "InvoiceUSA",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSADiscount_InvoiceUSA_InvoiceUSAId",
                table: "InvoiceUSADiscount",
                column: "InvoiceUSAId",
                principalTable: "InvoiceUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSAItem_InvoiceUSA_InvoiceUSAId",
                table: "InvoiceUSAItem",
                column: "InvoiceUSAId",
                principalTable: "InvoiceUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSAItem_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSAItem",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSATimeSheetTotal_InvoiceUSA_InvoiceUSAId",
                table: "InvoiceUSATimeSheetTotal",
                column: "InvoiceUSAId",
                principalTable: "InvoiceUSA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceUSATimeSheetTotal_TimeSheetTotal_TimeSheetTotalId",
                table: "InvoiceUSATimeSheetTotal",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_City_CityId",
                table: "Location",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationTaxes_Location_LocationId",
                table: "LocationTaxes",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStub_WorkerProfile_WorkerProfileId",
                table: "PayStub",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubItem_PayStub_PayStubId",
                table: "PayStubItem",
                column: "PayStubId",
                principalTable: "PayStub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubOtherDeduction_PayStub_PayStubId",
                table: "PayStubOtherDeduction",
                column: "PayStubId",
                principalTable: "PayStub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubPublicHoliday_PayStub_PayStubId",
                table: "PayStubPublicHoliday",
                column: "PayStubId",
                principalTable: "PayStub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubWageDetail_PayStub_PayStubId",
                table: "PayStubWageDetail",
                column: "PayStubId",
                principalTable: "PayStub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayStubWageDetail_TimeSheetTotalPayroll_TimeSheetTotalId",
                table: "PayStubWageDetail",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotalPayroll",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Province_Country_CountryId",
                table: "Province",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProvinceSettings_Province_ProvinceId",
                table: "ProvinceSettings",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReasonCancellationRequest_StringResource_ValueId",
                table: "ReasonCancellationRequest",
                column: "ValueId",
                principalTable: "StringResource",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractor_WorkerProfile_WorkerProfileId",
                table: "ReportSubcontractor",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubContractorOtherDeduction_ReportSubcontractor_Repor~",
                table: "ReportSubContractorOtherDeduction",
                column: "ReportSubcontractorId",
                principalTable: "ReportSubcontractor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractorPublicHoliday_ReportSubcontractor_Report~",
                table: "ReportSubcontractorPublicHoliday",
                column: "ReportSubcontractorId",
                principalTable: "ReportSubcontractor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractorWageDetail_ReportSubcontractor_ReportSub~",
                table: "ReportSubcontractorWageDetail",
                column: "ReportSubcontractorId",
                principalTable: "ReportSubcontractor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportSubcontractorWageDetail_TimeSheetTotalPayroll_TimeShe~",
                table: "ReportSubcontractorWageDetail",
                column: "TimeSheetTotalId",
                principalTable: "TimeSheetTotalPayroll",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_CompanyProfileJobPositionRate_JobPositionRateId",
                table: "Request",
                column: "JobPositionRateId",
                principalTable: "CompanyProfileJobPositionRate",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_CompanyProfile_CompanyProfileId",
                table: "Request",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Location_JobLocationId",
                table: "Request",
                column: "JobLocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Shift_ShiftId",
                table: "Request",
                column: "ShiftId",
                principalTable: "Shift",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApplicant_Candidates_CandidateId",
                table: "RequestApplicant",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApplicant_Request_RequestId",
                table: "RequestApplicant",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestApplicant_WorkerProfile_WorkerProfileId",
                table: "RequestApplicant",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCancellationDetail_ReasonCancellationRequest_ReasonC~",
                table: "RequestCancellationDetail",
                column: "ReasonCancellationRequestId",
                principalTable: "ReasonCancellationRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestComissions_Request_RequestId",
                table: "RequestComissions",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCompanyUsers_CompanyUser_CompanyUserId",
                table: "RequestCompanyUsers",
                column: "CompanyUserId",
                principalTable: "CompanyUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCompanyUsers_Request_RequestId",
                table: "RequestCompanyUsers",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestFinalizationDetail_Request_RequestId",
                table: "RequestFinalizationDetail",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestNote_CovenantNote_NoteId",
                table: "RequestNote",
                column: "NoteId",
                principalTable: "CovenantNote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestNote_Request_RequestId",
                table: "RequestNote",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRecruiter_AgencyPersonnel_RecruiterId",
                table: "RequestRecruiter",
                column: "RecruiterId",
                principalTable: "AgencyPersonnel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRecruiter_Request_RequestId",
                table: "RequestRecruiter",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestReportTo_CompanyProfileContactPerson_ContactPersonId",
                table: "RequestReportTo",
                column: "ContactPersonId",
                principalTable: "CompanyProfileContactPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestReportTo_Request_RequestId",
                table: "RequestReportTo",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRequestedBy_CompanyProfileContactPerson_ContactPerso~",
                table: "RequestRequestedBy",
                column: "ContactPersonId",
                principalTable: "CompanyProfileContactPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRequestedBy_Request_RequestId",
                table: "RequestRequestedBy",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSkill_Request_RequestId",
                table: "RequestSkill",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSource_Request_RequestId",
                table: "RequestSource",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSource_Sources_SourceId",
                table: "RequestSource",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerInterviews_User_CreatedBy",
                table: "RunnerInterviews",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerInterviews_User_RescheduledBy",
                table: "RunnerInterviews",
                column: "RescheduledBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_RequestRecruiter_RequestRecruiterId",
                table: "Runners",
                column: "RequestRecruiterId",
                principalTable: "RequestRecruiter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_Request_RequestId",
                table: "Runners",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_User_CreatedBy",
                table: "Runners",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_User_UpdatedBy",
                table: "Runners",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runners_WorkerProfile_WorkerProfileId",
                table: "Runners",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RunnerStatusHistories_User_ChangedBy",
                table: "RunnerStatusHistories",
                column: "ChangedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheet_WorkerRequest_WorkerRequestId",
                table: "TimeSheet",
                column: "WorkerRequestId",
                principalTable: "WorkerRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheetTotal_TimeSheet_TimeSheetId",
                table: "TimeSheetTotal",
                column: "TimeSheetId",
                principalTable: "TimeSheet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSheetTotalPayroll_TimeSheet_TimeSheetId",
                table: "TimeSheetTotalPayroll",
                column: "TimeSheetId",
                principalTable: "TimeSheet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationType_NotificationType_NotificationTypeId",
                table: "UserNotificationType",
                column: "NotificationTypeId",
                principalTable: "NotificationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotificationType_User_UserId",
                table: "UserNotificationType",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_CompanyProfile_CompanyProfileId",
                table: "WorkerComment",
                column: "CompanyProfileId",
                principalTable: "CompanyProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerComment_WorkerProfile_WorkerProfileId",
                table: "WorkerComment",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_Agency_AgencyId",
                table: "WorkerProfile",
                column: "AgencyId",
                principalTable: "Agency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_CovenantFile_IdentificationType1FileId",
                table: "WorkerProfile",
                column: "IdentificationType1FileId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_CovenantFile_IdentificationType2FileId",
                table: "WorkerProfile",
                column: "IdentificationType2FileId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_CovenantFile_PoliceCheckBackGroundId",
                table: "WorkerProfile",
                column: "PoliceCheckBackGroundId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_CovenantFile_ProfileImageId",
                table: "WorkerProfile",
                column: "ProfileImageId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_CovenantFile_ResumeId",
                table: "WorkerProfile",
                column: "ResumeId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_CovenantFile_SocialInsuranceFileId",
                table: "WorkerProfile",
                column: "SocialInsuranceFileId",
                principalTable: "CovenantFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_Gender_GenderId",
                table: "WorkerProfile",
                column: "GenderId",
                principalTable: "Gender",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_IdentificationType_IdentificationType1Id",
                table: "WorkerProfile",
                column: "IdentificationType1Id",
                principalTable: "IdentificationType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_IdentificationType_IdentificationType2Id",
                table: "WorkerProfile",
                column: "IdentificationType2Id",
                principalTable: "IdentificationType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_Lift_LiftId",
                table: "WorkerProfile",
                column: "LiftId",
                principalTable: "Lift",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_Location_LocationId",
                table: "WorkerProfile",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfile_User_WorkerId",
                table: "WorkerProfile",
                column: "WorkerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailability_Availability_AvailabilityId",
                table: "WorkerProfileAvailability",
                column: "AvailabilityId",
                principalTable: "Availability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailability_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileAvailability",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityDay_Day_DayId",
                table: "WorkerProfileAvailabilityDay",
                column: "DayId",
                principalTable: "Day",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityDay_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileAvailabilityDay",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityTime_AvailabilityTime_Availability~",
                table: "WorkerProfileAvailabilityTime",
                column: "AvailabilityTimeId",
                principalTable: "AvailabilityTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileAvailabilityTime_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileAvailabilityTime",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileCertificate_CovenantFile_CertificateId",
                table: "WorkerProfileCertificate",
                column: "CertificateId",
                principalTable: "CovenantFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileCertificate_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileCertificate",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileHoliday_Holiday_HolidayId",
                table: "WorkerProfileHoliday",
                column: "HolidayId",
                principalTable: "Holiday",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileHoliday_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileHoliday",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileJobExperience_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileJobExperience",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLanguage_Language_LanguageId",
                table: "WorkerProfileLanguage",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLanguage_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileLanguage",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLicense_CovenantFile_LicenseId",
                table: "WorkerProfileLicense",
                column: "LicenseId",
                principalTable: "CovenantFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLicense_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileLicense",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLocationPreference_City_CityId",
                table: "WorkerProfileLocationPreference",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileLocationPreference_WorkerProfile_WorkerProfile~",
                table: "WorkerProfileLocationPreference",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileNote_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileNote",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileOtherDocument_CovenantFile_DocumentId",
                table: "WorkerProfileOtherDocument",
                column: "DocumentId",
                principalTable: "CovenantFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileOtherDocument_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileOtherDocument",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileSkill_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileSkill",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerProfileTaxCategories_WorkerProfile_WorkerProfileId",
                table: "WorkerProfileTaxCategories",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequest_Request_RequestId",
                table: "WorkerRequest",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequest_WorkerProfile_WorkerProfileId",
                table: "WorkerRequest",
                column: "WorkerProfileId",
                principalTable: "WorkerProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequestNote_CovenantNote_NoteId",
                table: "WorkerRequestNote",
                column: "NoteId",
                principalTable: "CovenantNote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRequestNote_WorkerRequest_WorkerRequestId",
                table: "WorkerRequestNote",
                column: "WorkerRequestId",
                principalTable: "WorkerRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
