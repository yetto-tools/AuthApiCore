import { useState, useMemo } from "react";
import { ChevronLeft, ChevronRight, ArrowUpDown } from "lucide-react";

export default function TableData({ data, columns, pageSize = 5 }) {
  const [search, setSearch] = useState("");
  const [sortConfig, setSortConfig] = useState({ key: null, direction: "asc" });
  const [page, setPage] = useState(1);

  // 🔹 Filtrado global
  const filteredData = useMemo(() => {
    
    return data.filter((row) =>
      Object.values(row)
        .join(" ")
        .toLowerCase()
        .includes(search.toLowerCase())
    );
  }, [data, search]);

  // 🔹 Ordenamiento
  const sortedData = useMemo(() => {
    if (!sortConfig.key) return filteredData;
    return [...filteredData].sort((a, b) => {
      const valA = a[sortConfig.key];
      const valB = b[sortConfig.key];
      if (valA < valB) return sortConfig.direction === "asc" ? -1 : 1;
      if (valA > valB) return sortConfig.direction === "asc" ? 1 : -1;
      return 0;
    });
  }, [filteredData, sortConfig]);

  // 🔹 Paginación
  const totalPages = Math.ceil(sortedData.length / pageSize);
  const paginatedData = sortedData.slice((page - 1) * pageSize, page * pageSize);

  const handleSort = (key) => {
    setSortConfig((prev) => {
      if (prev.key === key && prev.direction === "asc")
        return { key, direction: "desc" };
      return { key, direction: "asc" };
    });
  };

  return (
    <div className="bg-white shadow-lg rounded-xl p-4 border border-gray-200">
      {/* 🔍 Barra de búsqueda */}
      <div className="flex justify-between items-center mb-4">
        <input
          type="text"
          placeholder="Buscar..."
          className="border rounded-lg px-3 py-2 w-64 text-sm focus:outline-none focus:ring-2 focus:ring-sky-500"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <p className="text-sm text-gray-600">
          Mostrando {paginatedData.length} de {filteredData.length} resultados
        </p>
      </div>

      {/* 🧾 Tabla */}
      <div className="overflow-x-auto">
        <table className="min-w-full text-sm text-left border-collapse">
          <thead>
            <tr className="bg-gray-100 text-gray-700">
              {columns.map((col) => (
                <th
                  key={col.key}
                  onClick={() => handleSort(col.key)}
                  className="px-4 py-3 font-medium cursor-pointer select-none"
                >
                  <div className="flex items-center gap-1">
                    {col.label}
                    <ArrowUpDown
                      size={14}
                      className={
                        sortConfig.key === col.key
                          ? "text-sky-500"
                          : "text-gray-400"
                      }
                    />
                  </div>
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {paginatedData.length === 0 ? (
              <tr>
                <td
                  colSpan={columns.length}
                  className="text-center text-gray-500 py-4"
                >
                  No hay resultados.
                </td>
              </tr>
            ) : (
              paginatedData.map((row, idx) => (
                <tr
                  key={idx}
                  className="border-t hover:bg-gray-50 transition-colors"
                >
                  {columns.map((col) => (
                    <td key={col.key} className="px-4 py-2 text-gray-800">
                      {col.render ? col.render(row[col.key], row) : row[col.key]}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* 📄 Paginación */}
      <div className="flex items-center justify-between mt-4">
        <button
          className="flex items-center gap-1 px-3 py-1 border rounded-lg text-sm hover:bg-gray-100 disabled:opacity-50"
          onClick={() => setPage((p) => Math.max(p - 1, 1))}
          disabled={page === 1}
        >
          <ChevronLeft size={16} /> Anterior
        </button>
        <p className="text-sm text-gray-600">
          Página {page} de {totalPages}
        </p>
        <button
          className="flex items-center gap-1 px-3 py-1 border rounded-lg text-sm hover:bg-gray-100 disabled:opacity-50"
          onClick={() => setPage((p) => Math.min(p + 1, totalPages))}
          disabled={page === totalPages}
        >
          Siguiente <ChevronRight size={16} />
        </button>
      </div>
    </div>
  );
}
