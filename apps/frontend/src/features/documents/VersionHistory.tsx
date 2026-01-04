"use client";

import { formatDistanceToNow } from "date-fns";
import { History, User, RotateCcw, FileText } from "lucide-react";
import { PageVersion } from "./types";
import { cn } from "@/lib/utils";

interface VersionHistoryProps {
  versions: PageVersion[];
  currentVersionNumber?: number;
  onRestore?: (versionNumber: number) => void;
  onPreview?: (versionNumber: number) => void;
  className?: string;
}

export function VersionHistory({
  versions,
  currentVersionNumber,
  onRestore,
  onPreview,
  className = "",
}: VersionHistoryProps) {
  if (versions.length === 0) {
    return (
      <div
        className={cn(
          "flex flex-col items-center justify-center py-12 text-gray-500",
          className
        )}
      >
        <History size={48} className="text-gray-300 mb-4" />
        <p className="text-sm">No version history available</p>
      </div>
    );
  }

  return (
    <div className={cn("divide-y divide-gray-200", className)}>
      {versions.map((version) => (
        <div
          key={version.id}
          className={cn(
            "flex items-start gap-3 p-4 transition-colors",
            version.versionNumber === currentVersionNumber &&
              "bg-blue-50 border-l-4 border-l-blue-500"
          )}
        >
          {/* Version Icon */}
          <div className="flex-shrink-0 mt-1">
            <FileText
              size={18}
              className={cn(
                version.versionNumber === currentVersionNumber
                  ? "text-blue-500"
                  : "text-gray-400"
              )}
            />
          </div>

          {/* Content */}
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2">
              <span className="font-medium text-gray-900">
                Version {version.versionNumber}
              </span>
              {version.versionNumber === currentVersionNumber && (
                <span className="text-xs bg-blue-500 text-white px-2 py-0.5 rounded">
                  Current
                </span>
              )}
            </div>

            {/* Commit message */}
            {version.commitMessage && (
              <p className="text-sm text-gray-600 mt-1 line-clamp-2">
                {version.commitMessage}
              </p>
            )}

            {/* Meta info */}
            <div className="flex items-center gap-3 mt-2 text-xs text-gray-500">
              <span className="flex items-center gap-1">
                <User size={12} />
                {version.createdByName || "Unknown"}
              </span>
              <span>
                {formatDistanceToNow(new Date(version.createdAt), {
                  addSuffix: true,
                })}
              </span>
            </div>
          </div>

          {/* Actions */}
          <div className="flex flex-col gap-2">
            {onPreview && version.versionNumber !== currentVersionNumber && (
              <button
                onClick={() => onPreview(version.versionNumber)}
                className="text-xs text-blue-600 hover:text-blue-800 transition-colors"
              >
                Preview
              </button>
            )}
            {onRestore && version.versionNumber !== currentVersionNumber && (
              <button
                onClick={() => onRestore(version.versionNumber)}
                className="flex items-center gap-1 text-xs text-gray-600 hover:text-gray-800 transition-colors"
              >
                <RotateCcw size={12} />
                Restore
              </button>
            )}
          </div>
        </div>
      ))}
    </div>
  );
}

// Compact version list for sidebar
interface VersionListProps {
  versions: PageVersion[];
  currentVersionNumber?: number;
  onSelectVersion?: (versionNumber: number) => void;
  className?: string;
}

export function VersionList({
  versions,
  currentVersionNumber,
  onSelectVersion,
  className = "",
}: VersionListProps) {
  return (
    <div className={cn("space-y-1", className)}>
      {versions.map((version) => (
        <button
          key={version.id}
          onClick={() => onSelectVersion?.(version.versionNumber)}
          className={cn(
            "w-full flex items-center gap-2 px-3 py-2 rounded-md text-left transition-colors",
            version.versionNumber === currentVersionNumber
              ? "bg-blue-100 text-blue-900"
              : "hover:bg-gray-100"
          )}
        >
          <span className="text-sm font-medium">v{version.versionNumber}</span>
          <span className="text-xs text-gray-500 flex-1 truncate">
            {version.commitMessage || "No message"}
          </span>
        </button>
      ))}
    </div>
  );
}
