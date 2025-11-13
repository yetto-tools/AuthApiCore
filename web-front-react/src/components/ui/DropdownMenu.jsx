// components/layout/DropdownMenu.jsx
import { useState, useRef, useEffect } from "react";

export default function DropdownMenu({ label, items }) {
  const [open, setOpen] = useState(false);
  const ref = useRef();

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false);
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <div className="relative" ref={ref}>
      <button
        className="px-3 py-2 bg-gray-100 rounded-md hover:bg-gray-200"
        onClick={() => setOpen(!open)}
      >
        {label}
      </button>
      {open && (
        <ul className="absolute right-0 mt-2 bg-white shadow-lg border rounded-md w-48 z-50">
          {items.map((item, i) => (
            <li
              key={i}
              onClick={item.onClick}
              className="px-4 py-2 hover:bg-sky-50 cursor-pointer text-gray-700"
            >
              {item.label}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
