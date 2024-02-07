-------------DELETE_EVERYTHING_AFTER_DELETE_PROJECT-------------
CREATE OR REPLACE TRIGGER DELETE_EVERYTHING_AFTER_DELETE_PROJECT
    AFTER DELETE on PROJECT
    FOR EACH ROW

    DECLARE
        id_project_delete PROJECT.ID%TYPE;
    BEGIN
        id_project_delete := :OLD.ID;

        --delete in pr_photo
        DELETE FROM PROJECT_PHOTO
            where PROJECT_PHOTO.PROJECT_ID = id_project_delete;

        --delete in pr_document
        DELETE FROM PROJECT_DOCUMENT
            where PROJECT_DOCUMENT.PROJECT_ID = id_project_delete;

        --delete in pr_task
        DELETE FROM TASK_IN_PROJECT
            where TASK_IN_PROJECT.PROJECT_ID = id_project_delete;

        --delete in ready_pr
        DELETE FROM READY_PROJECT
            where READY_PROJECT.PROJECT_ID = id_project_delete;

        --delete in send_pr
        DELETE FROM SEND_PROJECT
            where SEND_PROJECT.PROJECT_ID = id_project_delete;

        commit;
    end;

-------------UPDATE_PROJECT_AFTER_ADD_SEND_PROJECT-------------
CREATE OR REPLACE TRIGGER UPDATE_PROJECT_AFTER_ADD_SEND_PROJECT
    AFTER INSERT on SEND_PROJECT
    FOR EACH ROW

    DECLARE
        id_project SEND_PROJECT.ID%TYPE;
    BEGIN
        id_project := :NEW.ID;

        -- Обновляем поле isSend
        UPDATE PROJECT
        SET PROJECT.IS_SEND = 1
        WHERE id = id_project;

        commit;
    end;

-------------UPDATE_PROJECT_AFTER_DELETE_SEND_PROJECT-------------
CREATE OR REPLACE TRIGGER UPDATE_PROJECT_AFTER_DELETE_SEND_PROJECT
    AFTER DELETE on SEND_PROJECT
    FOR EACH ROW

    DECLARE
        id_project SEND_PROJECT.ID%TYPE;
    BEGIN
        id_project := :NEW.ID;

        -- Обновляем поле isSend
        UPDATE PROJECT
        SET PROJECT.IS_SEND = 0
        WHERE id = id_project;

        commit;
    end;

-------------UPDATE_PROJECT_AFTER_ADD_READY_PROJECT-------------
CREATE OR REPLACE TRIGGER UPDATE_PROJECT_AFTER_ADD_READY_PROJECT
    AFTER DELETE on READY_PROJECT
    FOR EACH ROW

    DECLARE
        id_project READY_PROJECT.ID%TYPE;
    BEGIN
        id_project := :NEW.ID;

        -- Обновляем поле isReady
        UPDATE PROJECT
        SET PROJECT.IS_READY = 1
        WHERE id = id_project;

        commit;
    end;

------------DELETE_EVERYTHING_AFTER_DELETE_USER-------------
drop TRIGGER DELETE_EVERYTHING_AFTER_DELETE_USER;
CREATE OR REPLACE TRIGGER DELETE_EVERYTHING_AFTER_DELETE_USER
    AFTER DELETE on USERS
    FOR EACH ROW

    DECLARE
        id_user USERS.ID%TYPE;
    BEGIN
        id_user := :NEW.ID;

         --delete in user_photo
        DELETE FROM USER_PHOTO
            where USER_PHOTO.USER_ID = id_user;

         --delete in user_description
        DELETE FROM USER_DESCRIPTION
            where USER_DESCRIPTION.USER_ID = id_user;

        commit;
    end;

------------DELETE_NOTIFICATION_AFTER_DELETE_USER-------------
drop TRIGGER DELETE_NOTIFICATION_AFTER_DELETE_USER;
CREATE OR REPLACE TRIGGER DELETE_NOTIFICATION_AFTER_DELETE_USER
    AFTER DELETE on USERS
    FOR EACH ROW

    DECLARE
        id_user USERS.ID%TYPE;
    BEGIN
        id_user := :NEW.ID;

         --delete in user_photo
        DELETE FROM NOTIFICATION
            where NOTIFICATION.SENDER_ID = id_user;

        commit;
    end;

--SELECT * FROM DBA_TRIGGERS ///by SYS
