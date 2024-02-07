-------------CreateStandardUser-------------
--drop procedure CreateStandartUser;
CREATE OR REPLACE PROCEDURE CreateStandartUser
(
    user_login IN USERS.login%TYPE,
    user_password IN USERS.password%TYPE,
    user_email IN USERS.gmail_address%TYPE,
    user_description IN USERS.description%TYPE := null
)
IS
        cnt NUMBER;
        encrypted_password_result USERS.password%type;
        user_type USERS.type%TYPE := 'user';
        user_is_register USERS.is_register%TYPE := 1;

    BEGIN
        SELECT COUNT(*) INTO cnt FROM USERS where UPPER(LOGIN) = UPPER(user_login);
        IF(cnt=0) then
            encrypted_password_result := encrypt_password(user_password);
            INSERT INTO USERS(login, password, GMAIL_ADDRESS,DESCRIPTION, type, is_register)
            values(
                   user_login, encrypted_password_result,user_email,user_description, user_type, user_is_register
                  );
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20001, 'This person already exists');
        END IF;
end CreateStandartUser;

-------------CreateSuperUser-------------
drop procedure CreateSuperUser;
CREATE OR REPLACE PROCEDURE CreateSuperUser
(
    user_login IN USERS.login%TYPE,
    user_password IN USERS.password%TYPE,
    user_email IN USERS.gmail_address%TYPE,
    user_description IN USERS.description%TYPE := null
)
IS
        cnt NUMBER;
        encrypted_password_result USERS.password%type;
        user_type USERS.type%TYPE := 'superUser';
        user_is_register USERS.is_register%TYPE := 0;
        current_id USERS.id%TYPE;

    BEGIN
        SELECT COUNT(*) INTO cnt FROM USERS where UPPER(LOGIN) = UPPER(user_login);
        IF(cnt=0) then
            encrypted_password_result := encrypt_password(user_password);
            INSERT INTO USERS(login, password, GMAIL_ADDRESS,DESCRIPTION, type, is_register)
            values(
                   user_login, encrypted_password_result,user_email,user_description, user_type, user_is_register
                  );
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20001, 'This person already exists');
        END IF;
    end CreateSuperUser;

-------------CreateProject-------------
drop procedure CreateProject;
CREATE OR REPLACE PROCEDURE CreateProject
(
    creatorid IN PROJECT.creator_id%TYPE,
    project_name IN PROJECT.name%TYPE,
    toDate IN PROJECT.to_date%TYPE,
    project_description IN PROJECT.description%TYPE,
    photo_name IN PROJECT_PHOTO.title%TYPE DEFAULT NULL,
    photo_data IN PROJECT_PHOTO.photo%TYPE DEFAULT NULL,
    document_name IN PROJECT_DOCUMENT.title%TYPE DEFAULT NULL,
    document_type IN PROJECT_DOCUMENT.type%TYPE DEFAULT NULL,
    document_data IN PROJECT_DOCUMENT.data%TYPE DEFAULT NULL,
    current_id OUT PROJECT.id%TYPE
)
IS
    cnt NUMBER;

BEGIN
    -- Insert data into the PROJECT table
    INSERT INTO PROJECT (
        name,
        to_date,
        description,
        creator_id,
        type,
        is_ready,
        is_send
    ) VALUES (
        project_name,
        toDate,
        project_description,
        creatorid,
        'individual',
        0,
        0
    ) RETURNING id INTO cnt;

    -- Insert data into the PROJECT_PHOTO table if values are provided
    IF photo_name IS NOT NULL AND photo_data IS NOT NULL THEN
        INSERT INTO PROJECT_PHOTO (
            project_id,
            title,
            photo
        ) VALUES (
            cnt,
            photo_name,
            photo_data
        );
    END IF;

    -- Insert data into the PROJECT_DOCUMENT table if values are provided
    IF document_name IS NOT NULL AND document_type IS NOT NULL AND document_data IS NOT NULL THEN
        INSERT INTO PROJECT_DOCUMENT (
            project_id,
            title,
            type,
            data
        ) VALUES (
            cnt,
            document_name,
            document_type,
            document_data
        );
    END IF;

    current_id := cnt;

    COMMIT;
END CreateProject;

-------------CreateTask-------------
drop procedure CreateTask;
CREATE OR REPLACE PROCEDURE CreateTask
(
   project_id IN TASK_IN_PROJECT.PROJECT_ID%TYPE,
   description IN TASK_IN_PROJECT.description%TYPE
)
IS
    BEGIN
        insert into TASK_IN_PROJECT (PROJECT_ID, DESCRIPTION)
        VALUES (PROJECT_ID, DESCRIPTION);
        COMMIT;
END CreateTask;

-------------CreateDefaultDescription-------------
drop procedure CreateDefaultDescription;
CREATE OR REPLACE PROCEDURE CreateDefaultDescription
(
   user_id IN USERS.ID%TYPE
)
IS
    BEGIN
        insert into USER_DESCRIPTION (user_id, telegram_url, gmail_address, company_name, city_name)
        VALUES (USER_ID, '','','','');
        insert into USER_PHOTO (user_id, TITLE, IMAGE)
        VALUES (USER_ID, '','');
        COMMIT;

END CreateDefaultDescription;

-------------ChangeUserInformation-------------
drop procedure ChangeUserInformation;
CREATE OR REPLACE PROCEDURE ChangeUserInformation
(
   user_login IN USERS.LOGIN%TYPE,
   user_password IN USERS.PASSWORD%TYPE,
   user_id IN USERS.ID%TYPE
)
IS
    current_login USERS.LOGIN%TYPE;
    current_password USERS.PASSWORD%TYPE;

BEGIN
    -- Получаем текущие значения логина и пароля для пользователя с указанным ID
    SELECT login, password INTO current_login, current_password
    FROM USERS U
    WHERE U.ID = user_id;

    -- Проверяем, совпадают ли переданные значения с текущими
    IF user_password = '********' AND user_login != current_login then
        UPDATE USERS
        SET LOGIN = user_login

        WHERE ID = user_id;

        COMMIT; -- Фиксация изменений в БД

    elsif user_login != current_login OR user_password != current_password THEN
        -- Если не совпадают, обновляем значения в БД
        UPDATE USERS
        SET LOGIN = user_login,
        PASSWORD = encrypt_password(user_password)
        WHERE ID = user_id;

        COMMIT; -- Фиксация изменений в БД

    END IF;

END ChangeUserInformation;

-------------ChangeDescriptionAccount-------------
drop procedure ChangeDescriptionAccount;
CREATE OR REPLACE PROCEDURE ChangeDescriptionAccount
(
   userid IN USERS.ID%TYPE,
   user_email IN USER_DESCRIPTION.GMAIL_ADDRESS%TYPE,
   user_company IN USER_DESCRIPTION.COMPANY_NAME%TYPE,
   user_telegram IN USER_DESCRIPTION.TELEGRAM_URL%TYPE,
   user_city IN USER_DESCRIPTION.CITY_NAME%TYPE,
   user_image IN USER_PHOTO.IMAGE%TYPE,
   user_title IN USER_PHOTO.TITLE%TYPE
)
IS
   user_description_count INTEGER;
   user_photo_count INTEGER;

BEGIN
    -- Проверяем существование записи в USER_DESCRIPTION для заданного user_id
    SELECT COUNT(*)
    INTO user_description_count
    FROM USER_DESCRIPTION
    WHERE USER_ID = userid;

    -- Если запись уже существует, обновляем значения
    IF user_description_count > 0 THEN
        UPDATE USER_DESCRIPTION
        SET
            TELEGRAM_URL = user_telegram,
            GMAIL_ADDRESS = user_email,
            COMPANY_NAME = user_company,
            CITY_NAME = user_city
        WHERE USER_ID = userid;
    ELSE
        -- Вставляем новую запись в USER_DESCRIPTION
        INSERT INTO USER_DESCRIPTION(USER_ID, TELEGRAM_URL, GMAIL_ADDRESS, COMPANY_NAME, CITY_NAME)
        VALUES (userid, user_telegram, user_email, user_company, user_city);
    END IF;

    -- Проверяем существование записи в USER_PHOTO для заданного user_id
    SELECT COUNT(*)
    INTO user_photo_count
    FROM USER_PHOTO
    WHERE USER_ID = userid;

    -- Если запись уже существует и передано значение для user_title, обновляем значения
    IF user_photo_count > 0 AND user_title IS NOT NULL THEN
        UPDATE USER_PHOTO
        SET
            TITLE = user_title,
            IMAGE = user_image
        WHERE USER_ID = userid;
    ELSIF user_title IS NOT NULL THEN
        -- Вставляем новую запись в USER_PHOTO
        INSERT INTO USER_PHOTO(USER_ID, TITLE, IMAGE)
        VALUES (userid, user_title, user_image);
    END IF;

    COMMIT;

END ChangeDescriptionAccount;

-------------CompleteProject-------------
drop procedure CompleteProject;
CREATE OR REPLACE PROCEDURE CompleteProject
(
    projectId in PROJECT.ID%TYPE
)
IS
    creatorID PROJECT.CREATOR_ID%type;
    BEGIN
    UPDATE PROJECT
        set IS_READY = 1
    where PROJECT.ID = projectId
    returning PROJECT.CREATOR_ID into creatorID;

    insert into READY_PROJECT(PROJECT_ID, PERFORMER_USER_ID)
    VALUES (projectId, creatorID);

END CompleteProject;

-------------DeleteProject-------------
drop procedure DeleteProject;
CREATE OR REPLACE PROCEDURE DeleteProject
(
    projectId in PROJECT.ID%TYPE
)
IS
    creatorID PROJECT.CREATOR_ID%type;
BEGIN
    delete TASK_IN_PROJECT
    where TASK_IN_PROJECT.PROJECT_ID = projectId;

    delete PROJECT_DOCUMENT
    where PROJECT_DOCUMENT.PROJECT_ID = projectId;

    delete PROJECT_PHOTO
    where PROJECT_PHOTO.PROJECT_ID = projectId;

    delete PROJECT
    where PROJECT.ID = projectId;
END DeleteProject;

-------------UpdateProject-------------
drop procedure UpdateProject;
CREATE OR REPLACE PROCEDURE UpdateProject
(
    proj_id IN PROJECT.id%TYPE,
    project_name IN PROJECT.name%TYPE,
    toDate IN PROJECT.to_date%TYPE,
    project_description IN PROJECT.description%TYPE,
    photo_name IN PROJECT_PHOTO.title%TYPE DEFAULT NULL,
    photo_data IN PROJECT_PHOTO.photo%TYPE DEFAULT NULL,
    document_name IN PROJECT_DOCUMENT.title%TYPE DEFAULT NULL,
    document_type IN PROJECT_DOCUMENT.type%TYPE DEFAULT NULL,
    document_data IN PROJECT_DOCUMENT.data%TYPE DEFAULT NULL
)
IS
BEGIN
    -- Update data in the PROJECT table
    UPDATE PROJECT
    SET
        name = project_name,
        to_date = toDate,
        description = project_description
    WHERE id = proj_id;

    -- Update data in the PROJECT_PHOTO table if values are provided

    IF photo_name IS NOT NULL AND photo_data IS NOT NULL THEN
        UPDATE PROJECT_PHOTO ph
        SET
            title = photo_name,
            photo = photo_data
        WHERE ph.project_id = proj_id;
    END IF;

    -- Update data in the PROJECT_DOCUMENT table if values are provided
    IF document_name IS NOT NULL AND document_type IS NOT NULL AND document_data IS NOT NULL THEN
        UPDATE PROJECT_DOCUMENT
        SET
            title = document_name,
            type = document_type,
            data = document_data
        WHERE project_id = proj_id;
    END IF;

    COMMIT;
END UpdateProject;

-------------UpdateTask-------------
drop procedure UpdateTask;
CREATE OR REPLACE PROCEDURE UpdateTask
(
   task_id IN TASK_IN_PROJECT.ID%TYPE,
   descript IN TASK_IN_PROJECT.description%TYPE
)
IS
BEGIN
    -- Update data in the TASK_IN_PROJECT table
    UPDATE TASK_IN_PROJECT
    SET
        description = descript
    WHERE ID = task_id;

    COMMIT;
END UpdateTask;

-------------SendProject-------------
drop procedure SendProject;
CREATE OR REPLACE PROCEDURE SendProject
(
    projectId in PROJECT.ID%TYPE,
    descrip in SEND_PROJECT.DESCRIPTION%TYPE
)
IS
    artistID PROJECT.CREATOR_ID%type;
    BEGIN
    UPDATE PROJECT
        set IS_SEND = 1
    where PROJECT.ID = projectId
    returning PROJECT.ARTIST_ID into artistID;

    insert into SEND_PROJECT(PROJECT_ID, SENDER_USER_ID, DESCRIPTION)
    VALUES (projectId, artistID, descrip);

END SendProject;

-------------CreateProject-------------
drop procedure CreateTeamProject;
CREATE OR REPLACE PROCEDURE CreateTeamProject
(
    creatorid IN PROJECT.creator_id%TYPE,
    project_name IN PROJECT.name%TYPE,
    toDate IN PROJECT.to_date%TYPE,
    project_description IN PROJECT.description%TYPE,
    photo_name IN PROJECT_PHOTO.title%TYPE DEFAULT NULL,
    photo_data IN PROJECT_PHOTO.photo%TYPE DEFAULT NULL,
    document_name IN PROJECT_DOCUMENT.title%TYPE DEFAULT NULL,
    document_type IN PROJECT_DOCUMENT.type%TYPE DEFAULT NULL,
    document_data IN PROJECT_DOCUMENT.data%TYPE DEFAULT NULL,
    artistId in PROJECT.ARTIST_ID%type,
    current_id OUT PROJECT.id%TYPE
)
IS
    cnt NUMBER;

BEGIN

     SELECT COUNT(*) INTO cnt FROM USERS where ID = artistId;
        IF(cnt=0) then
           RAISE_APPLICATION_ERROR(-20001, 'This person already exists');
        END IF;

    -- Insert data into the PROJECT table
    INSERT INTO PROJECT (
        name,
        to_date,
        description,
        creator_id,
        type,
        is_ready,
        is_send,
        ARTIST_ID
    ) VALUES (
        project_name,
        toDate,
        project_description,
        creatorid,
        'group',
        0,
        0,
        artistId
    ) RETURNING id INTO cnt;

    -- Insert data into the PROJECT_PHOTO table if values are provided
    IF photo_name IS NOT NULL AND photo_data IS NOT NULL THEN
        INSERT INTO PROJECT_PHOTO (
            project_id,
            title,
            photo
        ) VALUES (
            cnt,
            photo_name,
            photo_data
        );
    END IF;

    -- Insert data into the PROJECT_DOCUMENT table if values are provided
    IF document_name IS NOT NULL AND document_type IS NOT NULL AND document_data IS NOT NULL THEN
        INSERT INTO PROJECT_DOCUMENT (
            project_id,
            title,
            type,
            data
        ) VALUES (
            cnt,
            document_name,
            document_type,
            document_data
        );
    END IF;

    current_id := cnt;

    COMMIT;
END CreateTeamProject;

-------------AcceptProject-------------
drop procedure AcceptProject;
CREATE OR REPLACE PROCEDURE AcceptProject
(
    projectId in PROJECT.ID%TYPE
)
IS
    creatorID PROJECT.CREATOR_ID%type;
    BEGIN

    UPDATE PROJECT
        set IS_READY = 1, IS_SEND = 0
    where PROJECT.ID = projectId
    returning PROJECT.CREATOR_ID into creatorID;

    insert into READY_PROJECT(PROJECT_ID, PERFORMER_USER_ID)
    VALUES (projectId, creatorID);

    delete from SEND_PROJECT where PROJECT_ID = projectId;

END AcceptProject;

-------------RejectProject-------------
drop procedure RejectProject;
CREATE OR REPLACE PROCEDURE RejectProject
(
    projectId in PROJECT.ID%TYPE
)
IS
    creatorID PROJECT.CREATOR_ID%type;
    BEGIN

    UPDATE PROJECT
        set IS_READY = 0, IS_SEND = 0
    where PROJECT.ID = projectId
    returning PROJECT.CREATOR_ID into creatorID;

    delete from SEND_PROJECT where PROJECT_ID = projectId;

END RejectProject;

-------------DeleteUser-------------
drop procedure DeleteUser;
CREATE OR REPLACE PROCEDURE DeleteUser
(
    userId in users.ID%TYPE
)
IS BEGIN
    delete from USERS where id=userId;
END DeleteUser;

-------------ConfirmUser-------------
drop procedure ConfirmUser;
CREATE OR REPLACE PROCEDURE ConfirmUser
(
    user_id IN USERS.id%TYPE
)
IS
        cnt NUMBER;

    BEGIN
        SELECT COUNT(*) INTO cnt FROM USERS where id = user_id;
        IF(cnt=1) then
            UPDATE USERS
            SET IS_REGISTER = 1
            WHERE USERS.ID = user_id;

            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20007, 'There is no such user');
        END IF;

end ConfirmUser;

select *
from USERS;