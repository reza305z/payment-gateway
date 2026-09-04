import type { ButtonHTMLAttributes } from "react";

type Variant = "primary" | "outline";

const VARIANT_CLASSES: Record<Variant, string> = {
  primary: "bg-emerald-600 text-white hover:bg-emerald-700",
  outline: "border border-zinc-300 bg-white text-zinc-700 hover:bg-zinc-50",
};

const BASE_CLASSES =
  "w-full rounded-full px-6 py-3 text-base font-bold transition disabled:cursor-not-allowed disabled:opacity-60";

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: Variant;
};

export default function Button({
  variant = "primary",
  className = "",
  type = "button",
  ...props
}: ButtonProps) {
  const classes = [BASE_CLASSES, VARIANT_CLASSES[variant], className]
    .filter(Boolean)
    .join(" ");
  return <button type={type} className={classes} {...props} />;
}
