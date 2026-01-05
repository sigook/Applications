CREATE OR REPLACE FUNCTION get_week_start_sunday(date timestamp) RETURNS integer AS $total$
DECLARE total integer;
BEGIN
SELECT extract(week from (date + '1 day'::interval)) into total;
RETURN total;
END;$total$ LANGUAGE plpgsql;