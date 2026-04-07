-- Smart Hostel Management System - Database Initialization Script
-- This script runs automatically when the PostgreSQL container starts

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Set default client encoding
SET client_encoding = 'UTF8';

-- Display startup message
\echo 'Smart Hostel Management System - Database Initialization'
\echo 'Database: SmartHostelDB'
\echo 'Initialization started...'

-- Create application user if not exists
DO
$do$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_roles WHERE rolname = 'smarthostel'
   ) THEN
      CREATE ROLE smarthostel WITH LOGIN PASSWORD 'SmartHostelUser123!';
      ALTER ROLE smarthostel CREATEDB;
   END IF;
END
$do$;

-- Grant privileges
GRANT CONNECT ON DATABASE "SmartHostelDB" TO smarthostel;
GRANT USAGE ON SCHEMA public TO smarthostel;
GRANT CREATE ON SCHEMA public TO smarthostel;

-- Set connection limit
ALTER ROLE smarthostel WITH CONNECTION LIMIT 50;

\echo 'Database initialization completed successfully!'
\echo 'Server ready to accept connections.'
