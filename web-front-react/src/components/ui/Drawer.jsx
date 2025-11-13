// components/layout/Drawer.jsx
import { useState } from "react";

export default function Drawer({ side = "right", buttonLabel = "Abrir", children }) {
  const [open, setOpen] = useState(false);
  const sideClasses = {
    right: "right-0 top-0 h-full w-72",
    left: "left-0 top-0 h-full w-72",
    top: "top-0 left-0 w-full h-64",
    bottom: "bottom-0 left-0 w-full h-64",
  };

  return (
    <>
      <button
        className="px-4 py-2 bg-sky-600 text-white rounded-md"
        onClick={() => setOpen(true)}
      >
        {buttonLabel}
      </button>

      {open && (
        <div
          className="fixed inset-0 bg-black bg-opacity-40 z-40"
          onClick={() => setOpen(false)}
        />
      )}

      <div
        className={`fixed bg-white shadow-xl z-50 p-4 transition-transform ${
          sideClasses[side]
        } ${open ? "translate-x-0" : side === "right" ? "translate-x-full" : "-translate-x-full"}`}
      >
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-bold">Panel</h3>
          <button onClick={() => setOpen(false)}>✖</button>
        </div>
        {children}
      </div>
    </>
  );
}
