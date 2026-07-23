import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../context/AuthContext";
import { loginUser } from "../../services/authService";

function Login() {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");

    loginUser({ email, password })
      .then((result) => {
        const user = {
          email: result.email,
          fullName: result.fullName,
          role: result.role,
          expiresAt: result.expiresAt
        };

        login(user, result.token);

        if (result.role === "Admin") {
          navigate("/admin/dashboard");
        } else if (result.role === "Recruiter") {
          navigate("/recruiter/dashboard");
        } else {
          navigate("/candidate/dashboard");
        }
      })
      .catch((err) => {
        setError(
          err?.response?.data?.message ||
          "Login failed. Check your email and password."
        );
      });
  };

  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="p-8 shadow-lg rounded-xl w-96">
        <h1 className="text-2xl font-bold mb-6">
          Login
        </h1>

        <form onSubmit={handleSubmit}>
          {error ? (
            <p className="mb-4 text-sm text-red-600">
              {error}
            </p>
          ) : null}

          <input
            type="email"
            placeholder="Email"
            className="border p-2 w-full mb-4"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <input
            type="password"
            placeholder="Password"
            className="border p-2 w-full mb-4"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          <button
            type="submit"
            className="bg-blue-600 text-white p-2 w-full rounded"
          >
            Login
          </button>
        </form>
      </div>
    </div>
  );
}

export default Login;
