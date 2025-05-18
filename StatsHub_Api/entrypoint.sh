#!/bin/bash

echo "Waiting for PostgreSQL to start..."
until pg_isready -h postgres -p 5432 -U postgres; do
  echo "PostgreSQL not ready yet, waiting..."
  sleep 2
done

echo "Applying database migrations..."
dotnet ef database update --project /app/StatsHub_Api.csproj

echo "Starting application..."
dotnet StatsHub_Api.dll