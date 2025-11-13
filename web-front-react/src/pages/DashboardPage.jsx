// import { useAuthStore } from "../store/useAuthStore";
// import { useNavigate, Link } from "react-router-dom";

// export default function DashboardPage() {
//   const { user, logout } = useAuthStore();
//   const navigate = useNavigate();

//   const handleLogout = () => {
//     logout();
//     navigate("/login");
//   };

//   return (
//     <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50">
//       <div className="bg-white shadow-lg rounded-lg p-6 text-center">
//         <h1 className="text-3xl font-bold mb-4 text-sky-700">Panel principal</h1>
//         <p className="text-gray-600 mb-4">
//           Bienvenido <span className="font-semibold">{user?.nombre || "Usuario"}</span>
//         </p>

//         <div className="flex gap-4 justify-center mb-4">
//           <Link to="/profile" className="text-sky-600 underline">
//             Ver perfil
//           </Link>
//           <button
//             onClick={handleLogout}
//             className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg"
//           >
//             Cerrar sesión
//           </button>
//         </div>
//       </div>
//     </div>
//   );
// }


// pages/DashboardPage.jsx
import LayoutDashboard from "../components/layout/LayoutDashboard";
import Drawer from "../components/ui/Drawer";
import DropdownMenu from "../components/ui/DropdownMenu";
import { useAuthStore } from "../store/useAuthStore";
import { useNavigate } from "react-router-dom";

export default function DashboardPage() {


  const { logout, isLoading, error } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = async () => {
    logout();
    alert("Cerrar sesión")
  };

  const handleConfiguraciones = async () => {
    navigate("/settings");
    alert("Configuraciones")
  }

  const handlePerfil = async () => {
    navigate("/profile");
    alert("Perfil")
  }


  return (
    <LayoutDashboard>
      <h1 className="text-2xl font-bold mb-4">Panel de control</h1>

      <div className="flex gap-4">
        <Drawer buttonLabel="Abrir Panel Derecho" side="right">
          <p>Contenido del panel lateral derecho.</p>
        </Drawer>

        <DropdownMenu
          label="Opciones"
          items={[
            { label: "Perfil", onClick: () =>  handlePerfil()   },
            { label: "Configuraciones", onClick: () => handleConfiguraciones() },
            { label: "Salir", onClick: () => handleLogout() },
          ]}
        />
      </div>
    </LayoutDashboard>
  );
}
