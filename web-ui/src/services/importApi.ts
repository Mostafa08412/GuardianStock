// Import API service for CSV product imports

export interface UploadPreviewResponse {
  jobId: string;
}

export interface ImportStatusResponse {
  jobId: string;
  status: 'pending' | 'processing' | 'preview_ready' | 'importing' | 'completed' | 'failed';
  succeededCount?: number;
  failedCount?: number;
  errorMessage?: string;
}

import { API_BASE_URL } from '@/api/config';

const API_BASE = API_BASE_URL;

function getAuthHeaders(): HeadersInit {
  const token = localStorage.getItem('access_token');
  return token ? { 'Authorization': `Bearer ${token}` } : {};
}

export async function uploadForPreview(file: File, jobId: string): Promise<UploadPreviewResponse> {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('jobId', jobId);

  const response = await fetch(`${API_BASE}/v2/products/import-preview`, {
    method: 'POST',
    headers: {
      ...getAuthHeaders(),
      // FormData sets its own Content-Type boundary
    },
    body: formData,
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to upload file for preview');
  }

  return response.json();
}

export async function confirmImport(previewId: string, jobId: string): Promise<void> {

  const formData = new FormData();
  formData.append('previewId', previewId);
  formData.append('jobId', jobId);
  const response = await fetch(`${API_BASE}/v2/products/confirm-import`, {

    method: 'POST',
    headers: {
      ...getAuthHeaders(),
    },
    body: formData,
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to confirm import');
  }
}

export async function getImportStatus(jobId: string): Promise<ImportStatusResponse> {
  const response = await fetch(`${API_BASE}/import/status/${jobId}`, {
    headers: {
      ...getAuthHeaders(),
    },
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || 'Failed to get import status');
  }

  return response.json();
}
