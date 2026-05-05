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



-- PROCEDURE: public.add_new_order(integer, integer, integer, integer, character varying)

-- DROP PROCEDURE IF EXISTS public.add_new_order(integer, integer, integer, integer, character varying);

CREATE OR REPLACE PROCEDURE public.add_new_order(
	IN p_staff_id integer,
	IN p_client_id integer,
	IN p_technic_id integer,
	IN p_order_mileage integer,
	IN p_order_description character varying)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
    -- Вставляем заказ
    INSERT INTO "Order" (
        staff_id, client_id, technic_id,
        order_datecreation, order_mileage,
        order_description, order_status
    )
    VALUES (
        p_staff_id, p_client_id, p_technic_id,
        CURRENT_DATE, p_order_mileage,
        p_order_description, 0  -- "В работе"
    );

    -- Автоматически обновляем технику
    UPDATE technic
    SET technic_mileage = p_order_mileage,
        technic_condition = 'В ремонте'
    WHERE technic_id = p_technic_id;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        RAISE NOTICE 'Ошибка при добавлении заказа: %', SQLERRM;
        ROLLBACK;
END;
$BODY$;
ALTER PROCEDURE public.add_new_order(integer, integer, integer, integer, character varying)
    OWNER TO postgres;



-- PROCEDURE: public.complete_order(integer, date)

-- DROP PROCEDURE IF EXISTS public.complete_order(integer, date);

CREATE OR REPLACE PROCEDURE public.complete_order(
	IN p_order_id integer,
	IN p_completion_date date DEFAULT CURRENT_DATE)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    v_technic_id integer;
BEGIN
    -- Получаем technic_id заказа
    SELECT technic_id INTO v_technic_id
    FROM "Order"
    WHERE order_id = p_order_id;

    IF v_technic_id IS NULL THEN
        RAISE EXCEPTION 'Заказ % не найден', p_order_id;
    END IF;

    -- Обновляем заказ
    UPDATE "Order"
    SET order_status = 1,          -- Выполнен
        order_datecompletion = p_completion_date
    WHERE order_id = p_order_id;

    -- Обновляем состояние техники
    UPDATE technic
    SET technic_condition = 'Отремонтировано'
    WHERE technic_id = v_technic_id;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        RAISE NOTICE 'Ошибка завершения заказа: %', SQLERRM;
        ROLLBACK;
END;
$BODY$;
ALTER PROCEDURE public.complete_order(integer, date)
    OWNER TO postgres;
