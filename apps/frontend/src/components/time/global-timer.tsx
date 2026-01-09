"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { timeService, TimeEntry } from "@/lib/services/time-service";
import { format } from "date-fns";
import { Play, Pause, Clock } from "lucide-react";

interface GlobalTimerProps {
  onTimerUpdate?: (entry: TimeEntry | null) => void;
}

export function GlobalTimer({ onTimerUpdate }: GlobalTimerProps) {
  const [activeEntry, setActiveEntry] = useState<TimeEntry | null>(null);
  const [elapsed, setElapsed] = useState(0);
  const [description, setDescription] = useState("");
  const [isBillable, setIsBillable] = useState(false);
  const [loading, setLoading] = useState(false);

  // Load active timer on mount
  useEffect(() => {
    loadActiveTimer();
  }, []);

  // Update elapsed time every second when timer is active
  useEffect(() => {
    let interval: NodeJS.Timeout;
    if (activeEntry && !activeEntry.endTime) {
      interval = setInterval(() => {
        const startTime = new Date(activeEntry.startTime).getTime();
        const now = Date.now();
        setElapsed(Math.floor((now - startTime) / 1000));
      }, 1000);
    }
    return () => clearInterval(interval);
  }, [activeEntry]);

  // Sync with localStorage for cross-tab persistence
  useEffect(() => {
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === "active_timer") {
        if (e.newValue) {
          setActiveEntry(JSON.parse(e.newValue));
        } else {
          setActiveEntry(null);
          setElapsed(0);
        }
      }
    };

    window.addEventListener("storage", handleStorageChange);
    return () => window.removeEventListener("storage", handleStorageChange);
  }, []);

  // Auto-idle detection (5 minutes of inactivity)
  useEffect(() => {
    let idleTimeout: NodeJS.Timeout;

    const resetIdleTimer = () => {
      clearTimeout(idleTimeout);
      if (activeEntry && !activeEntry.endTime) {
        idleTimeout = setTimeout(() => {
          // Auto-pause timer after 5 minutes of inactivity
          stopTimer(true);
        }, 5 * 60 * 1000);
      }
    };

    const events = ["mousedown", "keydown", "scroll", "touchstart"];
    events.forEach((event) => window.addEventListener(event, resetIdleTimer));
    resetIdleTimer();

    return () => {
      clearTimeout(idleTimeout);
      events.forEach((event) => window.removeEventListener(event, resetIdleTimer));
    };
  }, [activeEntry]);

  const loadActiveTimer = async () => {
    try {
      const entry = await timeService.getActiveTimer();
      if (entry) {
        setActiveEntry(entry);
        setDescription(entry.description || "");
        setIsBillable(entry.isBillable);

        // Sync to localStorage
        localStorage.setItem("active_timer", JSON.stringify(entry));
      } else {
        localStorage.removeItem("active_timer");
      }
    } catch (error) {
      console.error("Failed to load active timer:", error);
    }
  };

  const startTimer = async () => {
    setLoading(true);
    try {
      const entry = await timeService.startTimer({
        description,
        isBillable,
      });
      setActiveEntry(entry);
      setDescription("");
      setIsBillable(false);

      // Sync to localStorage
      localStorage.setItem("active_timer", JSON.stringify(entry));
      onTimerUpdate?.(entry);
    } catch (error) {
      console.error("Failed to start timer:", error);
    } finally {
      setLoading(false);
    }
  };

  const stopTimer = async (autoPaused = false) => {
    setLoading(true);
    try {
      const entry = await timeService.stopTimer({
        description: description || undefined,
      });
      setActiveEntry(null);
      setElapsed(0);
      setDescription("");

      // Remove from localStorage
      localStorage.removeItem("active_timer");
      onTimerUpdate?.(null);

      if (autoPaused) {
        console.log("Timer auto-paused due to inactivity");
      }
    } catch (error) {
      console.error("Failed to stop timer:", error);
    } finally {
      setLoading(false);
    }
  };

  const formatTime = (seconds: number) => {
    const hrs = Math.floor(seconds / 3600);
    const mins = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hrs.toString().padStart(2, "0")}:${mins.toString().padStart(2, "0")}:${secs.toString().padStart(2, "0")}`;
  };

  return (
    <Card className="p-6">
      <div className="flex items-center gap-4">
        <div className="flex-1">
          <div className="flex items-center gap-2 mb-2">
            <Clock className="h-5 w-5 text-muted-foreground" />
            <span className="text-sm font-medium">Time Tracker</span>
          </div>

          {activeEntry && !activeEntry.endTime ? (
            <>
              <div className="text-3xl font-mono font-bold mb-2">
                {formatTime(elapsed)}
              </div>
              <div className="text-xs text-muted-foreground mb-3">
                Started at {format(new Date(activeEntry.startTime), "HH:mm")}
              </div>
            </>
          ) : (
            <div className="text-3xl font-mono font-bold mb-2 text-muted-foreground">
              00:00:00
            </div>
          )}

          <div className="space-y-2">
            <Textarea
              placeholder="What are you working on?"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="min-h-[60px] resize-none"
              disabled={!!activeEntry && !activeEntry.endTime}
            />

            <div className="flex items-center gap-2">
              <Switch
                id="billable"
                checked={isBillable}
                onCheckedChange={setIsBillable}
                disabled={!!activeEntry && !activeEntry.endTime}
              />
              <Label htmlFor="billable" className="text-sm">
                Billable
              </Label>
            </div>
          </div>
        </div>

        <div className="flex flex-col gap-2">
          {activeEntry && !activeEntry.endTime ? (
            <Button
              onClick={() => stopTimer()}
              disabled={loading}
              size="lg"
              variant="destructive"
            >
              <Pause className="h-4 w-4 mr-2" />
              Stop
            </Button>
          ) : (
            <Button
              onClick={startTimer}
              disabled={loading || !description.trim()}
              size="lg"
            >
              <Play className="h-4 w-4 mr-2" />
              Start
            </Button>
          )}
        </div>
      </div>
    </Card>
  );
}
