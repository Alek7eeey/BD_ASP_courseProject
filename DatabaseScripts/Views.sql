-------------USER_VIEW-------------
CREATE VIEW USER_VIEW AS
    select USERS.ID as USER_ID, USERS.NAME, USERS.LOGIN,
           USERS.PASSWORD, USERS.IS_REGISTER, USERS.TYPE,
           UP.ID as user_photo_id, UP.IMAGE,
           UD.ID as user_description_id, UD.COMPANY_NAME,
           UD.TELEGRAM_URL, UD.GMAIL_ADDRESS
    from USERS inner join USER_PHOTO UP on USERS.ID = UP.USER_ID
               inner join USER_DESCRIPTION UD on USERS.ID = UD.USER_ID;

drop view USER_VIEW;

-------------USER_NOTIFICATION_VIEW-------------
CREATE VIEW USER_NOTIFICATION_VIEW AS
        select USERS.ID as user_id, USERS.NAME, USERS.LOGIN,
               USERS.PASSWORD, USERS.IS_REGISTER, USERS.TYPE,
               UP.ID as user_photo_id, UP.IMAGE,
               UD.ID as user_description_id, UD.COMPANY_NAME,
               UD.TELEGRAM_URL, UD.GMAIL_ADDRESS,
               N.ID as notification_id, N.DATE_SEND, N.MESSAGE
    from USERS inner join USER_PHOTO UP on USERS.ID = UP.USER_ID
               inner join USER_DESCRIPTION UD on USERS.ID = UD.USER_ID
               inner join NOTIFICATION N on USERS.ID = N.SENDER_ID;

drop view USER_NOTIFICATION_VIEW;
-------------PROJECT_VIEW-------------
CREATE VIEW PROJECT_VIEW AS
        select pr.ID as project_id, pr.TYPE as pr_type, pr.NAME, pr.CREATOR_ID,
               pr.DESCRIPTION, pr.FROM_DATE, pr.IS_READY, pr.IS_SEND,
               pr.TO_DATE, pr.ARTIST_ID, PP.ID as project_photo_id,
               PP.PHOTO, PD.ID as project_document_id, PD.TYPE as document_type,
               PD.DATA, PD.TITLE
    from PROJECT pr inner join PROJECT_PHOTO PP on pr.ID = PP.PROJECT_ID
                    inner join PROJECT_DOCUMENT PD on pr.ID = PD.PROJECT_ID;

drop view PROJECT_VIEW;
-------------TASK_PROJECT_VIEW-------------
CREATE VIEW TASK_PROJECT_VIEW AS
        select PW.project_id, PW.pr_type, PW.NAME, PW.CREATOR_ID,
               PW.DESCRIPTION, PW.FROM_DATE, PW.IS_READY, PW.IS_SEND,
               PW.TO_DATE, PW.ARTIST_ID, PW.project_photo_id,
               PW.PHOTO, PW.project_document_id, PW.document_type,
               PW.DATA, PW.TITLE, TIP.ID as task_id, TIP.IS_READY as is_ready_task,
               TIP.DESCRIPTION as description_task, TIP.NAME as task_name
        from PROJECT_VIEW PW inner join TASK_IN_PROJECT TIP
            on PW.DESCRIPTION = TIP.DESCRIPTION;

drop view TASK_PROJECT_VIEW;
-------------SEND_PROJECT_VIEW-------------
CREATE VIEW SEND_PROJECT_VIEW AS
        select PW.project_id, PW.pr_type, PW.NAME, PW.CREATOR_ID,
               PW.DESCRIPTION, PW.FROM_DATE, PW.IS_READY, PW.IS_SEND,
               PW.TO_DATE, PW.ARTIST_ID, PW.project_photo_id,
               PW.PHOTO, PW.project_document_id, PW.document_type,
               PW.DATA, PW.TITLE, PW.task_id, PW.is_ready_task,
               PW.description_task, PW.task_name, SP.ID as send_project_id,
               SP.DATE_SEND, SP.DESCRIPTION as send_project_descriprion,
               SP.SENDER_USER_ID
        from TASK_PROJECT_VIEW PW inner join SEND_PROJECT SP
            on PW.DESCRIPTION = SP.DESCRIPTION;

drop view SEND_PROJECT_VIEW;

-------------READY_PROJECT_VIEW-------------
CREATE VIEW READY_PROJECT_VIEW AS
        select PW.project_id, PW.pr_type, PW.NAME, PW.CREATOR_ID,
               PW.DESCRIPTION, PW.FROM_DATE, PW.IS_READY, PW.IS_SEND,
               PW.TO_DATE, PW.ARTIST_ID, PW.project_photo_id,
               PW.PHOTO, PW.project_document_id, PW.document_type,
               PW.DATA, PW.TITLE, PW.task_id, PW.is_ready_task,
               PW.description_task, PW.task_name, RP.ID as ready_project_id,
               RP.DESCRIPTION as reasy_project_description,
               RP.DATE_COMPLETE, RP.PERFORMER_USER_ID
        from TASK_PROJECT_VIEW PW inner join READY_PROJECT RP
            on PW.DESCRIPTION = RP.DESCRIPTION;

drop view READY_PROJECT_VIEW;