/**
 * API Configuration
 * 
 * This file controls whether the app uses mock data or real API calls.
 * Set VITE_USE_MOCK_DATA=true in your .env file to use mock data.
 * Set VITE_USE_MOCK_DATA=false (or omit) to use real API calls.
 */

import { API_BASE_URL, USE_MOCK_DATA } from '@/config/runtimeConfig';
export { API_BASE_URL, USE_MOCK_DATA };

export const API_TIMEOUT = 30000; // 30 seconds

export const API_CONFIG = {
  useMockData: USE_MOCK_DATA,
  baseUrl: API_BASE_URL,
  timeout: API_TIMEOUT,
  mockDelay: 300, // Simulated network delay for mock data (ms)
} as const;
