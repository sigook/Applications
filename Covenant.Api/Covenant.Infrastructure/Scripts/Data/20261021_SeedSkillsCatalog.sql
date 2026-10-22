INSERT INTO "Skills" ("Value", "Aliases") VALUES
('Accounting', ARRAY['accountant']),
('Administration', ARRAY['admin']),
('Administration Assistant', ARRAY['admin assistant', 'administrative assistant']),
('Administrative', ARRAY[]::text[]),
('Assembly', ARRAY['assembler', 'assembly line']),
('Az Driver', ARRAY['az', 'az truck driver', 'az licence', 'az license']),
('Bakery', ARRAY['baker']),
('Cleaning', ARRAY['cleaner', 'janitor', 'janitorial', 'industrial cleaning']),
('Clerical', ARRAY['clerk']),
('Computer', ARRAY['computer skills', 'computer literacy', 'computers']),
('Construction', ARRAY['construction worker']),
('Counter Balance', ARRAY['counterbalance', 'cb', 'counter balance forklift', 'counterbalance forklift']),
('Customer Service', ARRAY['customer service representative', 'csr']),
('Data Analytics', ARRAY['data analysis']),
('Data Entry', ARRAY['data entry clerk']),
('Database', ARRAY['databases']),
('Delivery', ARRAY['deliveries']),
('Dispatcher', ARRAY['dispatch']),
('Driver', ARRAY['driving']),
('Electrician', ARRAY['electrical']),
('Food Service', ARRAY['food preparation', 'food services']),
('Forklift', ARRAY['forklift operator', 'forklift driver', 'forklift experience', 'forklift certified', 'lift truck']),
('General Labour', ARRAY['general labor', 'general laborer', 'general labourer', 'labor', 'labour', 'general', 'gl']),
('General Packer', ARRAY['packer']),
('Heavy Lifting', ARRAY['heavy lifting fit']),
('Inventory', ARRAY['inventory management', 'inventory control']),
('Loading/Unloading', ARRAY['loading & unloading', 'loading and unloading', 'loading', 'unloading']),
('Logistics', ARRAY[]::text[]),
('Lumper', ARRAY['lumping']),
('Machine Builder', ARRAY[]::text[]),
('Machine Operator', ARRAY['machine operation', 'machine operating']),
('Material Handling', ARRAY['material handler']),
('Mixer', ARRAY[]::text[]),
('Order Picking', ARRAY['order picker', 'picking', 'picker', 'general order picker']),
('Packing', ARRAY['packaging']),
('Pallet Jack', ARRAY['walkie', 'electric pallet jack', 'walkie rider']),
('Plumbing', ARRAY['plumber']),
('Reach Truck', ARRAY['reach', 'raymond reach', 'double deep reach', 'reach truck operator']),
('Reception', ARRAY['receptionist']),
('Recruiting', ARRAY['recruiter', 'recruitment']),
('RF Scanning', ARRAY['rf scanner', 'rf', 'scanning']),
('Sanitation', ARRAY['sanitizing']),
('Shipping/Receiving', ARRAY['shipping and receiving', 'shipping & receiving', 'shipping receiving', 'shipping', 'receiving', 'shipper', 'receiver']),
('Soldering', ARRAY['solder']),
('Ticketing', ARRAY[]::text[]),
('Training', ARRAY[]::text[]),
('Truck Driver', ARRAY['truck driving']),
('Warehouse', ARRAY['warehouse associate', 'warehouse worker', 'warehouse experience', 'warehouse general labor', 'warehouse operations', 'warehouse clerk']),
('Welding', ARRAY['welder'])
ON CONFLICT ("Value") DO UPDATE SET "Aliases" = EXCLUDED."Aliases";

CREATE TEMP TABLE skill_map ON COMMIT DROP AS
SELECT unaccent_lower("Value") AS key, "Value" AS value FROM "Skills"
UNION
SELECT unnest("Aliases"), "Value" FROM "Skills";

UPDATE "WorkerProfileSkills" SET "Skill" = regexp_replace(trim("Skill"), '\s+', ' ', 'g')
WHERE "Skill" <> regexp_replace(trim("Skill"), '\s+', ' ', 'g');
UPDATE "WorkerProfileSkills" t SET "Skill" = m.value FROM skill_map m
WHERE unaccent_lower(t."Skill") = m.key AND t."Skill" <> m.value;
DELETE FROM "WorkerProfileSkills"
WHERE "Skill" IS NULL OR length("Skill") < 2 OR unaccent_lower("Skill") IN ('na', 'n/a', 'none');
DELETE FROM "WorkerProfileSkills" t USING "WorkerProfileSkills" d
WHERE t."WorkerProfileId" = d."WorkerProfileId" AND unaccent_lower(t."Skill") = unaccent_lower(d."Skill") AND t."Id" > d."Id";

UPDATE "CandidateSkills" SET "Skill" = regexp_replace(trim("Skill"), '\s+', ' ', 'g')
WHERE "Skill" <> regexp_replace(trim("Skill"), '\s+', ' ', 'g');
UPDATE "CandidateSkills" t SET "Skill" = m.value FROM skill_map m
WHERE unaccent_lower(t."Skill") = m.key AND t."Skill" <> m.value;
DELETE FROM "CandidateSkills"
WHERE "Skill" IS NULL OR length("Skill") < 2 OR unaccent_lower("Skill") IN ('na', 'n/a', 'none');
DELETE FROM "CandidateSkills" t USING "CandidateSkills" d
WHERE t."CandidateId" = d."CandidateId" AND unaccent_lower(t."Skill") = unaccent_lower(d."Skill") AND t."Id" > d."Id";

UPDATE "RequestSkills" SET "Skill" = regexp_replace(trim("Skill"), '\s+', ' ', 'g')
WHERE "Skill" <> regexp_replace(trim("Skill"), '\s+', ' ', 'g');
UPDATE "RequestSkills" t SET "Skill" = m.value FROM skill_map m
WHERE unaccent_lower(t."Skill") = m.key AND t."Skill" <> m.value;
DELETE FROM "RequestSkills"
WHERE "Skill" IS NULL OR length("Skill") < 2 OR unaccent_lower("Skill") IN ('na', 'n/a', 'none');
DELETE FROM "RequestSkills" t USING "RequestSkills" d
WHERE t."RequestId" = d."RequestId" AND unaccent_lower(t."Skill") = unaccent_lower(d."Skill") AND t."Id" > d."Id";
