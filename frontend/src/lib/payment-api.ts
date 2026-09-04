import type {
  PaymentTransaction,
  UpdateStatusPayload,
} from "@/types/payment";

const publicBaseUrl =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:8080";

export const API_BASE_URL =
  typeof window === "undefined"
    ? (process.env.API_BASE_URL ?? publicBaseUrl)
    : publicBaseUrl;

export const TRANSACTION_PATH = "/api/payment/transaction";
export const UPDATE_STATUS_PATH = "/api/payment/update-status";

export const GUID_RE =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    detail: string,
  ) {
    super(detail);
    this.name = "ApiError";
  }
}

async function problemDetail(response: Response): Promise<string> {
  try {
    const body: unknown = await response.json();
    if (typeof body === "object" && body !== null) {
      const record = body as Record<string, unknown>;
      const detail = record["detail"];
      const title = record["title"];
      if (typeof detail === "string" && detail.length > 0) return detail;
      if (typeof title === "string" && title.length > 0) return title;
    }
  } catch {
  }
  return `درخواست با خطا مواجه شد (${response.status}).`;
}

async function toApiError(response: Response): Promise<ApiError> {
  return new ApiError(response.status, await problemDetail(response));
}

export async function fetchTransaction(
  token: string,
): Promise<PaymentTransaction> {
  const response = await fetch(
    `${API_BASE_URL}${TRANSACTION_PATH}/${encodeURIComponent(token)}`,
    { cache: "no-store" },
  );
  if (!response.ok) throw await toApiError(response);
  return (await response.json()) as PaymentTransaction;
}

export async function finalizePayment(
  token: string,
  isSuccess: boolean,
  rrn?: string,
): Promise<void> {
  const payload: UpdateStatusPayload = { token, isSuccess };
  if (isSuccess) payload.rrn = rrn;

  const response = await fetch(`${API_BASE_URL}${UPDATE_STATUS_PATH}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  if (!response.ok) throw await toApiError(response);
}
