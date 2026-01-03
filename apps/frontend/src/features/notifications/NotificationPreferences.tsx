"use client";

import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

export interface NotificationPreference {
  taskAssigned: boolean;
  commentMentioned: boolean;
  statusChanged: boolean;
  dueDateReminder: boolean;
  projectInvitation: boolean;
  inAppEnabled: boolean;
  emailEnabled: boolean;
  quietHoursEnabled: boolean;
  quietHoursStart?: string; // HH:MM format
  quietHoursEnd?: string; // HH:MM format
}

interface NotificationPreferencesProps {
  preferences: NotificationPreference;
  onChange: (preferences: NotificationPreference) => void;
  className?: string;
}

export function NotificationPreferences({
  preferences,
  onChange,
  className = ""
}: NotificationPreferencesProps) {
  const updatePreference = <K extends keyof NotificationPreference>(
    key: K,
    value: NotificationPreference[K]
  ) => {
    onChange({ ...preferences, [key]: value });
  };

  return (
    <div className={className}>
      <Card>
        <CardHeader>
          <CardTitle>Notification Preferences</CardTitle>
          <CardDescription>
            Choose which notifications you want to receive and how
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          {/* Notification Types */}
          <div className="space-y-4">
            <h3 className="text-sm font-medium">Notification Types</h3>
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="task-assigned">Task Assigned</Label>
                  <p className="text-xs text-muted-foreground">
                    When someone assigns you to a task
                  </p>
                </div>
                <Switch
                  id="task-assigned"
                  checked={preferences.taskAssigned}
                  onCheckedChange={(checked) =>
                    updatePreference("taskAssigned", checked)
                  }
                />
              </div>

              <Separator />

              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="comment-mentioned">Comment Mentions</Label>
                  <p className="text-xs text-muted-foreground">
                    When someone mentions you in a comment
                  </p>
                </div>
                <Switch
                  id="comment-mentioned"
                  checked={preferences.commentMentioned}
                  onCheckedChange={(checked) =>
                    updatePreference("commentMentioned", checked)
                  }
                />
              </div>

              <Separator />

              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="status-changed">Status Changes</Label>
                  <p className="text-xs text-muted-foreground">
                    When a task status changes
                  </p>
                </div>
                <Switch
                  id="status-changed"
                  checked={preferences.statusChanged}
                  onCheckedChange={(checked) =>
                    updatePreference("statusChanged", checked)
                  }
                />
              </div>

              <Separator />

              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="due-date-reminder">Due Date Reminders</Label>
                  <p className="text-xs text-muted-foreground">
                    Reminders before task due dates
                  </p>
                </div>
                <Switch
                  id="due-date-reminder"
                  checked={preferences.dueDateReminder}
                  onCheckedChange={(checked) =>
                    updatePreference("dueDateReminder", checked)
                  }
                />
              </div>

              <Separator />

              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="project-invitation">Project Invitations</Label>
                  <p className="text-xs text-muted-foreground">
                    When someone invites you to a project
                  </p>
                </div>
                <Switch
                  id="project-invitation"
                  checked={preferences.projectInvitation}
                  onCheckedChange={(checked) =>
                    updatePreference("projectInvitation", checked)
                  }
                />
              </div>
            </div>
          </div>

          <Separator />

          {/* Delivery Methods */}
          <div className="space-y-4">
            <h3 className="text-sm font-medium">Delivery Methods</h3>
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="in-app">In-App Notifications</Label>
                  <p className="text-xs text-muted-foreground">
                    Show notifications in the app
                  </p>
                </div>
                <Switch
                  id="in-app"
                  checked={preferences.inAppEnabled}
                  onCheckedChange={(checked) =>
                    updatePreference("inAppEnabled", checked)
                  }
                />
              </div>

              <Separator />

              <div className="flex items-center justify-between">
                <div className="space-y-0.5">
                  <Label htmlFor="email">Email Notifications</Label>
                  <p className="text-xs text-muted-foreground">
                    Receive notifications via email
                  </p>
                </div>
                <Switch
                  id="email"
                  checked={preferences.emailEnabled}
                  onCheckedChange={(checked) =>
                    updatePreference("emailEnabled", checked)
                  }
                />
              </div>
            </div>
          </div>

          <Separator />

          {/* Quiet Hours */}
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label htmlFor="quiet-hours">Quiet Hours</Label>
                <p className="text-xs text-muted-foreground">
                  Disable notifications during specific hours
                </p>
              </div>
              <Switch
                id="quiet-hours"
                checked={preferences.quietHoursEnabled}
                onCheckedChange={(checked) =>
                  updatePreference("quietHoursEnabled", checked)
                }
              />
            </div>

            {preferences.quietHoursEnabled && (
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="quiet-hours-start">Start Time</Label>
                  <input
                    id="quiet-hours-start"
                    type="time"
                    value={preferences.quietHoursStart || "22:00"}
                    onChange={(e) =>
                      updatePreference("quietHoursStart", e.target.value)
                    }
                    className="w-full px-3 py-2 border border-input rounded-md"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="quiet-hours-end">End Time</Label>
                  <input
                    id="quiet-hours-end"
                    type="time"
                    value={preferences.quietHoursEnd || "08:00"}
                    onChange={(e) =>
                      updatePreference("quietHoursEnd", e.target.value)
                    }
                    className="w-full px-3 py-2 border border-input rounded-md"
                  />
                </div>
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
