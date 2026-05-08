/** API response shape for a single command history entry. */
export interface CommandHistoryResponse {
  /** Unique history entry identifier (UUID). */
  id: string;
  /** Name of the command that was executed. */
  commandName: string;
  /** ISO timestamp of when the command was executed. */
  timestamp: string;
}
