import { useState, useRef, useEffect } from 'react';
import { X, Upload, FileText, CheckCircle, AlertCircle, Download, Loader2, Wifi, WifiOff } from 'lucide-react';
import { toast } from 'sonner';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Progress } from '@/components/ui/progress';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { useImport } from '@/contexts/ImportContext';
import { uploadForPreview, confirmImport } from '@/services/importApi';

interface CSVImportModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function CSVImportModal({ isOpen, onClose }: CSVImportModalProps) {
  const [file, setFile] = useState<File | null>(null);
  const [isDragOver, setIsDragOver] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const {
    jobs,
    activeJobId,
    connectionState,
    startJob,
    updateJobStatus,
    updateJobProgress,
    removeJob,
    setActiveJob,
    syncJobStatus,
  } = useImport();

  const activeJob = activeJobId ? jobs[activeJobId] : null;

  // Sync job status on reconnection
  useEffect(() => {
    if (connectionState === 'connected' && activeJobId && activeJob?.status === 'processing') {
      syncJobStatus(activeJobId);
    }
  }, [connectionState, activeJobId, activeJob?.status, syncJobStatus]);

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(true);
  };

  const handleDragLeave = () => {
    setIsDragOver(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
    const droppedFile = e.dataTransfer.files[0];
    if (droppedFile?.type === 'text/csv' || droppedFile?.name.endsWith('.csv')) {
      setFile(droppedFile);
    } else {
      toast.error('Please upload a CSV file');
    }
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (selectedFile) {
      setFile(selectedFile);
    }
  };

  const handleUploadForPreview = async () => {
    if (!file) return;

    try {
      // Generate ID on frontend
      const jobId = crypto.randomUUID();

      // Start the job immediately with real ID
      startJob(jobId, file.name);
      //updateJobProgress(jobId, 0, 'Uploading file...');

      // Upload to server
      await uploadForPreview(file, jobId);

      // Update status on success
      //updateJobStatus(jobId, 'processing');
      //updateJobProgress(jobId, 20, 'Processing CSV...');

      toast.info('File uploaded. Processing in background...');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to upload file');
      // If upload failed, we might want to mark job as failed or remove it
      // For now, let's leave it as 'uploading' with error or user can retry
    }
  };

  const handleConfirmImport = async () => {
    if (!activeJobId || !activeJob?.report) return;

    try {
      const previewJobId = activeJobId;
      const importJobId = crypto.randomUUID();

      // Start the new import job
      startJob(importJobId, activeJob.fileName, 'importing');
      setActiveJob(importJobId);

      // Call API with both IDs
      await confirmImport(previewJobId, importJobId);

      updateJobProgress(importJobId, 0, 'Starting import...');

      toast.info('Import started. You can navigate away - we\'ll notify you when done.');
      handleClose();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to confirm import');
      // If failed, revert to preview job?
      // setActiveJob(activeJobId); // Original preview job
      // updateJobStatus(activeJobId, 'preview_ready');
    }
  };

  const handleClose = () => {
    setFile(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
    onClose();
  };

  const handleCancelJob = () => {
    if (activeJobId) {
      removeJob(activeJobId);
    }
    setFile(null);
  };

  const downloadTemplate = () => {
    const template = 'name,description,category,price,quantity,supplier,lowStockAlertThreshold\nSample Product,Product description,Electronics,99.99,100,Sample Supplier,10';
    const blob = new Blob([template], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'products_template.csv';
    a.click();
    URL.revokeObjectURL(url);
  };

  const validCount = activeJob?.report?.succeededCount ?? 0;
  const invalidCount = activeJob?.report?.failedCount ?? 0;
  const rowResults = activeJob?.report?.rowResults ?? [];

  const isUploading = activeJob?.status === 'uploading';
  const isProcessing = activeJob?.status === 'processing';
  const isPreviewReady = activeJob?.status === 'preview_ready';
  const isConfirming = activeJob?.status === 'confirming';
  const isImporting = activeJob?.status === 'importing';
  const isFailed = activeJob?.status === 'failed';

  const showUploadArea = !activeJob || isFailed;
  const showProcessing = isUploading || isProcessing || isConfirming || isImporting;
  const showPreview = (isPreviewReady || (activeJob?.status === 'completed' && activeJob.report)) && activeJob?.report;

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Upload className="w-5 h-5 text-primary" />
            Import Products from CSV
            {/* Connection indicator */}
            <span className="ml-auto flex items-center gap-1 text-xs font-normal">
              {connectionState === 'connected' ? (
                <>
                  <Wifi className="w-3 h-3 text-emerald-500" />
                  <span className="text-muted-foreground">Connected</span>
                </>
              ) : connectionState === 'reconnecting' ? (
                <>
                  <Loader2 className="w-3 h-3 text-amber-500 animate-spin" />
                  <span className="text-amber-500">Reconnecting...</span>
                </>
              ) : (
                <>
                  <WifiOff className="w-3 h-3 text-destructive" />
                  <span className="text-destructive">Disconnected</span>
                </>
              )}
            </span>
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-6">
          {/* Download Template */}
          <div className="flex items-center justify-between p-4 bg-secondary rounded-xl">
            <div>
              <p className="font-medium text-foreground">Need a template?</p>
              <p className="text-sm text-muted-foreground">Download our CSV template to get started</p>
            </div>
            <button
              onClick={downloadTemplate}
              className="flex items-center gap-2 px-4 py-2 bg-card border border-border text-foreground rounded-lg hover:bg-card/80 transition-colors text-sm font-medium"
            >
              <Download className="w-4 h-4" />
              Download Template
            </button>
          </div>

          {/* Upload Area */}
          {showUploadArea && (
            <>
              {isFailed && activeJob?.error && (
                <div className="p-4 bg-destructive/10 border border-destructive/20 rounded-xl">
                  <div className="flex items-center gap-2 text-destructive">
                    <AlertCircle className="w-5 h-5" />
                    <span className="font-medium">Import Failed</span>
                  </div>
                  <p className="text-sm text-destructive/80 mt-1">{activeJob.error}</p>
                  <button
                    onClick={handleCancelJob}
                    className="mt-2 text-sm text-primary hover:underline"
                  >
                    Try again
                  </button>
                </div>
              )}

              <div
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
                onDrop={handleDrop}
                onClick={() => fileInputRef.current?.click()}
                className={`border-2 border-dashed rounded-xl p-8 text-center cursor-pointer transition-all ${isDragOver
                  ? 'border-primary bg-primary/5'
                  : 'border-border hover:border-primary/50 hover:bg-secondary/50'
                  }`}
              >
                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".csv"
                  onChange={handleFileSelect}
                  className="hidden"
                />
                <Upload className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
                <p className="text-foreground font-medium">Drop your CSV file here or click to browse</p>
                <p className="text-sm text-muted-foreground mt-1">Supports .csv files up to 10MB</p>
              </div>

              {file && !isFailed && (
                <div className="flex items-center gap-4 p-4 bg-secondary rounded-xl">
                  <div className="p-3 bg-primary/10 rounded-lg">
                    <FileText className="w-6 h-6 text-primary" />
                  </div>
                  <div className="flex-1">
                    <p className="font-medium text-foreground">{file.name}</p>
                    <p className="text-sm text-muted-foreground">
                      {(file.size / 1024).toFixed(2)} KB
                    </p>
                  </div>
                  <button
                    onClick={() => setFile(null)}
                    className="p-2 rounded-lg hover:bg-card text-muted-foreground hover:text-foreground transition-colors"
                  >
                    <X className="w-5 h-5" />
                  </button>
                </div>
              )}

              {file && !isFailed && (
                <div className="flex justify-end gap-3">
                  <button
                    onClick={handleClose}
                    className="px-4 py-2 bg-secondary text-foreground rounded-lg hover:bg-secondary/80 transition-colors font-medium"
                  >
                    Cancel
                  </button>
                  <button
                    onClick={handleUploadForPreview}
                    disabled={connectionState !== 'connected'}
                    className="px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Upload & Preview
                  </button>
                </div>
              )}
            </>
          )}

          {/* Processing State */}
          {showProcessing && (
            <div className="space-y-4">
              <div className="flex items-center gap-4 p-4 bg-secondary rounded-xl">
                <div className="p-3 bg-primary/10 rounded-lg">
                  <FileText className="w-6 h-6 text-primary" />
                </div>
                <div className="flex-1">
                  <p className="font-medium text-foreground">{activeJob?.fileName || file?.name}</p>
                  <p className="text-sm text-muted-foreground">
                    {activeJob?.progressMessage || 'Processing...'}
                  </p>
                </div>
                <Loader2 className="w-5 h-5 text-primary animate-spin" />
              </div>

              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Progress</span>
                  <span className="text-foreground font-medium">{activeJob?.progress || 0}%</span>
                </div>
                <Progress value={activeJob?.progress || 0} className="h-2" />
              </div>

              <p className="text-sm text-center text-muted-foreground">
                {isConfirming || isImporting
                  ? 'You can close this modal and continue working. We\'ll notify you when the import is complete.'
                  : 'Processing your file. This may take a moment for large files.'}
              </p>
            </div>
          )}

          {/* Preview State */}
          {showPreview && (
            <>
              <div className="flex items-center gap-4 p-4 bg-secondary rounded-xl">
                <div className="p-3 bg-primary/10 rounded-lg">
                  <FileText className="w-6 h-6 text-primary" />
                </div>
                <div className="flex-1">
                  <p className="font-medium text-foreground">{activeJob?.fileName}</p>
                  <p className="text-sm text-muted-foreground">
                    {rowResults.length} rows found
                  </p>
                </div>
                <button
                  onClick={handleCancelJob}
                  className="p-2 rounded-lg hover:bg-card text-muted-foreground hover:text-foreground transition-colors"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>

              {/* Validation Summary */}
              <div className="flex gap-4">
                <div className="flex-1 p-4 bg-emerald-500/10 border border-emerald-500/20 rounded-xl">
                  <div className="flex items-center gap-2">
                    <CheckCircle className="w-5 h-5 text-emerald-500" />
                    <span className="font-medium text-emerald-700 dark:text-emerald-400">{validCount} Valid</span>
                  </div>
                  <p className="text-sm text-emerald-600 dark:text-emerald-300 mt-1">Ready to import</p>
                </div>
                {invalidCount > 0 && (
                  <div className="flex-1 p-4 bg-destructive/10 border border-destructive/20 rounded-xl">
                    <div className="flex items-center gap-2">
                      <AlertCircle className="w-5 h-5 text-destructive" />
                      <span className="font-medium text-destructive">{invalidCount} Invalid</span>
                    </div>
                    <p className="text-sm text-destructive/80 mt-1">Will be skipped</p>
                  </div>
                )}
              </div>

              {/* Preview Table */}
              <div className="border border-border rounded-xl overflow-hidden max-h-64 overflow-y-auto">
                <table className="w-full text-sm">
                  <thead className="bg-secondary sticky top-0">
                    <tr>
                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Status</th>
                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Name</th>
                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Category</th>
                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Price</th>
                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Qty</th>
                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Low Stock</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-border">
                    {rowResults.map((row, index) => (
                      <tr key={index} className={!row.isValid ? 'bg-destructive/5' : ''}>
                        <td className="px-4 py-3">
                          {row.isValid ? (
                            <CheckCircle className="w-4 h-4 text-emerald-500" />
                          ) : (
                            <TooltipProvider>
                              <Tooltip>
                                <TooltipTrigger>
                                  <AlertCircle className="w-4 h-4 text-destructive cursor-help" />
                                </TooltipTrigger>
                                <TooltipContent>
                                  <div className="space-y-1">
                                    <p className="font-semibold text-destructive">Validation Errors:</p>
                                    <ul className="list-disc list-inside text-xs">
                                      {row.errors.map((err, i) => (
                                        <li key={i}>{err}</li>
                                      ))}
                                    </ul>
                                  </div>
                                </TooltipContent>
                              </Tooltip>
                            </TooltipProvider>
                          )}
                        </td>
                        <td className="px-4 py-3 text-foreground">{row.name || '-'}</td>
                        <td className="px-4 py-3 text-muted-foreground">{row.category || '-'}</td>
                        <td className="px-4 py-3 text-foreground">${row.price || '-'}</td>
                        <td className="px-4 py-3 text-foreground">{row.quantity || '-'}</td>
                        <td className="px-4 py-3 text-foreground">{row.lowStockAlertThreshold || '-'}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Actions */}
              <div className="flex justify-end gap-3">
                <button
                  onClick={handleCancelJob}
                  className="px-4 py-2 bg-secondary text-foreground rounded-lg hover:bg-secondary/80 transition-colors font-medium"
                >
                  Cancel
                </button>
                <button
                  onClick={handleConfirmImport}
                  disabled={validCount === 0}
                  className="px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Import {validCount} Products
                </button>
              </div>
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
