/**
 * API Client
 * 
 * Centralized HTTP client for making API requests.
 * Automatically unwraps the backend's ApiResponse<T> wrapper.
 */

import { API_CONFIG } from './config';

interface RequestOptions extends RequestInit {
  params?: Record<string, string | number | boolean | undefined>;
}

/**
 * Shape of the backend's standard response envelope.
 * Every API response is wrapped in this structure.
 */
interface BackendApiResponse<T> {
  isSuccess: boolean;
  data: T;
  message?: string | null;
  errorCode?: string | null;
  validationErrors?: Record<string, string> | null;
  meta?: Record<string, string> | null;
  instance?: string | null;
  traceId?: string | null;
}

/**
 * Shape of the backend's paginated list (inner data).
 * e.g. ProductListItemDtoPaginatedList has { items: [...] }
 * Pagination metadata (page, pageSize, totalCount, totalPages) comes in `meta`.
 */
interface BackendPaginatedList<T> {
  items: T[] | null;
}

interface HttpResponse<T> {
  data: T;
  status: number;
  ok: boolean;
}

class ApiClient {
  private baseUrl: string;
  private timeout: number;

  constructor() {
    this.baseUrl = API_CONFIG.baseUrl;
    this.timeout = API_CONFIG.timeout;
  }

  private buildUrl(endpoint: string, params?: Record<string, string | number | boolean | undefined>): string {
    const url = new URL(`${this.baseUrl}${endpoint}`, window.location.origin);

    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          url.searchParams.append(key, String(value));
        }
      });
    }

    return url.toString();
  }

  private async request<T>(
    endpoint: string,
    options: RequestOptions = {}
  ): Promise<HttpResponse<T>> {
    const { params, ...fetchOptions } = options;

    const url = this.buildUrl(endpoint, params);

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), this.timeout);

    const body = fetchOptions.body instanceof FormData ? fetchOptions.body :
      (fetchOptions.body ? JSON.stringify(fetchOptions.body) : undefined);

    const headers: Record<string, string> = {
      ...(localStorage.getItem('access_token')
        ? { 'Authorization': `Bearer ${localStorage.getItem('access_token')}` }
        : {}),
      ...fetchOptions.headers as Record<string, string>,
    };

    if (!(fetchOptions.body instanceof FormData)) {
      headers['Content-Type'] = 'application/json';
    }

    try {
      const response = await fetch(url, {
        ...fetchOptions,
        body,
        signal: controller.signal,
        headers,
      });

      clearTimeout(timeoutId);

      // Handle 204 No Content (no body to parse)
      if (response.status === 204) {
        return {
          data: undefined as T,
          status: response.status,
          ok: response.ok,
        };
      }

      const data = await response.json();

      return {
        data,
        status: response.status,
        ok: response.ok,
      };
    } catch (error) {
      clearTimeout(timeoutId);

      if (error instanceof Error && error.name === 'AbortError') {
        throw new Error('Request timeout');
      }

      throw error;
    }
  }

  /**
   * Unwraps the backend ApiResponse<T> envelope.
   * Checks isSuccess and throws with the backend message if false.
   * Returns the inner `.data` payload.
   */
  private unwrapResponse<T>(raw: unknown): T {
    const envelope = raw as BackendApiResponse<T>;

    if (envelope && typeof envelope === 'object' && 'isSuccess' in envelope) {
      if (!envelope.isSuccess) {
        const errorMsg = envelope.message || 'API request failed';
        const error = new Error(errorMsg) as Error & {
          errorCode?: string;
          validationErrors?: Record<string, string>;
        };
        error.errorCode = envelope.errorCode ?? undefined;
        error.validationErrors = envelope.validationErrors ?? undefined;
        throw error;
      }
      return envelope.data;
    }

    // If response doesn't have isSuccess, return raw (backwards compat)
    return raw as T;
  }

  /**
   * Unwraps a paginated backend response into our frontend PaginatedResponse shape.
   * Backend returns: { isSuccess, data: { items: [...] }, meta: { page, pageSize, totalCount, totalPages } }
   * Frontend expects: { items: [...], totalCount, page, pageSize, totalPages }
   */
  private unwrapPaginatedResponse<T>(raw: unknown): {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
  } {
    const envelope = raw as BackendApiResponse<BackendPaginatedList<T>>;

    if (envelope && typeof envelope === 'object' && 'isSuccess' in envelope) {
      if (!envelope.isSuccess) {
        const errorMsg = envelope.message || 'API request failed';
        const error = new Error(errorMsg) as Error & {
          errorCode?: string;
          validationErrors?: Record<string, string>;
        };
        error.errorCode = envelope.errorCode ?? undefined;
        error.validationErrors = envelope.validationErrors ?? undefined;
        throw error;
      }

      const items = envelope.data?.items ?? [];
      const meta = (envelope.meta || {}) as Record<string, string>;

      // Helper to find a value by multiple possible keys (case-insensitive)
      const getMetaValue = (keys: string[], defaultValue: string) => {
        for (const key of keys) {
          if (meta[key] !== undefined && meta[key] !== null) return meta[key];
          // Check lowercase version too
          const lowerKey = key.toLowerCase();
          const actualKey = Object.keys(meta).find(k => k.toLowerCase() === lowerKey);
          if (actualKey) return meta[actualKey];
        }
        return defaultValue;
      };

      return {
        items,
        totalCount: parseInt(getMetaValue(['totalCount', 'TotalCount'], '0'), 10),
        page: parseInt(getMetaValue(['page', 'Page', 'pageNumber', 'PageNumber'], '1'), 10),
        pageSize: parseInt(getMetaValue(['pageSize', 'PageSize'], '10'), 10),
        totalPages: parseInt(getMetaValue(['totalPages', 'TotalPages'], '1'), 10),
      };
    }

    // Fallback
    return raw as {
      items: T[];
      totalCount: number;
      page: number;
      pageSize: number;
      totalPages: number;
    };
  }

  /**
   * GET request — unwraps the ApiResponse envelope automatically.
   */
  async get<T>(endpoint: string, options: RequestOptions = {}): Promise<T> {
    const response = await this.request<unknown>(endpoint, {
      ...options,
      method: 'GET',
    });

    if (!response.ok) {
      // Try to extract error message from backend envelope
      try {
        this.unwrapResponse(response.data);
      } catch {
        throw new Error(`API Error: ${response.status}`);
      }
      throw new Error(`API Error: ${response.status}`);
    }

    return this.unwrapResponse<T>(response.data);
  }

  /**
   * GET request for paginated lists — unwraps both the ApiResponse envelope
   * and the PaginatedList structure.
   */
  async getPaginated<T>(endpoint: string, options: RequestOptions = {}): Promise<{
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
  }> {
    const response = await this.request<unknown>(endpoint, {
      ...options,
      method: 'GET',
    });

    if (!response.ok) {
      try {
        this.unwrapResponse(response.data);
      } catch {
        throw new Error(`API Error: ${response.status}`);
      }
      throw new Error(`API Error: ${response.status}`);
    }

    return this.unwrapPaginatedResponse<T>(response.data);
  }

  async post<T>(endpoint: string, body?: any, options: RequestOptions = {}): Promise<T> {
    const response = await this.request<unknown>(endpoint, {
      ...options,
      method: 'POST',
      body: body,
    });

    if (!response.ok) {
      // Try to extract backend error details before throwing
      try {
        this.unwrapResponse(response.data);
      } catch (e) {
        throw e;
      }
      throw new Error(`API Error: ${response.status}`);
    }

    return this.unwrapResponse<T>(response.data);
  }

  async put<T>(endpoint: string, body?: any, options: RequestOptions = {}): Promise<T> {
    const response = await this.request<unknown>(endpoint, {
      ...options,
      method: 'PUT',
      body: body,
    });

    if (!response.ok) {
      try {
        this.unwrapResponse(response.data);
      } catch (e) {
        throw e;
      }
      throw new Error(`API Error: ${response.status}`);
    }

    return this.unwrapResponse<T>(response.data);
  }

  async delete<T>(endpoint: string, options: RequestOptions = {}): Promise<T> {
    const response = await this.request<unknown>(endpoint, {
      ...options,
      method: 'DELETE',
    });

    if (!response.ok) {
      try {
        this.unwrapResponse(response.data);
      } catch (e) {
        throw e;
      }
      throw new Error(`API Error: ${response.status}`);
    }

    return this.unwrapResponse<T>(response.data);
  }
}

export const apiClient = new ApiClient();
