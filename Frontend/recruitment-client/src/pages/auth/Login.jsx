import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../context/AuthContext";


function Login() {

  const navigate = useNavigate();

  const { login } = useContext(AuthContext);


  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");


  const handleSubmit = (e) => {

    e.preventDefault();


    // temporary fake user
    const fakeUser = {
      name: "Test Candidate",
      role: "Candidate"
    };


    const fakeToken = "sample-jwt-token";


    login(fakeUser, fakeToken);


    navigate("/candidate/dashboard");

  };


  return (
    <div className="min-h-screen flex items-center justify-center">

      <div className="p-8 shadow-lg rounded-xl w-96">

        <h1 className="text-2xl font-bold mb-6">
          Login
        </h1>


        <form onSubmit={handleSubmit}>

          <input
            type="email"
            placeholder="Email"
            className="border p-2 w-full mb-4"
            value={email}
            onChange={(e)=>setEmail(e.target.value)}
          />


          <input
            type="password"
            placeholder="Password"
            className="border p-2 w-full mb-4"
            value={password}
            onChange={(e)=>setPassword(e.target.value)}
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