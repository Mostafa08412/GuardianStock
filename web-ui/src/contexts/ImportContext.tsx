import { createContext, useContext, useReducer, useEffect, useCallback, ReactNode } from 'react';
import { signalRService, ImportReport, ImportSummary } from '@/services/signalr';
import { getImportStatus } from '@/services/importApi';
import { useAuth } from '@/contexts/AuthContext';
import { toast } from 'sonner';

export type ImportJobStatus =
  | 'uploading'
  | 'processing'
  | 'preview_ready'
  | 'confirming'
  | 'importing'
  | 'completed'
  | 'failed';

export interface ImportJob {
  jobId: string;
  fileName: string;
  status: ImportJobStatus;
  progress: number;
  progressMessage?: string;
  report?: ImportReport;
  summary?: ImportSummary;
  error?: string;
  createdAt: Date;
}

interface ImportState {
  jobs: Record<string, ImportJob>;
  activeJobId: string | null;
  connectionState: 'disconnected' | 'connecting' | 'connected' | 'reconnecting';
}

type ImportAction =
  | { type: 'ADD_JOB'; payload: ImportJob }
  | { type: 'UPDATE_JOB'; payload: { jobId: string; updates: Partial<ImportJob> } }
  | { type: 'REMOVE_JOB'; payload: string }
  | { type: 'SET_ACTIVE_JOB'; payload: string | null }
  | { type: 'SET_CONNECTION_STATE'; payload: ImportState['connectionState'] };

function importReducer(state: ImportState, action: ImportAction): ImportState {
  switch (action.type) {
    case 'ADD_JOB':
      return {
        ...state,
        jobs: { ...state.jobs, [action.payload.jobId]: action.payload },
        activeJobId: action.payload.jobId,
      };
    case 'UPDATE_JOB': {
      const currentJob = state.jobs[action.payload.jobId];
      if (!currentJob) return state;

      // If job is already in a terminal state, ignore updates
      // This prevents race conditions where late progress events might update a failed/completed job
      if (['completed', 'failed'].includes(currentJob.status)) {
        return state;
      }

      return {
        ...state,
        jobs: {
          ...state.jobs,
          [action.payload.jobId]: { ...currentJob, ...action.payload.updates },
        },
      };
    }
    case 'REMOVE_JOB': {
      const { [action.payload]: removed, ...remaining } = state.jobs;
      return {
        ...state,
        jobs: remaining,
        activeJobId: state.activeJobId === action.payload ? null : state.activeJobId,
      };
    }
    case 'SET_ACTIVE_JOB':
      return { ...state, activeJobId: action.payload };
    case 'SET_CONNECTION_STATE':
      return { ...state, connectionState: action.payload };
    default:
      return state;
  }
}

interface ImportContextValue extends ImportState {
  startJob: (jobId: string, fileName: string, initialStatus?: ImportJobStatus) => void;
  updateJobStatus: (jobId: string, status: ImportJobStatus) => void;
  updateJobProgress: (jobId: string, progress: number, message?: string) => void;
  setJobReport: (jobId: string, report: ImportReport) => void;
  setJobSummary: (jobId: string, summary: ImportSummary) => void;
  setJobError: (jobId: string, error: string) => void;
  removeJob: (jobId: string) => void;
  setActiveJob: (jobId: string | null) => void;
  syncJobStatus: (jobId: string) => Promise<void>;
  hasActiveImports: boolean;
}

const ImportContext = createContext<ImportContextValue | null>(null);

export function ImportProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(importReducer, {
    jobs: {},
    activeJobId: null,
    connectionState: 'disconnected',
  });

  const { isAuthenticated } = useAuth();

  // Connect to SignalR when authenticated
  useEffect(() => {
    if (!isAuthenticated) {
      signalRService.disconnect();
      dispatch({ type: 'SET_CONNECTION_STATE', payload: 'disconnected' });
      return;
    }

    const connect = async () => {
      dispatch({ type: 'SET_CONNECTION_STATE', payload: 'connecting' });
      try {
        await signalRService.connect();
      } catch (error) {
        console.error('Failed to connect to SignalR:', error);
      }
    };

    connect();

    const unsubscribeState = signalRService.onStateChange((newState) => {
      dispatch({ type: 'SET_CONNECTION_STATE', payload: newState });
    });

    return () => {
      unsubscribeState();
      // We don't necessarily want to disconnect here if the component just re-renders
      // but we do want to disconnect on unmount or when isAuthenticated becomes false.
    };
  }, [isAuthenticated]);

  // Listen for SignalR events
  useEffect(() => {
    const unsubscribePreview = signalRService.on<[ImportReport]>('OnPreviewReady', (report) => {
      console.log('SignalR: OnPreviewReady', report);
      dispatch({
        type: 'UPDATE_JOB',
        payload: {
          jobId: report.jobId,
          updates: {
            status: 'completed',
            progress: 100,
            report,
          },
        },
      });
    });

    const unsubscribeCompleted = signalRService.on<[ImportSummary]>('OnImportCompleted', (summary) => {
      dispatch({
        type: 'UPDATE_JOB',
        payload: {
          jobId: summary.jobId,
          updates: {
            status: 'completed',
            progress: 100,
            summary,
          },
        },
      });
      toast.success(`Import completed! ${summary.totalImported} products imported.`);
      // Trigger product list refresh event
      window.dispatchEvent(new CustomEvent('products-updated'));
    });

    const unsubscribeFailed = signalRService.on<[string, string]>('OnJobFailed', (jobId, errorMessage) => {
      dispatch({
        type: 'UPDATE_JOB',
        payload: {
          jobId,
          updates: {
            status: 'failed',
            error: errorMessage,
          },
        },
      });
      toast.error(`Import failed: ${errorMessage}`);
    });

    const unsubscribeProgress = signalRService.on<[{ jobId: string; percentage: number; message: string }]>(
      'OnProgress',
      ({ jobId, percentage, message }) => {
        dispatch({
          type: 'UPDATE_JOB',
          payload: {
            jobId,
            updates: {
              progress: percentage,
              progressMessage: message,
            },
          },
        });
      }
    );

    return () => {
      unsubscribePreview();
      unsubscribeCompleted();
      unsubscribeFailed();
      unsubscribeProgress();
    };
  }, []);

  const startJob = useCallback((jobId: string, fileName: string, initialStatus: ImportJobStatus = 'uploading') => {
    dispatch({
      type: 'ADD_JOB',
      payload: {
        jobId,
        fileName,
        status: initialStatus,
        progress: 0,
        createdAt: new Date(),
      },
    });
  }, []);

  const updateJobStatus = useCallback((jobId: string, status: ImportJobStatus) => {
    dispatch({ type: 'UPDATE_JOB', payload: { jobId, updates: { status } } });
  }, []);

  const updateJobProgress = useCallback((jobId: string, progress: number, message?: string) => {
    dispatch({
      type: 'UPDATE_JOB',
      payload: { jobId, updates: { progress, progressMessage: message } },
    });
  }, []);

  const setJobReport = useCallback((jobId: string, report: ImportReport) => {
    dispatch({
      type: 'UPDATE_JOB',
      payload: { jobId, updates: { report, status: 'completed', progress: 100 } },
    });
  }, []);

  const setJobSummary = useCallback((jobId: string, summary: ImportSummary) => {
    dispatch({
      type: 'UPDATE_JOB',
      payload: { jobId, updates: { summary, status: 'completed', progress: 100 } },
    });
  }, []);

  const setJobError = useCallback((jobId: string, error: string) => {
    dispatch({
      type: 'UPDATE_JOB',
      payload: { jobId, updates: { error, status: 'failed' } },
    });
  }, []);

  const removeJob = useCallback((jobId: string) => {
    dispatch({ type: 'REMOVE_JOB', payload: jobId });
  }, []);

  const setActiveJob = useCallback((jobId: string | null) => {
    dispatch({ type: 'SET_ACTIVE_JOB', payload: jobId });
  }, []);

  // Fallback sync for reconnection scenarios
  const syncJobStatus = useCallback(async (jobId: string) => {
    try {
      const statusResponse = await getImportStatus(jobId);
      const statusMap: Record<string, ImportJobStatus> = {
        pending: 'processing',
        processing: 'processing',
        preview_ready: 'preview_ready',
        importing: 'importing',
        completed: 'completed',
        failed: 'failed',
      };

      dispatch({
        type: 'UPDATE_JOB',
        payload: {
          jobId,
          updates: {
            status: statusMap[statusResponse.status] || 'processing',
            error: statusResponse.errorMessage,
          },
        },
      });
    } catch (error) {
      console.error('Failed to sync job status:', error);
    }
  }, []);

  const hasActiveImports = Object.values(state.jobs).some(
    (job) => !['completed', 'failed'].includes(job.status)
  );

  return (
    <ImportContext.Provider
      value={{
        ...state,
        startJob,
        updateJobStatus,
        updateJobProgress,
        setJobReport,
        setJobSummary,
        setJobError,
        removeJob,
        setActiveJob,
        syncJobStatus,
        hasActiveImports,
      }}
    >
      {children}
    </ImportContext.Provider>
  );
}

export function useImport() {
  const context = useContext(ImportContext);
  if (!context) {
    throw new Error('useImport must be used within an ImportProvider');
  }
  return context;
}
