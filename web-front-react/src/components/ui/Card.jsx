// src/components/ui/Card.jsx
import clsx from "clsx";

export default function Card({
  title,
  subtitle,
  children,
  footer,
  variant = "default", // "default" | "outline" | "flat"
  className = "",
}) {
  const variants = {
    default: "bg-white shadow-md border border-gray-200",
    outline: "bg-white border border-gray-300",
    flat: "bg-gray-50 border border-gray-100",
  };

  return (
    <div className={clsx("rounded-xl p-5 transition-all", variants[variant], className)}>
      {title && (
        <div className="mb-3 border-b pb-2">
          <h2 className="text-xl font-semibold text-gray-800">{title}</h2>
          {subtitle && <p className="text-gray-500 text-sm">{subtitle}</p>}
        </div>
      )}

      <div className="text-gray-700">{children}</div>

      {footer && (
        <div className="mt-4 border-t pt-2 text-sm text-gray-600">
          {footer}
        </div>
      )}
    </div>
  );
}
