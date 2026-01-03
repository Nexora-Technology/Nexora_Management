-- Nexora Management Database Initialization Script
-- This script creates the initial database schema

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Create initial schema placeholder
-- Tables will be created by EF Core migrations

-- Grant necessary permissions
GRANT ALL PRIVILEGES ON DATABASE nexora_dev TO nexora;
GRANT ALL PRIVILESES ON SCHEMA public TO nexora;

-- Create custom types (if needed)
-- DO $$ BEGIN
--     IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_role') THEN
--         CREATE TYPE user_role AS ENUM ('Admin', 'Manager', 'User');
--     END IF;
-- END $$;

-- Log initialization
INSERT INTO public.schema_version (version, applied_at) VALUES ('v1.0.0-init', NOW()) ON CONFLICT DO NOTHING;
