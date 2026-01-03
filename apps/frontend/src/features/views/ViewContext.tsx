"use client";

import React, { createContext, useContext, useState, useCallback } from "react";

export type ViewType = "list" | "board" | "calendar" | "gantt";

interface ViewContextValue {
  currentView: ViewType;
  setCurrentView: (view: ViewType) => void;
  viewPreferences: Record<string, unknown>;
  setViewPreference: (key: string, value: unknown) => void;
}

const ViewContext = createContext<ViewContextValue | undefined>(undefined);

export function ViewProvider({ children }: { children: React.ReactNode }) {
  const [currentView, setCurrentViewState] = useState<ViewType>("list");
  const [viewPreferences, setViewPreferencesState] = useState<Record<string, unknown>>({});

  const setCurrentView = useCallback((view: ViewType) => {
    setCurrentViewState(view);
    // Persist to localStorage
    if (typeof window !== "undefined") {
      localStorage.setItem("preferredView", view);
    }
  }, []);

  const setViewPreference = useCallback((key: string, value: unknown) => {
    setViewPreferencesState((prev) => {
      const updated = { ...prev, [key]: value };
      // Persist to localStorage
      if (typeof window !== "undefined") {
        localStorage.setItem(`viewPrefs_${key}`, JSON.stringify(value));
      }
      return updated;
    });
  }, []);

  return (
    <ViewContext.Provider value={{ currentView, setCurrentView, viewPreferences, setViewPreference }}>
      {children}
    </ViewContext.Provider>
  );
}

export function useViewContext() {
  const context = useContext(ViewContext);
  if (!context) {
    throw new Error("useViewContext must be used within ViewProvider");
  }
  return context;
}
