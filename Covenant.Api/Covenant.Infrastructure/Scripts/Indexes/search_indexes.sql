CREATE INDEX IF NOT EXISTS "IX_Candidates_Name_Search" ON "Candidates" USING gin (unaccent_lower("Name") gin_trgm_ops);
CREATE INDEX IF NOT EXISTS "IX_Candidates_Email_Search" ON "Candidates" USING gin (unaccent_lower("Email") gin_trgm_ops);
CREATE INDEX IF NOT EXISTS "IX_CompanyProfiles_FullName_Search" ON "CompanyProfiles" USING gin (unaccent_lower("FullName") gin_trgm_ops);
CREATE INDEX IF NOT EXISTS "IX_Users_Email_Lower" ON "Users" (lower("Email"));
CREATE INDEX IF NOT EXISTS "IX_Candidates_Email_Lower" ON "Candidates" (lower("Email"));
