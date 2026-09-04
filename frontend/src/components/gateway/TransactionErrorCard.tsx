import Link from "next/link";

export default function TransactionErrorCard({ token }: { token: string }) {
  return (
    <div className="w-full max-w-md rounded-2xl border border-red-200 bg-red-50 p-6 text-center">
      <h1 className="text-lg font-bold text-red-800">خطا در دریافت تراکنش</h1>
      <p className="mt-2 text-sm leading-7 text-red-700">
        ارتباط با سرور برقرار نشد. لطفاً دوباره تلاش کنید.
      </p>
      <Link
        href={`/gateway/${token}`}
        className="mt-4 inline-block rounded-full bg-red-800 px-6 py-2 text-sm font-medium text-white"
      >
        تلاش مجدد
      </Link>
    </div>
  );
}
