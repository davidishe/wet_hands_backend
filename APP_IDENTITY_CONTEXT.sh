#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$ROOT_DIR/WetHands.Identity.Database/WetHands.Identity.Database.csproj"
STARTUP_PROJECT="$ROOT_DIR/WetHands.WebAPI/WebAPI.csproj"
OUTPUT_FILE="$ROOT_DIR/IdentityContext.sql"

echo "Generating IdentityContext migration script -> ${OUTPUT_FILE}"
dotnet ef dbcontext script \
  --context IdentityContext \
  --project "${PROJECT_PATH}" \
  --startup-project "${STARTUP_PROJECT}" \
  --output "${OUTPUT_FILE}" \
  "$@"

echo "Done."
