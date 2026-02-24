import { useState } from 'react';
import { X, Loader2, CheckCircle, AlertCircle, FileText, ChevronDown, ChevronUp } from 'lucide-react';
import { useImport } from '@/contexts/ImportContext';
import { Progress } from '@/components/ui/progress';

export default function GlobalImportStatus() {
  const { jobs, hasActiveImports, removeJob } = useImport();
  const [isExpanded, setIsExpanded] = useState(true);

  const activeJobs = Object.values(jobs).filter(
    (job) => !['completed', 'failed'].includes(job.status)
  );
  
  const recentCompletedJobs = Object.values(jobs)
    .filter((job) => ['completed', 'failed'].includes(job.status))
    .slice(0, 3);

  const allVisibleJobs = [...activeJobs, ...recentCompletedJobs];

  if (allVisibleJobs.length === 0) {
    return null;
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed':
        return <CheckCircle className="w-4 h-4 text-emerald-500" />;
      case 'failed':
        return <AlertCircle className="w-4 h-4 text-destructive" />;
      default:
        return <Loader2 className="w-4 h-4 text-primary animate-spin" />;
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'uploading':
        return 'Uploading...';
      case 'processing':
        return 'Processing...';
      case 'preview_ready':
        return 'Ready for review';
      case 'confirming':
        return 'Confirming...';
      case 'importing':
        return 'Importing...';
      case 'completed':
        return 'Completed';
      case 'failed':
        return 'Failed';
      default:
        return status;
    }
  };

  return (
    <div className="fixed bottom-4 right-4 z-50 w-80 bg-card border border-border rounded-xl shadow-lg overflow-hidden">
      {/* Header */}
      <button
        onClick={() => setIsExpanded(!isExpanded)}
        className="w-full flex items-center justify-between p-3 bg-secondary hover:bg-secondary/80 transition-colors"
      >
        <div className="flex items-center gap-2">
          <FileText className="w-4 h-4 text-primary" />
          <span className="font-medium text-sm text-foreground">
            Import Status
            {hasActiveImports && (
              <span className="ml-2 inline-flex items-center gap-1 text-xs text-muted-foreground">
                <Loader2 className="w-3 h-3 animate-spin" />
                {activeJobs.length} active
              </span>
            )}
          </span>
        </div>
        {isExpanded ? (
          <ChevronDown className="w-4 h-4 text-muted-foreground" />
        ) : (
          <ChevronUp className="w-4 h-4 text-muted-foreground" />
        )}
      </button>

      {/* Job List */}
      {isExpanded && (
        <div className="max-h-64 overflow-y-auto">
          {allVisibleJobs.map((job) => (
            <div
              key={job.jobId}
              className="p-3 border-t border-border hover:bg-secondary/30 transition-colors"
            >
              <div className="flex items-start justify-between gap-2">
                <div className="flex items-start gap-2 flex-1 min-w-0">
                  {getStatusIcon(job.status)}
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-foreground truncate">
                      {job.fileName}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      {job.progressMessage || getStatusText(job.status)}
                    </p>
                    {job.summary && (
                      <p className="text-xs text-emerald-600 dark:text-emerald-400 mt-0.5">
                        {job.summary.totalImported} imported, {job.summary.totalFailed} failed
                      </p>
                    )}
                    {job.error && (
                      <p className="text-xs text-destructive mt-0.5 truncate">
                        {job.error}
                      </p>
                    )}
                  </div>
                </div>
                {['completed', 'failed'].includes(job.status) && (
                  <button
                    onClick={() => removeJob(job.jobId)}
                    className="p-1 rounded hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                  >
                    <X className="w-3 h-3" />
                  </button>
                )}
              </div>

              {/* Progress bar for active jobs */}
              {!['completed', 'failed', 'preview_ready'].includes(job.status) && (
                <div className="mt-2">
                  <Progress value={job.progress} className="h-1" />
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
