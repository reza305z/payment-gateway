import { PaymentStatus } from "@/types/payment";

export function formatAmount(amount: number): string {
  return new Intl.NumberFormat("fa-IR").format(amount);
}

const STATUS_LABELS: Record<PaymentStatus, string> = {
  [PaymentStatus.Pending]: "در انتظار پرداخت",
  [PaymentStatus.Success]: "پرداخت موفق",
  [PaymentStatus.Failed]: "ناموفق",
  [PaymentStatus.Expired]: "منقضی‌شده",
};

export function statusLabel(status: string): string {
  return STATUS_LABELS[status as PaymentStatus] ?? "نامشخص";
}

export type StatusTone = "neutral" | "success" | "danger" | "warning";

export function statusTone(status: string): StatusTone {
  switch (status as PaymentStatus) {
    case PaymentStatus.Success:
      return "success";
    case PaymentStatus.Failed:
      return "danger";
    case PaymentStatus.Expired:
      return "warning";
    default:
      return "neutral";
  }
}

const TONE_CLASSES: Record<StatusTone, string> = {
  neutral: "bg-zinc-100 text-zinc-800 border-zinc-200",
  success: "bg-emerald-50 text-emerald-800 border-emerald-200",
  danger: "bg-red-50 text-red-800 border-red-200",
  warning: "bg-amber-50 text-amber-800 border-amber-200",
};

export function toneClasses(tone: StatusTone): string {
  return TONE_CLASSES[tone];
}
