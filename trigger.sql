-- FUNCTION: public.trigger_function()

-- DROP FUNCTION IF EXISTS public.trigger_function();

CREATE OR REPLACE FUNCTION public.trigger_function()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    INSERT INTO completed_works (order_id, staff_id, complworks_datetime)
    VALUES (NEW.order_id, NEW.staff_id, NOW());
    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.trigger_function()
    OWNER TO postgres;

-- FUNCTION: public.trigger_function_technic()

-- DROP FUNCTION IF EXISTS public.trigger_function_technic();

CREATE OR REPLACE FUNCTION public.trigger_function_technic()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
	
    UPDATE technic	 
    SET  technic_mileage = NEW.order_mileage,
		 technic_condition = 'В ремонте'
	WHERE technic.technic_id = NEW.technic_id; 

    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.trigger_function_technic()
    OWNER TO postgres;

-- FUNCTION: public.update_order_completion()

-- DROP FUNCTION IF EXISTS public.update_order_completion();

CREATE OR REPLACE FUNCTION public.update_order_completion()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    IF NEW.order_status = 1 AND (OLD.order_status != 1 OR NEW.order_datecompletion IS NULL) THEN
        NEW.order_datecompletion = CURRENT_DATE;
        UPDATE technic
        SET technic_condition = 'Отремонтировано'
        WHERE technic_id = NEW.technic_id;
    END IF;
    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.update_order_completion()
    OWNER TO postgres;

-- FUNCTION: public.update_parts_quantity_on_supply()

-- DROP FUNCTION IF EXISTS public.update_parts_quantity_on_supply();

CREATE OR REPLACE FUNCTION public.update_parts_quantity_on_supply()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN
    UPDATE repairparts
    SET repairparts_quantity = repairparts_quantity + NEW.supply_quantity
    WHERE repairparts_id = NEW.repairparts_id;
    RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.update_parts_quantity_on_supply()
    OWNER TO postgres;
