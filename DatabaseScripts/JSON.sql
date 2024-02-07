-- Экспорт данных из таблицы PROJECT
CREATE OR REPLACE FUNCTION EXPORT_PROJECT_JSON (p_project_id IN NUMBER)
RETURN CLOB
AS
    v_json CLOB;
BEGIN
    SELECT JSON_OBJECT(
               'id'          VALUE id,
               'name'        VALUE name,
               'from_date'   VALUE from_date,
               'to_date'     VALUE to_date,
               'description' VALUE description,
               'type'        VALUE type,
               'artist_id'   VALUE artist_id,
               'is_ready'    VALUE is_ready,
               'is_send'     VALUE is_send,
               'creator_id'  VALUE creator_id
           )
    INTO v_json
    FROM PROJECT
    WHERE id = p_project_id;

    RETURN v_json;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN NULL;
END EXPORT_PROJECT_JSON;


--Импорт данных из таблицы PROJECT
CREATE OR REPLACE PROCEDURE IMPORT_PROJECT_JSON (p_json_data IN CLOB)
AS
    v_name         NVARCHAR2(100);
    v_from_date    DATE;
    v_to_date      DATE;
    v_description  NVARCHAR2(400);
    v_type         NVARCHAR2(12);
    v_artist_id    NUMBER;
    v_is_ready     NUMBER(1);
    v_is_send      NUMBER(1);
    v_creator_id   NUMBER;
BEGIN
    SELECT JSON_VALUE(p_json_data, '$.name') INTO v_name FROM DUAL;
    SELECT TO_DATE(JSON_VALUE(p_json_data, '$.from_date'), 'YYYY-MM-DD"T"HH24:MI:SS') INTO v_from_date FROM DUAL;
    SELECT TO_DATE(JSON_VALUE(p_json_data, '$.to_date'), 'YYYY-MM-DD"T"HH24:MI:SS') INTO v_to_date FROM DUAL;
    SELECT JSON_VALUE(p_json_data, '$.description') INTO v_description FROM DUAL;
    SELECT JSON_VALUE(p_json_data, '$.type') INTO v_type FROM DUAL;
    SELECT JSON_VALUE(p_json_data, '$.artist_id') INTO v_artist_id FROM DUAL;
    SELECT JSON_VALUE(p_json_data, '$.is_ready') INTO v_is_ready FROM DUAL;
    SELECT JSON_VALUE(p_json_data, '$.is_send') INTO v_is_send FROM DUAL;
    SELECT JSON_VALUE(p_json_data, '$.creator_id') INTO v_creator_id FROM DUAL;
    v_name := v_name || '_2';
    v_from_date := CURRENT_TIMESTAMP;
    INSERT INTO PROJECT (
        name,
        from_date,
        to_date,
        description,
        type,
        artist_id,
        is_ready,
        is_send,
        creator_id
    )
    VALUES (
        v_name,
        v_from_date,
        v_to_date,
        v_description,
        v_type,
        v_artist_id,
        v_is_ready,
        v_is_send,
        v_creator_id
    );

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
        ROLLBACK;
END IMPORT_PROJECT_JSON;

--вызов
DECLARE
    v_exported_json CLOB;
BEGIN
    -- Вызываем экспорт данных для проекта с определенным ID
    v_exported_json := EXPORT_PROJECT_JSON(201);

    -- Выводим экспортированный JSON
    DBMS_OUTPUT.PUT_LINE('Exported JSON: ' || v_exported_json);

    -- Вызываем импорт данных из экспортированного JSON
    IMPORT_PROJECT_JSON(v_exported_json);

    -- Выводим сообщение об успешном импорте
    DBMS_OUTPUT.PUT_LINE('Import successful');
EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END;