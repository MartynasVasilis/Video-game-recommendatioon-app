import { useEffect, useState } from "react";
import { api, type User } from "./api";
import { Library } from "./Library";
import { Recommendations } from "./Recommendations";

export default function App() {
  const [users, setUsers] = useState<User[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    api
      .users()
      .then((loaded) => {
        setUsers(loaded);
        setUserId(loaded[0]?.id ?? null);
      })
      .catch(() => setError("Nepavyko pasiekti API."));
  }, []);

  return (
    <main>
      <h1>GamePicker</h1>

      <div className="context">
        <label>
          <span>Naudotojas</span>
          <select
            value={userId ?? ""}
            onChange={(e) => setUserId(Number(e.target.value))}
          >
            {users.map((user) => (
              <option key={user.id} value={user.id}>
                {user.name}
              </option>
            ))}
          </select>
        </label>
      </div>

      {error && <p className="error">{error}</p>}

      {userId !== null && (
        <div key={userId} className="columns">
          <Recommendations userId={userId} />
          <Library userId={userId} />
        </div>
      )}
    </main>
  );
}
