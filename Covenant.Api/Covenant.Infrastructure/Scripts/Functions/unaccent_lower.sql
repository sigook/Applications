CREATE OR REPLACE FUNCTION unaccent_lower(value text) RETURNS text AS $$
SELECT lower(public.unaccent('public.unaccent'::regdictionary, value));
$$ LANGUAGE sql IMMUTABLE PARALLEL SAFE STRICT;
