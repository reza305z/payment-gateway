import Button from "@/components/ui/Button";
import { formatAmount } from "@/lib/format";

export type BusyState = "pay" | "cancel" | null;

export default function PendingPaymentCard({
  amount,
  reservationNumber,
  busy,
  error,
  onPay,
  onCancel,
}: {
  amount: number;
  reservationNumber: string | null;
  busy: BusyState;
  error: string | null;
  onPay: () => void;
  onCancel: () => void;
}) {
  return (
    <section className="w-full max-w-md rounded-2xl border border-zinc-200 bg-white p-6 shadow-sm">
      <h1 className="text-center text-lg font-bold">پرداخت آنلاین</h1>

      <div className="mt-6 rounded-xl bg-zinc-50 p-4 text-center">
        <p className="text-sm text-zinc-500">مبلغ قابل پرداخت</p>
        <p className="mt-1 text-4xl font-black tracking-tight text-zinc-900">
          {formatAmount(amount)}
        </p>
        <p className="mt-1 text-xs text-zinc-400">تومان</p>
      </div>

      {reservationNumber ? (
        <p className="mt-4 text-center text-xs text-zinc-500">
          شماره رزرو:{" "}
          <span dir="ltr" className="font-medium text-zinc-700">
            {reservationNumber}
          </span>
        </p>
      ) : null}

      <div className="mt-6 flex flex-col gap-3">
        <Button onClick={onPay} disabled={busy !== null}>
          {busy === "pay" ? "در حال پرداخت…" : "پرداخت موفق"}
        </Button>
        <Button variant="outline" onClick={onCancel} disabled={busy !== null}>
          {busy === "cancel" ? "در حال انصراف…" : "انصراف"}
        </Button>
      </div>

      {error ? (
        <p className="mt-4 rounded-lg bg-red-50 px-3 py-2 text-center text-xs leading-6 text-red-700">
          {error}
        </p>
      ) : null}
    </section>
  );
}
