-------------SUPER_user-------------
--TABLESPACE
CREATE TABLESPACE TASK_WAVE_SUPER_USER_TS
        DATAFILE 'TASK_WAVE_SUPER_USER_TS'
        SIZE 7M
        AUTOEXTEND ON NEXT 5M
        MAXSIZE 100 M;

--DROP TABLESPACE TASK_WAVE_ADMIN_TS INCLUDING CONTENTS and DATAFILES;

create TEMPORARY TABLESPACE TASK_WAVE_SUPER_USER_TS_TEMP
    TEMPFILE 'TASK_WAVE_SUPER_USER_TS_TEMP'
    SIZE 5M
    AUTOEXTEND ON NEXT 3M
    MAXSIZE 30M;

--DROP TABLESPACE TASK_WAVE_ADMIN_TS_TEMP INCLUDING CONTENTS and DATAFILES;

--PROFILE
    CREATE PROFILE SUPER_USER_TW_profile
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

--user
    CREATE USER SUPER_USER_TASK_WAVE IDENTIFIED BY 1
    PROFILE SUPER_USER_TW_profile
    DEFAULT TABLESPACE TASK_WAVE_SUPER_USER_TS
    TEMPORARY TABLESPACE TASK_WAVE_SUPER_USER_TS_TEMP
    ACCOUNT UNLOCK
    PASSWORD EXPIRE;

    --DROP USER USER_TASK_WAVE cascade;
--PRIV Super User
    GRANT CREATE SESSION, ALTER SESSION
    TO SUPER_USER_TASK_WAVE;
    GRANT RESTRICTED SESSION TO SUPER_USER_TASK_WAVE;
    grant EXECUTE ON sys.dbms_crypto to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.CreateStandartUser to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.encrypt_password to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.getUser to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.CreateProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.CREATETASK to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.getUserById to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetOwnProjects to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GETTEAMPROJECTS to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetSendProjects to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetReadyProjects to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetStat to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetDescription to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.CreateDefaultDescription to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.ChangeUserInformation to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.getUserByLogin to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.ChangeDescriptionAccount to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GETCURRENTPROJECT to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetCurrentTasks to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.CompleteProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.DeleteProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.UpdateProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.UPDATETASK to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetCurrentTeamProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.SendProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetCurrentSendProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GETCURRENTREADYPROJECT to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.CreateTeamProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.GetReviewProjects to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.AcceptProject to SUPER_USER_TASK_WAVE;
    GRANT EXECUTE on ADMIN_TASK_WAVE.RejectProject to SUPER_USER_TASK_WAVE;
