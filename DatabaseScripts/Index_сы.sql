CREATE INDEX index_user
  ON USERS(LOGIN, PASSWORD, GMAIL_ADDRESS);


create INDEX index_project
    on PROJECT(NAME, DESCRIPTION, TO_DATE);

create INDEX index_photoPr
    on PROJECT_PHOTO(TITLE);

create INDEX index_documentPr
    on PROJECT_DOCUMENT(TITLE);

create index index_descrUser
    on USER_DESCRIPTION(USER_ID);

CREATE index index_photoUser
    on USER_PHOTO(TITLE);