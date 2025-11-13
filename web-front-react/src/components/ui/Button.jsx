// src/components/ui/Button.jsx
import clsx from "clsx";

export default function Button({
  children,
  variant = "primary",
  size = "md",
  loading = false,
  disabled = false,
  onClick,
  type = "button",
  className = "",
}) {
  const base = "rounded-lg font-medium transition-all focus:outline-none focus:ring-2";

  const variants = {
    primary: "bg-sky-600 text-white hover:bg-sky-700 focus:ring-sky-400",
    secondary: "bg-gray-200 text-gray-800 hover:bg-gray-300 focus:ring-gray-400",
    danger: "bg-red-600 text-white hover:bg-red-700 focus:ring-red-400",
    outline:
      "border border-gray-400 text-gray-700 hover:bg-gray-100 focus:ring-gray-300",
    ghost: "text-gray-700 hover:bg-gray-100 focus:ring-gray-300",
  };

  const sizes = {
    sm: "text-sm px-3 py-1.5",
    md: "text-base px-4 py-2",
    lg: "text-lg px-6 py-3",
  };

  const classes = clsx(
    base,
    variants[variant],
    sizes[size],
    "disabled:opacity-50 disabled:cursor-not-allowed",
    className
  );

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled || loading}
      className={classes}
    >
      {loading ? (
        <span className="flex items-center justify-center gap-2">
          <span className="h-4 w-4 border-2 border-t-transparent border-white animate-spin rounded-full" />
          Cargando...
        </span>
      ) : (
        children
      )}
    </button>
  );
}
