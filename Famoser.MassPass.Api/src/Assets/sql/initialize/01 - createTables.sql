CREATE TABLE users (
  id   INTEGER PRIMARY KEY,
  guid TEXT,
  name TEXT
);

CREATE TABLE devices (
  id                          INTEGER PRIMARY KEY,
  user_id                     INTEGER,
  guid                        TEXT,
  name                        TEXT,
  has_access                  TINYINT,
  last_modification_date_time INTEGER,
  last_request_date_time      INTEGER,
  authorization_date_time     INTEGER,
  access_revoked_reason       TEXT,
  access_revoked_by_device_id TEXT,
  access_revoked_date_time    TEXT
);

CREATE TABLE authorization_codes (
  id         INTEGER PRIMARY KEY,
  user_id    INTEGER,
  content_id INTEGER,
  code       TEXT,
  valid_from INTEGER,
  valid_till INTEGER
);

CREATE TABLE content (
  id            INTEGER PRIMARY KEY,
  user_id       INTEGER,
  guid          TEXT,
  relation_guid TEXT,
  version_id    TEXT
);

CREATE TABLE content_history (
  id            INTEGER PRIMARY KEY,
  content_id    INTEGER,
  device_id    INTEGER,
  version_id    TEXT,
  creation_date_time INTEGER
);