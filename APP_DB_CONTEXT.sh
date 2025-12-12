#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$ROOT_DIR/WetHands.Infrastructure.Database/WetHands.Infrastructure.Database.csproj"
STARTUP_PROJECT="$ROOT_DIR/WetHands.WebAPI/WebAPI.csproj"
OUTPUT_FILE="$ROOT_DIR/AppDbContext.sql"

echo "Generating AppDbContext migration script -> ${OUTPUT_FILE}"
dotnet ef dbcontext script \
  --context AppDbContext \
  --project "${PROJECT_PATH}" \
  --startup-project "${STARTUP_PROJECT}" \
  --output "${OUTPUT_FILE}" \
  "$@"

echo "Done."
