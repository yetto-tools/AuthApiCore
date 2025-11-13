// pages/SettingsPage.jsx
import LayoutDashboard from "../components/layout/LayoutDashboard";
import Button from "../components/ui/Button";
import Drawer from "../components/ui/Drawer";
import DropdownMenu from "../components/ui/DropdownMenu";
import TableData from "../components/ui/TableData";
import { useAuthStore } from "../store/useAuthStore";
import { useNavigate } from "react-router-dom";

export default function SettingsPage() {


  const { logout, isLoading, error } = useAuthStore();
  const navigate = useNavigate();


  return (
    <LayoutDashboard>
      <h1 className="text-2xl font-bold mb-4">Panel de control</h1>

      <div className="flex gap-4">

        <div>
            <div className="flex gap-3">
            <Button variant="primary">Guardar</Button>
            <Button variant="secondary">Cancelar</Button>
            <Button variant="danger" size="sm">Eliminar</Button>
            <Button variant="outline" loading>Procesando...</Button>
          </div>
        </div>
      </div>
      <div>
        
      </div>
    </LayoutDashboard>
  );
}
