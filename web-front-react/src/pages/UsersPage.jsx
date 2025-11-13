import TableData from "../components/ui/TableData";
import { useEffect, useState } from "react";
import { api } from "../api/AuthApi";
import LayoutDashboard from "../components/layout/LayoutDashboard";
import { Edit } from "lucide-react";
import Modal from "../components/ui/Modal";

export default function UsersPage() {
  const [usuarios, setUsuarios] = useState([]);
  const [openModal, setOpenModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);



  useEffect(() => {
    api
      .get("/usuarios", { withCredentials: true })
      .then((res) => setUsuarios(res.data.users))
      .catch(() => setUsuarios([]));

      console.log(usuarios)
  }, []);


  const columns = [
    // { key: "id", label: "ID" },
    { key: "nombre", label: "Nombre" },
    { key: "email", label: "Correo" },
    { key: "rol", label: "Rol" },
    {
      key: "activo",
      label: "Estado",
      render: (value) => (
        <span
        className={`px-2 py-1 rounded text-xs font-semibold ${
          value ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"
        }`}
        >
          {value ? "Activo" : "Inactivo"}
        </span>
      ),
    },
    { key: "userRef", label: "Acciones" , render: (value) => (
      <button 
        className="bg-sky-600 px-2.5 text-white rounded py-1.5"  
        onClick={() => {handleEditRegistro(value); } }>

        <Edit size={20}/>
      </button>)
    },
  ];

 

  const handleEditRegistro = (userRef) => {
  
   usuarios.filter((user) => {
      if (user.userRef === userRef) {
        setSelectedUser(user);
      }
    });
    setOpenModal(true)
    
  }


  return (
    <LayoutDashboard>
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6 text-sky-700">
        Lista de Usuarios
      </h1>
      <TableData data={usuarios} columns={columns} pageSize={7} />
      <Modal
        isOpen={openModal}
        onClose={() => setOpenModal(false)}
        title="Detalles del usuario"
        onConfirm={() => {
          alert("Confirmado!");
          setOpenModal(false);
        }}
        confirmText="Guardar"
        cancelText="Cerrar"
        size="lg"
      >
      {selectedUser ? (
          <div className="space-y-3">
            <div>
              <label className="block text-sm text-gray-500">Nombre</label>
              <input
                type="text"
                value={selectedUser.nombre}
                className="w-full border rounded-lg px-3 py-2 text-gray-800"
                readOnly
              />
            </div>

            <div>
              <label className="block text-sm text-gray-500">Correo</label>
              <input
                type="text"
                value={selectedUser.email}
                className="w-full border rounded-lg px-3 py-2 text-gray-800"
                readOnly
              />
            </div>

          <div>
            <label className="block text-sm text-gray-500">Rol</label>
            <input
              type="text"
              value={selectedUser.rol}
              className="w-full border rounded-lg px-3 py-2 text-gray-800"
              readOnly
            />
          </div>

          <div>
            <label className="block text-sm text-gray-500">Estado</label>
            <span
              className={`inline-block px-2 py-1 text-xs font-semibold rounded ${
                selectedUser.activo
                  ? "bg-green-100 text-green-700"
                  : "bg-red-100 text-red-700"
              }`}
            >
              {selectedUser.activo ? "Activo" : "Inactivo"}
            </span>
          </div>
        </div>
        ) : (
          <p className="text-gray-500">Cargando datos...</p>
        )}
      </Modal>
    </div>
    </LayoutDashboard>
  );
}
