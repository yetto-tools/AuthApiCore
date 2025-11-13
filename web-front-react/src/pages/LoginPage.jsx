import { useState } from "react";
import { useAuthStore } from "../store/useAuthStore";
import InputText from "../components/InputText";
import { useNavigate  } from "react-router-dom";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, isLoading, error } = useAuthStore();
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();

    const user = await login(username, password);
    console.log(user);
    if (user && user.activo) {
      alert(`✅ Bienvenido ${user.nombre}`);
      navigate("/dashboard");
    } else {
      alert("❌ Error de autenticación");
    }
  };


  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <form
        onSubmit={handleLogin}
        className="bg-white p-8 rounded-xl shadow-md w-96"
      >
        <h2 className="text-2xl font-bold mb-6 text-center text-sky-700" onClick={()=>{navigate("/dashboard")}}>Iniciar sesión</h2>

        <InputText label="Correo electrónico" value={username} onChange={setUsername} placeholder="correo@dominio.com" required/>
        <InputText label="Contraseña" type="password" value={password} onChange={setPassword} placeholder="••••••••" required/>

        {error && <p className="text-red-500 text-sm mb-4">{error}</p>}

        <button
          type="submit"
          disabled={isLoading}
          className="w-full bg-sky-600 hover:bg-sky-700 text-white py-2 rounded-lg transition"
        >
          {isLoading ? "Iniciando..." : "Iniciar sesión"}
        </button>
      </form>
    </div>
  );
}
