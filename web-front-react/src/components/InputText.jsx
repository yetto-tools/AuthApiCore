export default function InputText({ label, type = "text", value, onChange, placeholder , className, required }) {
  return (
    <div className="flex flex-col gap-1 mb-4">
      <label className="text-gray-700 font-medium">{label}</label>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="border rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-sky-600"
        required={required|| false}
      />
    </div>
  );
}
