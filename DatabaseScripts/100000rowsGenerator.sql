DECLARE
    v_user_login VARCHAR2(50);
    v_user_password VARCHAR2(50);
    v_user_email VARCHAR2(50);
    v_user_description VARCHAR2(100);

BEGIN
    FOR i IN 1..100000 LOOP
        -- Генерация случайных данных
        v_user_login := 'user' || i;
        v_user_password := 'password' || i;
        v_user_email := 'user' || i || '@example.com';
        v_user_description := 'Description for user ' || i;

        -- Вызов процедуры для вставки пользователя
        CreateStandartUser(
            user_login => v_user_login,
            user_password => v_user_password,
            user_email => v_user_email,
            user_description => v_user_description
        );
    END LOOP;
END;

explain plan for select *
from USERS
where LOGIN = 'user10000';
select * from table(DBMS_XPLAN.DISPLAY());