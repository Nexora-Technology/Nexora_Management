import { Calendar } from "lucide-react";

export default function CalendarPage() {
  return (
    <div className="h-full flex items-center justify-center">
      <div className="text-center">
        <Calendar className="w-16 h-16 text-gray-300 mx-auto mb-4" />
        <h1 className="text-2xl font-bold text-gray-900 mb-2">Calendar</h1>
        <p className="text-gray-500">View and manage your schedule</p>
      </div>
    </div>
  );
}
