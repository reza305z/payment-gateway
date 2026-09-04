"use client";

import { useRef, useState } from "react";

import { ApiError, fetchTransaction, finalizePayment } from "@/lib/payment-api";
import { generateRrn } from "@/lib/rrn";
import {
  PaymentStatus,
  type PaymentTransaction,
} from "@/types/payment";

import PaymentResultCard from "./PaymentResultCard";
import PendingPaymentCard, { type BusyState } from "./PendingPaymentCard";

export default function PaymentActions({
  token,
  initial,
}: {
  token: string;
  initial: PaymentTransaction;
}) {
  const [transaction, setTransaction] =
    useState<PaymentTransaction>(initial);
  const [busy, setBusy] = useState<BusyState>(null);
  const [error, setError] = useState<string | null>(null);
  const inFlight = useRef(false);

  async function submit(isSuccess: boolean) {
    if (inFlight.current) return;
    inFlight.current = true;
    setBusy(isSuccess ? "pay" : "cancel");
    setError(null);

    let navigating = false;
    try {
      if (isSuccess) {
        await finalizePayment(token, true, generateRrn());
      } else {
        await finalizePayment(token, false);
      }
      window.location.assign(transaction.redirectUrl);
      navigating = true;
    } catch (err) {
      if (err instanceof ApiError && err.status === 400) {
        // The background job may have flipped the row between page load and
        // click — re-sync from the source of truth.
        try {
          const fresh = await fetchTransaction(token);
          if (fresh.status === PaymentStatus.Success) {
            window.location.assign(fresh.redirectUrl);
            navigating = true;
          } else {
            setTransaction(fresh);
          }
        } catch {
          setError("خطا در دریافت وضعیت تراکنش. دوباره تلاش کنید.");
        }
      } else {
        setError(
          err instanceof Error
            ? err.message
            : "خطا در برقراری ارتباط با سرور.",
        );
      }
    } finally {
      inFlight.current = false;
      if (!navigating) setBusy(null);
    }
  }

  if (transaction.status === PaymentStatus.Pending) {
    return (
      <PendingPaymentCard
        amount={transaction.amount}
        reservationNumber={transaction.reservationNumber}
        busy={busy}
        error={error}
        onPay={() => void submit(true)}
        onCancel={() => void submit(false)}
      />
    );
  }

  return <PaymentResultCard transaction={transaction} />;
}
