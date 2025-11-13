// components/layout/Sidebar.jsx
import { Home, User, Settings, LogOut, Users } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import { useAuthStore } from "../../store/useAuthStore";

export default function Sidebar() {
  const navigate = useNavigate();
  const { logout } = useAuthStore();

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  const menu = [
    { icon: <Home size={20} />, label: "Dashboard", path: "/dashboard" },
    { icon: <User size={20} />, label: "Perfil", path: "/profile" },
    { icon: <Users size={20} />, label: "Usuarios", path: "/users" },
    { icon: <Settings size={20} />, label: "Configuración", path: "/settings" },
  ];

  return (
    <aside className="h-screen w-64 bg-white border-r shadow-sm flex flex-col justify-between">
      <div>
        <h2 className="text-xl font-semibold text-sky-700 p-4 border-b">Mi App</h2>
        <nav className="flex flex-col p-2">
          {menu.map((item, i) => (
            <Link
              key={i}
              to={item.path}
              className="flex items-center gap-3 px-4 py-2 text-gray-700 hover:bg-sky-100 rounded-lg transition-all"
            >
              {item.icon}
              <span>{item.label}</span>
            </Link>
          ))}
        </nav>
      </div>

      <button
        onClick={handleLogout}
        className="flex items-center gap-2 text-red-600 hover:bg-red-50 px-4 py-3 border-t"
      >
        <LogOut size={20} />
        Cerrar sesión
      </button>
    </aside>
  );
}
