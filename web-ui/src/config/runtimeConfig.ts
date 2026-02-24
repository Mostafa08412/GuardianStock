/**
 * Runtime configuration bridge.
 * Prioritizes window.__RUNTIME_CONFIG__ (populated at runtime by Docker entrypoint)
 * over import.meta.env (populated at build time).
 */

interface RuntimeConfig {
    VITE_API_BASE_URL?: string;
    VITE_GOOGLE_CLIENT_ID?: string;
    VITE_USE_MOCK_DATA?: string;
}

declare global {
    interface Window {
        __RUNTIME_CONFIG__?: RuntimeConfig;
    }
}

const getEnv = (key: keyof RuntimeConfig, defaultValue: string): string => {
    const value = window.__RUNTIME_CONFIG__?.[key] || (import.meta.env[key] as string) || defaultValue;
    return value;
};

export const API_BASE_URL = getEnv('VITE_API_BASE_URL', 'http://localhost:5089/api');
export const GOOGLE_CLIENT_ID = getEnv('VITE_GOOGLE_CLIENT_ID', '');
export const USE_MOCK_DATA = getEnv('VITE_USE_MOCK_DATA', 'false') === 'true';

// Diagnostic logging
if (import.meta.env.DEV || window.__RUNTIME_CONFIG__) {
    console.log('[RuntimeConfig] Active Configuration:', {
        API_BASE_URL,
        GOOGLE_CLIENT_ID,
        USE_MOCK_DATA,
        Source: window.__RUNTIME_CONFIG__ ? 'Docker Runtime' : 'Build Environment'
    });
}
