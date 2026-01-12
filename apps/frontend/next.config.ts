import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
  // Enable standalone output for Docker builds
  output: 'standalone',
  experimental: {
    typedRoutes: true,
  },
};

export default nextConfig;
