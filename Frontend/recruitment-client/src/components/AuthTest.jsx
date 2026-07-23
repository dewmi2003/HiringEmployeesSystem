import { useContext } from "react";
import { AuthContext } from "../context/AuthContext";


function AuthTest() {

  const { user, token, login, logout } = useContext(AuthContext);


  const fakeLogin = () => {

    const fakeUser = {
      name: "Test Candidate",
      role: "Candidate"
    };

    const fakeToken = "abc123xyz";

    login(fakeUser, fakeToken);
  };


  return (
    <div>

      <h1>Auth Context Test</h1>


      <button onClick={fakeLogin}>
        Fake Login
      </button>


      <button onClick={logout}>
        Logout
      </button>


      <h2>User:</h2>

      <pre>
        {JSON.stringify(user, null, 2)}
      </pre>


      <h2>Token:</h2>

      <p>{token}</p>


    </div>
  );
}


export default AuthTest;