import { notFound } from "next/navigation";

import PaymentActions from "@/components/gateway/PaymentActions";
import TransactionErrorCard from "@/components/gateway/TransactionErrorCard";
import { ApiError, fetchTransaction, GUID_RE } from "@/lib/payment-api";
import type { PaymentTransaction } from "@/types/payment";

export default async function GatewayPage({
  params,
}: {
  params: Promise<{ token: string }>;
}) {
  const { token } = await params;

  if (!GUID_RE.test(token)) {
    notFound();
  }

  let transaction: PaymentTransaction;
  try {
    transaction = await fetchTransaction(token);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) {
      notFound();
    }
    return (
      <main className="flex flex-1 flex-col items-center justify-center px-6 py-10">
        <TransactionErrorCard token={token} />
      </main>
    );
  }

  return (
    <main className="flex flex-1 flex-col items-center justify-center px-6 py-10">
      <PaymentActions token={token} initial={transaction} />
    </main>
  );
}
