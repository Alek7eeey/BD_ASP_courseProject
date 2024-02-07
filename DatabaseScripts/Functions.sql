------------encrypt_password--------------
--drop function encrypt_password;
create or replace
FUNCTION encrypt_password
(i_password in users.password%type)
    RETURN USERS.password%type
    IS
    my_key VARCHAR2(2000) := '2023110909112023';
    my_val VARCHAR2(2000) := i_password;
    my_mod NUMBER := sys.DBMS_CRYPTO.encrypt_aes128
+ sys.DBMS_CRYPTO.chain_cbc
+ sys.DBMS_CRYPTO.pad_pkcs5;
    encrypted_password RAW(2000);
BEGIN
    encrypted_password := sys.DBMS_CRYPTO.encrypt(utl_i18n.string_to_raw(my_val, 'AL32UTF8'), my_mod,
utl_i18n.string_to_raw(my_key, 'AL32UTF8'));
    RETURN RAWTOHEX(encrypted_password);
END encrypt_password;


------------getUserById--------------
CREATE OR REPLACE FUNCTION getUserById
(
    idUser in USERS.id%type
)
RETURN SYS_REFCURSOR
IS
    user_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_cursor FOR
    SELECT ID, LOGIN, PASSWORD, GMAIL_ADDRESS, DESCRIPTION, TYPE
    FROM users
    WHERE USERS.ID = idUser
      and
          users.IS_REGISTER = 1;

    RETURN user_cursor;
END getUserById;

------------getUser--------------
CREATE OR REPLACE FUNCTION getUser
(
passwor IN users.password%TYPE,
logi IN users.login%TYPE)
RETURN SYS_REFCURSOR
IS
    user_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_cursor FOR
    SELECT ID, LOGIN, PASSWORD, GMAIL_ADDRESS, DESCRIPTION, TYPE
    FROM users
    WHERE users.login = logi and
          users.PASSWORD = encrypt_password(passwor) and
          users.IS_REGISTER = 1;

    RETURN user_cursor;
END getUser;

------------getMyProjects--------------
--drop function GetOwnProjects;
CREATE OR REPLACE FUNCTION GetOwnProjects
(
userId IN PROJECT.CREATOR_ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE,
           PROJECT.DESCRIPTION as DESCRIPTION, PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
    WHERE
            PROJECT.CREATOR_ID = userId and
            PROJECT.TYPE = 'individual'
            and
            PROJECT.IS_SEND = 0
            and
            PROJECT.IS_READY = 0;
    RETURN project_cursor;
END GetOwnProjects;

------------GetTeamProjects--------------
CREATE OR REPLACE FUNCTION GetTeamProjects
(
userId IN PROJECT.ARTIST_ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
    WHERE
            PROJECT.ARTIST_ID = userId
      and
            PROJECT.TYPE = 'group'
      and
            PROJECT.IS_SEND = 0
      and
            PROJECT.IS_READY = 0;
    RETURN project_cursor;
END GetTeamProjects;

------------GetSendProjects--------------
CREATE OR REPLACE FUNCTION GetSendProjects
(
userId IN PROJECT.ARTIST_ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc,

           SP.DESCRIPTION as SEND_PROJECT_DESCRIPTION,
           SP.DATE_SEND AS DATE_SEND_PROJECT,
           SP.SENDER_USER_ID AS SENDER_USER_ID,
           SP.ID AS ID_SEND_PROJECT

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
        left join SEND_PROJECT SP on PROJECT.ID = SP.PROJECT_ID
    WHERE
            SP.SENDER_USER_ID = userId
      and
            PROJECT.TYPE = 'group'
      and
            PROJECT.IS_SEND = '1';
    RETURN project_cursor;
END GetSendProjects;

------------GetReadyProjects--------------
CREATE OR REPLACE FUNCTION GetReadyProjects
(
userId IN PROJECT.ARTIST_ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc,

           RP.DATE_COMPLETE AS DATE_COMPLETE_PROJECT,
           RP.performer_user_id AS perfomer_USER_ID,
           RP.ID AS ID_READY_PROJECT

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
        left join READY_PROJECT RP on PROJECT.ID = RP.PROJECT_ID
    WHERE
            (RP.performer_user_id = userId or PROJECT.CREATOR_ID = userId)
      and
            PROJECT.IS_READY = '1';
    RETURN project_cursor;
END GetReadyProjects;

------------GetStat--------------
CREATE OR REPLACE FUNCTION GetStat
(
userId IN PROJECT.ARTIST_ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT
        TO_CHAR(RP.DATE_COMPLETE, 'YYYY-MM') AS Month,
        COUNT(*) AS ProjectCount
     FROM
        READY_PROJECT RP
    WHERE
            RP.performer_user_id = userId

    GROUP BY TO_CHAR(RP.DATE_COMPLETE, 'YYYY-MM')
    ORDER BY TO_CHAR(RP.DATE_COMPLETE, 'YYYY-MM') DESC;

    RETURN project_cursor;
END GetStat;

------------GetDescription--------------
CREATE OR REPLACE FUNCTION GetDescription
(
    userId IN USERS.ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    user_description_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_description_cursor FOR
    SELECT
        ud.ID as ID,
        ud.GMAIL_ADDRESS AS GMAIL,
        ud.TELEGRAM_URL AS TELEGRAM,
        ud.COMPANY_NAME AS COMPANY,
        ud.city_name AS CITY,

        u.LOGIN AS LOGIN,
        CASE
            WHEN u.PASSWORD IS NOT NULL THEN '********'
            ELSE NULL
        END AS PASSWORD,

        UP.TITLE AS TITLEPHOTO,
        UP.IMAGE AS IMAGE
    FROM
        USERS U
        LEFT JOIN  USER_DESCRIPTION ud on U.ID = ud.USER_ID
        LEFT JOIN USER_PHOTO UP on U.ID = UP.USER_ID

    WHERE
        U.ID = userId;

    RETURN user_description_cursor;
END GetDescription;

------------getUserByLogin--------------
CREATE OR REPLACE FUNCTION getUserByLogin
(
    loginUser in USERS.LOGIN%type
)
RETURN SYS_REFCURSOR
IS
    user_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_cursor FOR
    SELECT ID, LOGIN, PASSWORD, GMAIL_ADDRESS, DESCRIPTION, TYPE
    FROM users
    WHERE USERS.LOGIN = loginUser
      and
          users.IS_REGISTER = 1;

    RETURN user_cursor;
END getUserByLogin;

------------GetCurrentProject--------------
--drop function GetCurrentProject;
CREATE OR REPLACE FUNCTION GETCURRENTPROJECT
(
    projectId IN PROJECT.ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE,
           PROJECT.DESCRIPTION as DESCRIPTION,

           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,

           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc
     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
    WHERE
            PROJECT.ID = projectId and
            PROJECT.TYPE = 'individual'
            and
            PROJECT.IS_SEND = 0
            and
            PROJECT.IS_READY = 0;
    RETURN project_cursor;
END GETCURRENTPROJECT;

------------GetCurrentTasks--------------
--drop function GetCurrentTasks;
CREATE OR REPLACE FUNCTION GetCurrentTasks
(
    projectId IN PROJECT.ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT TIP.ID as idTask, TIP.DESCRIPTION as TaskDescription
     FROM
       TASK_IN_PROJECT TIP
    WHERE
            TIP.PROJECT_ID = projectId;

    RETURN project_cursor;
END GetCurrentTasks;

------------GetCurrentTeamProject--------------
--drop function GetCurrentTeamProject;
CREATE OR REPLACE FUNCTION GetCurrentTeamProject
(
    projectId IN PROJECT.ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE,
           PROJECT.DESCRIPTION as DESCRIPTION,

           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,

           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc
     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
    WHERE
            PROJECT.ID = projectId and
            PROJECT.TYPE = 'group'
            and
            PROJECT.IS_SEND = 0
            and
            PROJECT.IS_READY = 0;
    RETURN project_cursor;
END GetCurrentTeamProject;

------------GetCurrentSendProject--------------
--drop function GetCurrentSendProject;
CREATE OR REPLACE FUNCTION GetCurrentSendProject
(
    projectId IN PROJECT.ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc,

           SP.DESCRIPTION as SEND_PROJECT_DESCRIPTION,
           SP.DATE_SEND AS DATE_SEND_PROJECT,
           SP.SENDER_USER_ID AS SENDER_USER_ID,
           SP.ID AS ID_SEND_PROJECT

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
        left join SEND_PROJECT SP on PROJECT.ID = SP.PROJECT_ID
    WHERE
            SP.PROJECT_ID = projectId
      and
            PROJECT.TYPE = 'group'
      and
            PROJECT.IS_SEND = '1';
    RETURN project_cursor;
END GetCurrentSendProject;

------------GetCurrentReadyProject--------------
--drop function GetCurrentReadyProject;
CREATE OR REPLACE FUNCTION GETCURRENTREADYPROJECT
(
    projectId IN PROJECT.ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PROJECT.IS_READY AS ISREADY,
           PROJECT.TYPE AS TYPE_PROJECT,

           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc,

           RP.DATE_COMPLETE AS DATE_COMPLETE_PROJECT,
           RP.PERFORMER_USER_ID AS COMPLETE_USER_ID,
           RP.ID AS ID_READY_PROJECT

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
        left join READY_PROJECT RP on PROJECT.ID = RP.PROJECT_ID
    WHERE
            PROJECT.ID = projectId
      and
            PROJECT.IS_READY = '1';
    RETURN project_cursor;
END GetCurrentReadyProject;

------------getUnaffectedUser--------------
CREATE OR REPLACE FUNCTION getUnaffectedUser
(
passwor IN users.password%TYPE,
logi IN users.login%TYPE)
RETURN SYS_REFCURSOR
IS
    user_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_cursor FOR
    SELECT ID, LOGIN, PASSWORD, GMAIL_ADDRESS, DESCRIPTION, TYPE
    FROM users
    WHERE users.login = logi and
          users.PASSWORD = encrypt_password(passwor);

    RETURN user_cursor;
END getUnaffectedUser;

------------GetReviewProjects--------------
CREATE OR REPLACE FUNCTION GetReviewProjects
(
userId IN PROJECT.ARTIST_ID%TYPE
)
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc,

           SP.DESCRIPTION as SEND_PROJECT_DESCRIPTION,
           SP.DATE_SEND AS DATE_SEND_PROJECT,
           SP.SENDER_USER_ID AS SENDER_USER_ID,
           SP.ID AS ID_SEND_PROJECT

     FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID
        left join SEND_PROJECT SP on PROJECT.ID = SP.PROJECT_ID
    WHERE
            PROJECT.CREATOR_ID = userId
      and
            PROJECT.TYPE = 'group'
      and
            PROJECT.IS_SEND = '1';
    RETURN project_cursor;
END GetReviewProjects;

------------GetAllUsers--------------
CREATE OR REPLACE FUNCTION GetAllUsers
RETURN SYS_REFCURSOR
IS
    user_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_cursor FOR
    SELECT ID, LOGIN,
           CASE WHEN users.PASSWORD IS NOT NULL THEN '********'
            ELSE NULL
        END AS PASSWORD,
        GMAIL_ADDRESS, DESCRIPTION, TYPE
    FROM users;

    RETURN user_cursor;
END GetAllUsers;

------------GetAllProjects--------------
CREATE OR REPLACE FUNCTION GetAllProjects
RETURN SYS_REFCURSOR
IS
    project_cursor SYS_REFCURSOR;
BEGIN
    OPEN project_cursor FOR
    SELECT PROJECT.ID as ID, PROJECT.CREATOR_ID as CREATOR_ID,
           PROJECT.NAME as NAME, PROJECT.TO_DATE as TO_DATE,
           PROJECT.FROM_DATE as FROM_DATE, PROJECT.ARTIST_ID as ARTIST_ID,
           PROJECT.DESCRIPTION as DESCRIPTION,
           PROJECT.TYPE AS TYPE,
           PP.ID as idPh,
           PP.TITLE as titlePh, PP.PHOTO as photoPh,
           PD.ID as idDoc, PD.TITLE as titleDoc,
           PD.TYPE as typeDoc, PD.DATA as dataDoc
    FROM
        PROJECT
        LEFT JOIN PROJECT_DOCUMENT PD ON PROJECT.ID = PD.PROJECT_ID
        LEFT JOIN PROJECT_PHOTO PP ON PROJECT.ID = PP.PROJECT_ID;

    RETURN project_cursor;
END GetAllProjects;

------------GetAllUnaffectedUser--------------
CREATE OR REPLACE FUNCTION GetAllUnaffectedUser
RETURN SYS_REFCURSOR
IS
    user_cursor SYS_REFCURSOR;
BEGIN
    OPEN user_cursor FOR
    SELECT ID, LOGIN,
           CASE WHEN users.PASSWORD IS NOT NULL THEN '********'
            ELSE NULL
        END AS PASSWORD,
        GMAIL_ADDRESS, DESCRIPTION, TYPE
    FROM users
    WHERE IS_REGISTER = 0;

    RETURN user_cursor;
END GetAllUnaffectedUser;
