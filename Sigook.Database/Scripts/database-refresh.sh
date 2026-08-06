#!/usr/bin/env bash
set -euo pipefail

trim() {
  local s="$1"
  s="${s#"${s%%[![:space:]]*}"}"
  s="${s%"${s##*[![:space:]]}"}"
  printf '%s' "$s"
}

PGHOST=""
PGPORT="5432"
PGUSER=""
PGPASSWORD=""
SOURCE_DB=""

IFS=';' read -ra PARTS <<< "$CONNECTION_STRING"
for part in "${PARTS[@]}"; do
  [[ "$part" == *"="* ]] || continue
  key="$(trim "${part%%=*}")"
  value="$(trim "${part#*=}")"
  case "${key,,}" in
    server|host) PGHOST="$value" ;;
    port) PGPORT="$value" ;;
    "user id"|username|user) PGUSER="$value" ;;
    password) PGPASSWORD="$value" ;;
    database) SOURCE_DB="$value" ;;
  esac
done

echo "##vso[task.setsecret]${PGPASSWORD}"

for var in PGHOST PGUSER PGPASSWORD SOURCE_DB; do
  if [[ -z "${!var}" ]]; then
    echo "##vso[task.logissue type=error]Connection string is missing a value for ${var}"
    exit 1
  fi
done

STAGING_DB="${SOURCE_DB}Staging"
RESET_USERS="${RESET_USERS:-false}"
PASSWORD_HASH="${PASSWORD_HASH:-}"

if [[ "$RESET_USERS" == "true" && -z "$PASSWORD_HASH" ]]; then
  echo "##vso[task.logissue type=error]PASSWORD_HASH is required when RESET_USERS is true"
  exit 1
fi

export PGHOST PGPORT PGUSER PGPASSWORD SOURCE_DB STAGING_DB RESET_USERS PASSWORD_HASH

echo "Refreshing ${STAGING_DB} from ${SOURCE_DB} on ${PGHOST}..."

docker run --rm -i \
  -e PGHOST -e PGPORT -e PGUSER -e PGPASSWORD -e PGSSLMODE=require \
  -e SOURCE_DB -e STAGING_DB -e RESET_USERS -e PASSWORD_HASH \
  postgres:latest \
  bash -s <<'INNER'
set -euo pipefail

echo "Dumping ${SOURCE_DB}..."
pg_dump --format tar --file /tmp/db.tar "$SOURCE_DB"

echo "Recreating ${STAGING_DB}..."
psql --dbname postgres --set ON_ERROR_STOP=1 <<SQL
DROP DATABASE IF EXISTS "${STAGING_DB}" WITH (FORCE);
CREATE DATABASE "${STAGING_DB}" OWNER "${PGUSER}";
SQL

echo "Restoring dump into ${STAGING_DB}..."
pg_restore --dbname "$STAGING_DB" --no-owner /tmp/db.tar

if [[ "$RESET_USERS" == "true" ]]; then
  echo "Resetting user credentials in ${STAGING_DB}..."
  psql --dbname "$STAGING_DB" --set ON_ERROR_STOP=1 <<SQL
UPDATE "User"
  SET "PasswordHash" = '${PASSWORD_HASH}',
      "EmailConfirmed" = TRUE;
SQL
fi

echo "${STAGING_DB} refreshed successfully."
INNER
