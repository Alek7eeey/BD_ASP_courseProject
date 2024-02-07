-------------USERS-------------
CREATE TABLE USERS (
  id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
  login NVARCHAR2(60) NOT NULL,
  password NVARCHAR2(100) NOT NULL,
  gmail_address NVARCHAR2(100) not null,
  description NVARCHAR2(100) default null,
  type NVARCHAR2(10) CHECK ( type IN ('user', 'admin', 'superUser')) NOT NULL,
  is_register NUMBER(1) CHECK ( is_register IN (0, 1)) NOT NULL,
  CONSTRAINT user_pk PRIMARY KEY (id)
);

insert into USERS (login, password, gmail_address, description, type, is_register)
values ('admin', ENCRYPT_PASSWORD('admin'), 'alekseykravchenko120@gmail.com', 'This is admin', 'admin', 1);

-------------USER_PHOTO-------------
CREATE TABLE USER_PHOTO (
  id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
  user_id NUMBER(10) NOT NULL,
  title NVARCHAR2(100) default null,
  image BLOB DEFAULT EMPTY_BLOB(),
  CONSTRAINT user_photo_pk PRIMARY KEY (id),
  CONSTRAINT user_id_fk FOREIGN KEY (user_id) REFERENCES USERS(id)
);
drop table USER_PHOTO;

-------------USER_DESCRIPTION-------------
CREATE TABLE USER_DESCRIPTION (
    id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
    user_id NUMBER(10) NOT NULL,
    telegram_URL NVARCHAR2(100) default null,
    gmail_address NVARCHAR2(100) default null,
    company_name NVARCHAR2(100) default null,
    city_name NVARCHAR2(100) default null,
    CONSTRAINT user_description_pk PRIMARY KEY (id),
    CONSTRAINT user_description_id_fk FOREIGN KEY (user_id) REFERENCES USERS(id)
);

-------------PROJECT-------------
drop table PROJECT;
CREATE TABLE PROJECT(
    id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
    name NVARCHAR2(100) NOT NULL,
    from_date DATE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    to_date DATE NOT NULL,
    description NVARCHAR2(400),
    type NVARCHAR2(12) CHECK (type in ('group', 'individual')) NOT NULL,
    artist_id NUMBER(10) default null, --id исполнителя
    is_ready NUMBER(1) CHECK ( is_ready IN (0, 1)) NOT NULL,
    is_send NUMBER(1) CHECK ( is_send IN (0, 1)) NOT NULL,
    creator_id NUMBER(10) NOT NULL,
    CONSTRAINT project_pk PRIMARY KEY (id),
    CONSTRAINT project_artist_id_fk FOREIGN KEY (artist_id) REFERENCES USERS(id),
    CONSTRAINT project_creator_id_fk FOREIGN KEY (creator_id) REFERENCES USERS(id)
);
insert into PROJECT(name, from_date, to_date, description, type, is_ready, is_send, creator_id)
values ('September project', to_date('01-09-2023'), to_date('10-09-2023'), 'This is description','individual',1,0,234762);
insert into READY_PROJECT(project_id, date_complete, performer_user_id)
values (333, to_date('08-09-2023'),234762 );

select *
from PROJECT;
-------------READY_PROJECT-------------
drop table READY_PROJECT;
CREATE TABLE READY_PROJECT(
    id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
    project_id NUMBER(10) NOT NULL,
    date_complete DATE DEFAULT CURRENT_TIMESTAMP NOT NULL,
    performer_user_id NUMBER(10) NOT NULL,
    CONSTRAINT ready_project_pk PRIMARY KEY (id),
    CONSTRAINT project_id_fk FOREIGN KEY (project_id) REFERENCES PROJECT(id),
    CONSTRAINT performer_user_fk FOREIGN KEY (performer_user_id) REFERENCES USERS(id)
);

-------------SEND_PROJECT-------------
drop table SEND_PROJECT;
CREATE TABLE SEND_PROJECT(
    id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
    project_id NUMBER(10) NOT NULL,
    date_send DATE DEFAULT CURRENT_TIMESTAMP not null,
    sender_user_id NUMBER(10) NOT NULL,
    description NVARCHAR2(400),
    CONSTRAINT send_project_pk PRIMARY KEY (id),
    CONSTRAINT send_project_id_fk FOREIGN KEY (project_id) REFERENCES PROJECT(id),
    CONSTRAINT sender_user_fk FOREIGN KEY (sender_user_id) REFERENCES USERS(id)
);

-------------PROJECT_PHOTO-------------
drop table PROJECT_PHOTO;
CREATE TABLE PROJECT_PHOTO(
    id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
    project_id NUMBER(10) NOT NULL,
    title NVARCHAR2(100) default null,
    photo BLOB DEFAULT EMPTY_BLOB(),
    CONSTRAINT photo_pk PRIMARY KEY (id),
    CONSTRAINT project_photo_id_fk FOREIGN KEY (project_id) REFERENCES PROJECT(id)
);

-------------PROJECT_DOCUMENT-------------
drop table PROJECT_DOCUMENT;
CREATE TABLE PROJECT_DOCUMENT(
    id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
    project_id NUMBER(10) NOT NULL,
    title NVARCHAR2(100) default null,
    type NVARCHAR2(10) default null,
    data BLOB DEFAULT EMPTY_BLOB(),
    CONSTRAINT project_document_pk PRIMARY KEY (id),
    CONSTRAINT project_document_id_fk FOREIGN KEY (project_id) REFERENCES PROJECT(id)
);

-------------TASK_IN_PROJECT-------------
drop table TASK_IN_PROJECT;
CREATE TABLE TASK_IN_PROJECT(
     id NUMBER(10) GENERATED AS IDENTITY (START WITH 1 INCREMENT BY 1),
     project_id NUMBER(10) NOT NULL,
     description NVARCHAR2(400),
     CONSTRAINT task_pk PRIMARY KEY (id),
     CONSTRAINT project_task_id_fk FOREIGN KEY (project_id) REFERENCES PROJECT(id)
);
