import { formatAmount, statusLabel, statusTone, toneClasses } from "@/lib/format";
import {
  PaymentStatus,
  type PaymentTransaction,
} from "@/types/payment";

export default function PaymentResultCard({
  transaction,
}: {
  transaction: PaymentTransaction;
}) {
  const { status, amount, rrn } = transaction;
  const tone = toneClasses(statusTone(status));

  return (
    <section
      className={`w-full max-w-md rounded-2xl border p-6 text-center ${tone}`}
    >
      <p className="text-2xl font-bold">{statusLabel(status)}</p>

      {status === PaymentStatus.Success && rrn ? (
        <p className="mt-4 text-sm leading-7 opacity-90">
          شماره پیگیری:{" "}
          <span dir="ltr" className="font-mono text-base font-bold">
            {rrn}
          </span>
        </p>
      ) : null}

      <p className="mt-4 text-sm opacity-80">
        مبلغ: {formatAmount(amount)} تومان
      </p>
    </section>
  );
}
