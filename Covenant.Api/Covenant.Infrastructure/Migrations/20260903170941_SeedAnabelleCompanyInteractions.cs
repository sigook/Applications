using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Covenant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAnabelleCompanyInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO "CompanyInteractions" (
                    "Id", "CompanyProfileId", "UserId", "Description",
                    "InteractionPurpose", "InteractionType", "InteractionStatus",
                    "CreatedAt", "UpdatedAt")
                SELECT v.id, v.company_profile_id, v.user_id, v.description,
                       v.purpose, v.type, v.status, v.created_at, v.created_at
                FROM (VALUES
                    ('0cd32b20-82b1-4baa-b869-2e4a960853c7'::uuid, 'a96eda08-9355-4354-a019-a33538f4a7a1'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Email with Kathy 08/27 - asked for opportunity to meet', 1, 1, 1, timestamptz '2026-08-31 09:56:38.954-04:00'),
                    ('aa81eee0-b228-4959-80ab-88caaf4b20e0'::uuid, 'a96eda08-9355-4354-a019-a33538f4a7a1'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Send introductory email to the buying team as per Kathryn in reception Production Manager- FUP in 3 business days', 0, 1, 1, timestamptz '2026-08-06 20:51:59.195-04:00'),
                    ('6bb1f9d3-436d-44d2-aec6-421ee3593a08'::uuid, 'faf7c2a8-4b2f-4aa1-bdf1-3d9a30eaf763'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke no Shannon- sent package 8/10.- they have an agency. Do not overwhelm her', 0, 0, 2, timestamptz '2026-08-11 20:00:28.891-04:00'),
                    ('11b8a5ef-2f3d-4124-927a-9b057ba8120d'::uuid, '5cf8549d-5598-44ba-a87a-94ea2c667353'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke to Maria send email about covenant ASAP', 0, 0, 1, timestamptz '2026-08-10 17:03:26.039-04:00'),
                    ('98bb798f-6236-4c33-aa20-f3bd8910f271'::uuid, 'fbfbf5ee-63d4-4712-8fef-aed339f4b97b'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Left message for Michael in HR - press 3 in switchboard -', 0, 0, 1, timestamptz '2026-08-11 16:11:00.105-04:00'),
                    ('2cd6e39b-4f3f-45ff-a7cd-b9786879cf8a'::uuid, 'c4c11162-a588-4eb9-8336-17132bc4c19a'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke with Julian at the reception person in charge of hiring or production is Nick today is August 10 call in two weeks when he comes back from vacation', 0, 0, 1, timestamptz '2026-08-11 20:14:53.593-04:00'),
                    ('6e4232e2-685d-4b7c-9871-1ea9ff059ccc'::uuid, 'a62cb374-a320-4130-986a-de72ca0b5eab'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'FUP in 08/10 - call again this week', 1, 0, 1, timestamptz '2026-08-10 16:39:31.157-04:00'),
                    ('78b54b16-7269-4bf9-8e0a-33393404fa8b'::uuid, 'a62cb374-a320-4130-986a-de72ca0b5eab'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'site visit. Reception is Lenny\n\nOprations manager Mujtaba Kanchwala exit 227 referred me to HR - name to contact is Murtaze ext 271 - no voicemail left try again for live call and email address', 0, 0, 1, timestamptz '2026-07-22 18:04:50.925-04:00'),
                    ('e4328f43-3684-4f98-8fde-49d653d371fe'::uuid, 'e39ad729-8b18-40e1-81e4-3beedfe43df5'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke with Cassandra - reception - referred me to Betty Production Manager, listed as CEO in website - left voicemail mail, follow up in 24 hrs - 5/7', 0, 0, 1, timestamptz '2026-08-06 20:40:35.155-04:00'),
                    ('23f86082-af7d-45cf-9a56-770033e8666d'::uuid, '623c60b6-2084-410c-8529-ed95a5834036'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Left long voicemail for Paul extension 1195 reintroducing myself to strengthen relationship with client. Follow up in 3 days', 1, 0, 1, timestamptz '2026-07-22 17:29:19.166-04:00'),
                    ('e11c8faf-2d3e-41b9-8b3e-9ef61db3b965'::uuid, '47afec78-aff7-46a2-a526-d9dffeb710c3'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke with John 07/24 & 07/30 to follow up on email sent . Referred me to Chouinard Brothers for staffing solutions', 1, 0, 2, timestamptz '2026-07-30 18:53:56.778-04:00'),
                    ('6e946399-c14d-4e40-a778-a9401f4bb5c4'::uuid, 'c99713dc-7557-4934-918a-c66bfbe66a2f'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Anuradha advised 0721 she is in no need for help at the moment. Follow up in 3 months', 1, 0, 2, timestamptz '2026-07-23 17:28:16.032-04:00'),
                    ('a674e81b-e1ac-4041-8db7-a441209d1539'::uuid, 'c99713dc-7557-4934-918a-c66bfbe66a2f'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'FUP on last weeks email. Requested meeting with Anuradha to learn about operations. FUP in 5 days', 1, 1, 1, timestamptz '2026-07-22 17:47:56.668-04:00'),
                    ('93031315-fd5a-4d0d-be43-55f19727fd50'::uuid, 'b25a7609-0081-4a58-abcd-f1207eb76c75'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Legacy contact is Kuldip Sighn - Spoke to Sean at the Vaughan location and introduced me to Nelson at the Gordon McKay store.\n\nManager is Olivier for the Weston locations and Zach for the Scarborough location\n—\nMet with merchandise manager and he asked about sales personnel and drivers - offered presentation within the next week', 0, 0, 1, timestamptz '2026-08-06 18:21:36.668-04:00'),
                    ('5732d9ef-0d3d-4f2a-8778-90069a983e6a'::uuid, 'cde4c600-266b-48e1-b31e-7d6f57e90f8f'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Called Jennifer to FUP on email last week - no answer, voicemail left', 1, 0, 1, timestamptz '2026-08-10 16:43:56.176-04:00'),
                    ('82a0880f-7e25-41b3-a83d-f3e1cb845a50'::uuid, 'e257fdbe-f016-46d1-8d81-4ee6153106a6'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Sent roofing flyer to Mike - called to his direct cell phone line 07/30', 1, 1, 1, timestamptz '2026-07-30 19:24:07.605-04:00'),
                    ('f88fa756-6214-4bf5-9edd-510a18df603a'::uuid, 'e257fdbe-f016-46d1-8d81-4ee6153106a6'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke to Carrie. Got an email - email sent to Mike (owner) and sales dept - follow up in 3 days and call cell phone next week - do not jump channels', 1, 1, 1, timestamptz '2026-07-23 18:34:37.835-04:00'),
                    ('f50d3550-3256-4a01-97d2-f4caf4f9c8bb'::uuid, 'e257fdbe-f016-46d1-8d81-4ee6153106a6'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Outreach began - spoke with Leah. Mike will give me a call back', 0, 0, 1, timestamptz '2026-07-22 18:37:14.717-04:00'),
                    ('16559c4a-7f1c-42c6-9d97-1c28e30d7c34'::uuid, '3602d92c-8ed4-4b7c-a1a6-587697b7cd05'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke to Nick - 08/06 - no need for driver at the moment - nurture', 0, 0, 2, timestamptz '2026-08-06 18:44:21.477-04:00'),
                    ('eacd4126-2dac-4cd9-9f76-c3b7875902b6'::uuid, '57236909-1329-4643-bb95-409c9248fbe8'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Proposal sent - 08/31 - under review', 2, 1, 1, timestamptz '2026-09-03 15:07:42.548-04:00'),
                    ('e998b911-f00c-4f94-967c-574bc9935807'::uuid, '57236909-1329-4643-bb95-409c9248fbe8'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Andrei Agreed on new terms as of 08/28 - prepare proposal and send by 08/31', 3, 0, 1, timestamptz '2026-08-31 09:50:26.583-04:00'),
                    ('5de160a5-9d7f-4982-b0c1-43ec4c40059f'::uuid, '57236909-1329-4643-bb95-409c9248fbe8'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Contact: Andrei\n* Opportunity: Recruitment for a Salesperson / Project Advisor\n* Current situation: SOSNA already has another recruitment agency actively working on the same position. Andrei is unwilling to end that relationship or grant Covenant exclusivity.\n* Covenant proposal: Retainer waived; 20% success-based professional fee, payable only if SOSNA hires a Covenant-introduced candidate.\n* Main objection: The proposed 30-day exclusivity clause conflicts with SOSNA’s existing agency relationship.\n* Covenant position: Working without both a retainer and exclusivity creates delivery risk. The 20% professional fee remains firm.\n* Potential solution: Non-exclusive contingency search with clear candidate ownership, timely client feedback, and Covenant’s fee payable if a Covenant-introduced candidate is hired.\n* Current status: Interested, but engagement terms remain unresolved.\n* Next action: Schedule a brief call with Andrei to clarify candidate protection, process expectations, and fee structure before preparing/finalizing the proposal.', 3, 0, 1, timestamptz '2026-08-27 14:21:56.036-04:00'),
                    ('f92f333b-6a77-4ecd-a3e0-eabdbe1cdb44'::uuid, '4a77dc26-34b7-462b-8f9c-06eed68763a0'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'New order for forklift drivers as of 08/25 - interviews held on 08/30, email to Jason -  FUP on 08/31', 4, 1, 1, timestamptz '2026-08-31 09:55:02.099-04:00'),
                    ('cdaa48f8-625c-4033-8e7f-66652a93b6dd'::uuid, '4a77dc26-34b7-462b-8f9c-06eed68763a0'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Met with Jason Penner 08/17. Spoke about business opportunities and introduced Covenant new services. Introduced me to Chris and Jonathan and spoke about the possibility to re-start to engage Covenant in small projects. Followed up via email on 08/19 to recap conversation', 1, 0, 2, timestamptz '2026-08-27 13:57:37.719-04:00'),
                    ('54ae3731-8d4c-4120-980a-32d45bd7a9d3'::uuid, '4a77dc26-34b7-462b-8f9c-06eed68763a0'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Spoke to Jason Penner 08/06 - will meet next week set up date and time on Tuesday 08/11 - reminded added to calendar', 1, 0, 1, timestamptz '2026-08-06 18:45:06.695-04:00'),
                    ('a129e611-eb49-4750-a19a-d8605137572c'::uuid, '693f14ac-fa31-41f9-bcd7-e46b649097d9'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Sent introductory email FUP in 3 days', 0, 1, 1, timestamptz '2026-08-11 19:10:52.096-04:00'),
                    ('84ca77de-1aaf-414f-b8eb-d0d0ba1d5bd5'::uuid, 'a59d9886-95ac-40ba-bb5f-3f7ac3317604'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Made contact with Jacqueline - asked for proposal ASAP', 0, 0, 1, timestamptz '2026-09-03 15:21:58.497-04:00'),
                    ('dce060c5-7df9-49ab-b544-24261bdc1bdb'::uuid, '268bbad1-e178-4f3a-9644-47e7e7b65615'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'FUP on 08/28 - no response', 1, 0, 1, timestamptz '2026-08-31 09:50:47.620-04:00'),
                    ('0d5772f7-03c4-4751-9e78-3aa936ee4866'::uuid, '268bbad1-e178-4f3a-9644-47e7e7b65615'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Sent full proposal- 3 hiring models - 0807 - FUP monday 0810 - sent email and call to get feedback', 2, 1, 1, timestamptz '2026-08-10 16:37:00.658-04:00'),
                    ('c7653f39-9f30-4f91-b0ef-703ac042605b'::uuid, '268bbad1-e178-4f3a-9644-47e7e7b65615'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'08/06 - Ali requested terms and pricing - discovery sent - needs worker by Monday 08/10 AM', 2, 1, 1, timestamptz '2026-08-06 19:32:54.844-04:00'),
                    ('d30b0e48-8479-44d0-a514-61afd29de11f'::uuid, '268bbad1-e178-4f3a-9644-47e7e7b65615'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'Sent proposal to Ali 08/05 - followed up on 08/06 - email and phone number', 2, 1, 1, timestamptz '2026-08-06 18:45:40.118-04:00'),
                    ('d76c9610-5a28-4678-aa5c-54408cf4e2d4'::uuid, '03f0017a-263a-4300-8b0e-38b2ac76990b'::uuid, '3d409fd3-cb57-4dd3-8240-f3519e93f3b3'::uuid, E'New contact is Earl D’Souza - sent email to reconnect and land a site visit FUP in 3 days', 0, 1, 1, timestamptz '2026-07-22 19:47:36.739-04:00')
                ) AS v (id, company_profile_id, user_id, description, purpose, type, status, created_at)
                WHERE EXISTS (SELECT 1 FROM "CompanyProfiles" cp WHERE cp."Id" = v.company_profile_id)
                  AND EXISTS (SELECT 1 FROM "Users" u WHERE u."Id" = v.user_id)
                  AND NOT EXISTS (SELECT 1 FROM "CompanyInteractions" ci WHERE ci."Id" = v.id);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "CompanyInteractions"
                WHERE "Id" IN (
                    '0cd32b20-82b1-4baa-b869-2e4a960853c7'::uuid,
                    'aa81eee0-b228-4959-80ab-88caaf4b20e0'::uuid,
                    '6bb1f9d3-436d-44d2-aec6-421ee3593a08'::uuid,
                    '11b8a5ef-2f3d-4124-927a-9b057ba8120d'::uuid,
                    '98bb798f-6236-4c33-aa20-f3bd8910f271'::uuid,
                    '2cd6e39b-4f3f-45ff-a7cd-b9786879cf8a'::uuid,
                    '6e4232e2-685d-4b7c-9871-1ea9ff059ccc'::uuid,
                    '78b54b16-7269-4bf9-8e0a-33393404fa8b'::uuid,
                    'e4328f43-3684-4f98-8fde-49d653d371fe'::uuid,
                    '23f86082-af7d-45cf-9a56-770033e8666d'::uuid,
                    'e11c8faf-2d3e-41b9-8b3e-9ef61db3b965'::uuid,
                    '6e946399-c14d-4e40-a778-a9401f4bb5c4'::uuid,
                    'a674e81b-e1ac-4041-8db7-a441209d1539'::uuid,
                    '93031315-fd5a-4d0d-be43-55f19727fd50'::uuid,
                    '5732d9ef-0d3d-4f2a-8778-90069a983e6a'::uuid,
                    '82a0880f-7e25-41b3-a83d-f3e1cb845a50'::uuid,
                    'f88fa756-6214-4bf5-9edd-510a18df603a'::uuid,
                    'f50d3550-3256-4a01-97d2-f4caf4f9c8bb'::uuid,
                    '16559c4a-7f1c-42c6-9d97-1c28e30d7c34'::uuid,
                    'eacd4126-2dac-4cd9-9f76-c3b7875902b6'::uuid,
                    'e998b911-f00c-4f94-967c-574bc9935807'::uuid,
                    '5de160a5-9d7f-4982-b0c1-43ec4c40059f'::uuid,
                    'f92f333b-6a77-4ecd-a3e0-eabdbe1cdb44'::uuid,
                    'cdaa48f8-625c-4033-8e7f-66652a93b6dd'::uuid,
                    '54ae3731-8d4c-4120-980a-32d45bd7a9d3'::uuid,
                    'a129e611-eb49-4750-a19a-d8605137572c'::uuid,
                    '84ca77de-1aaf-414f-b8eb-d0d0ba1d5bd5'::uuid,
                    'dce060c5-7df9-49ab-b544-24261bdc1bdb'::uuid,
                    '0d5772f7-03c4-4751-9e78-3aa936ee4866'::uuid,
                    'c7653f39-9f30-4f91-b0ef-703ac042605b'::uuid,
                    'd30b0e48-8479-44d0-a514-61afd29de11f'::uuid,
                    'd76c9610-5a28-4678-aa5c-54408cf4e2d4'::uuid
                );
                """);
        }
    }
}
