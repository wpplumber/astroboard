import { format } from "date-fns";

export function formatToFullTimestamp(dateActivity: string): string {
  return format(new Date(dateActivity), "MMM dd, yyyy HH:mm");
}
