export default function Home() {
  return (
    <main className="flex flex-1 flex-col items-center justify-center gap-4 px-6 text-center">
      <h1 className="text-2xl font-bold">درگاه پرداخت آزمایشی</h1>
      <p className="max-w-md text-sm leading-7 text-zinc-600">
        این صفحه، شبیه‌ساز درگاه پرداخت است. لینک پرداخت (Gateway URL) را سرویس
        backend می‌سازد؛ برای دیدن صفحه پرداخت، آدرس{" "}
        <code
          dir="ltr"
          className="rounded bg-zinc-100 px-1.5 py-0.5 font-mono text-[0.9em]"
        >
          /gateway/{`{token}`}
        </code>{" "}
        را در مرورگر باز کنید.
      </p>
    </main>
  );
}
