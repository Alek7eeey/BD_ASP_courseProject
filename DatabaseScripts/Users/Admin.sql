-------------USER_ADMIN-------------
--TABLESPACE
CREATE TABLESPACE TASK_WAVE_ADMIN_TS
        DATAFILE 'TASK_WAVE_ADMIN_TS'
        SIZE 7M
        AUTOEXTEND ON NEXT 5M
        MAXSIZE 100 M;

--DROP TABLESPACE TASK_WAVE_ADMIN_TS INCLUDING CONTENTS and DATAFILES;

create TEMPORARY TABLESPACE TASK_WAVE_ADMIN_TS_TEMP
    TEMPFILE 'TASK_WAVE_ADMIN_TS_TEMP'
    SIZE 5M
    AUTOEXTEND ON NEXT 3M
    MAXSIZE 30M;

--DROP TABLESPACE TASK_WAVE_ADMIN_TS_TEMP INCLUDING CONTENTS and DATAFILES;

--ROLE
ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE; --use hidden session param

--Priv admin
    GRANT CREATE SESSION, ALTER SESSION, CREATE PLUGGABLE DATABASE
    TO ADMIN_TASK_WAVE;

    GRANT RESTRICTED SESSION TO ADMIN_TASK_WAVE;

    GRANT CREATE TABLE, ALTER TABLE, DROP TABLE,
    COMMENT TABLE, SELECT TABLE, INSERT TABLE,
    UPDATE TABLE, DELETE TABLE,
    FLASHBACK TABLE, ALTER INDEX, DROP INDEX,
    CREATE INDEX, CREATE TRIGGER, ALTER TRIGGER,
    DROP TRIGGER, EXECUTE PROCEDURE, CREATE PROCEDURE,
    ALTER ANY PROCEDURE, DROP PROCEDURE, CREATE SEQUENCE,
    ALTER SEQUENCE, DROP SEQUENCE, CREATE VIEW,
    ALTER ANALYTIC VIEW,
    DROP VIEW TO ADMIN_TASK_WAVE;
    grant EXECUTE ON sys.dbms_crypto to ADMIN_TASK_WAVE;

    ALTER USER ADMIN_TASK_WAVE QUOTA UNLIMITED ON TASK_WAVE_ADMIN_TS;
    --DROP ROLE admin_TW_role;

--PROFILE
    CREATE PROFILE admin_TW_profile
  LIMIT
    SESSIONS_PER_USER UNLIMITED
    CPU_PER_SESSION UNLIMITED
    CPU_PER_CALL UNLIMITED
    CONNECT_TIME UNLIMITED
    IDLE_TIME UNLIMITED
    LOGICAL_READS_PER_SESSION UNLIMITED
    LOGICAL_READS_PER_CALL UNLIMITED
    PRIVATE_SGA UNLIMITED
    COMPOSITE_LIMIT UNLIMITED
    FAILED_LOGIN_ATTEMPTS UNLIMITED
    PASSWORD_LIFE_TIME UNLIMITED
    PASSWORD_REUSE_MAX UNLIMITED
    PASSWORD_REUSE_TIME UNLIMITED
    PASSWORD_LOCK_TIME UNLIMITED
    PASSWORD_GRACE_TIME UNLIMITED;

    --drop profile admin_TW_profile;

--user
    CREATE USER ADMIN_TASK_WAVE IDENTIFIED BY 1
    PROFILE admin_TW_profile
    DEFAULT TABLESPACE TASK_WAVE_ADMIN_TS
    TEMPORARY TABLESPACE TASK_WAVE_ADMIN_TS_TEMP
    ACCOUNT UNLOCK
    PASSWORD EXPIRE;

select * from SYS.DBA_USERS;

    --DROP USER ADMIN_TASK_WAVE cascade;

