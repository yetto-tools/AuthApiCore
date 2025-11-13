import { useEffect, useState } from "react";
import { api } from "../api/AuthApi";
import LayoutDashboard from "../components/layout/LayoutDashboard";

export default function ProfilePage() {
  const [profile, setProfile] = useState(null);

  useEffect(() => {
    api
      .get("auth/perfil")
      .then((res) => setProfile(res.data))
      .catch(() => setProfile({ error: "Error al obtener perfil" }));
  }, []);

  return (
    <LayoutDashboard>
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white shadow-lg rounded-lg p-6 w-96 text-center">
        <h2 className="text-2xl font-bold text-sky-600 mb-4">Perfil</h2>
        {profile ? (
          <div>
            {profile.error ? (
              <p className="text-red-500">{profile.error}</p>
            ) : (
              <>
                <p><strong>Nombre:</strong> {profile.nombre}</p>
                <p><strong>Email:</strong> {profile.email}</p>
                <p><strong>Rol:</strong> {profile.rol}</p>
              </>
            )}
          </div>
        ) : (
          <p className="text-gray-500">Cargando...</p>
        )}
      </div>
    </div>
    </LayoutDashboard>
  );
}
