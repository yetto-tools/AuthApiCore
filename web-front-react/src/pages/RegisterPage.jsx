import { useState } from "react";
import { useAuthStore } from "../store/useAuthStore";
import InputText from "../components/InputText";

export default function RegisterPage() {
  const [nombre, setNombre] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const { register, isLoading, error } = useAuthStore();

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      const res = await register(nombre, email, password);
      alert(`✅ Usuario registrado: ${res[0]?.Mensaje || "Éxito"}`);
    } catch {
      alert("❌ Error al registrar");
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <form
        onSubmit={handleRegister}
        className="bg-white p-8 rounded-xl shadow-md w-96"
      >
        <h2 className="text-2xl font-bold mb-6 text-center text-sky-700">Registro</h2>

        <InputText label="Nombre" value={nombre} onChange={setNombre} placeholder="Tu nombre completo" />
        <InputText label="Correo electrónico" type="email" value={email} onChange={setEmail} placeholder="correo@dominio.com" />
        <InputText label="Contraseña" type="password" value={password} onChange={setPassword} placeholder="••••••••" />

        {error && <p className="text-red-500 text-sm mb-4">{error}</p>}

        <button
          type="submit"
          disabled={isLoading}
          className="w-full bg-sky-600 hover:bg-sky-700 text-white py-2 rounded-lg transition"
        >
          {isLoading ? "Registrando..." : "Registrar"}
        </button>
      </form>
    </div>
  );
}
