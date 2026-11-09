import { useEffect, useState } from "react";
import { api, type Game, type LibraryStatus } from "./api";

const statusLabels: Record<LibraryStatus, string> = {
  Owned: "Turiu",
  Played: "Žaidžiau",
  NotInterested: "Nenoriu",
};

export function Library({ userId }: { userId: number }) {
  const [games, setGames] = useState<Game[]>([]);
  const [statuses, setStatuses] = useState<Record<number, LibraryStatus>>({});
  const [error, setError] = useState("");

  useEffect(() => {
    Promise.all([api.games(userId), api.library(userId)])
      .then(([loadedGames, entries]) => {
        setGames(loadedGames);
        setStatuses(
          Object.fromEntries(
            entries.map((entry) => [entry.gameId, entry.status]),
          ),
        );
      })
      .catch((e: Error) => setError(e.message));
  }, [userId]);

  async function changeStatus(gameId: number, value: string) {
    setError("");
    try {
      if (value) {
        await api.setStatus(userId, gameId, value as LibraryStatus);
        setStatuses({ ...statuses, [gameId]: value as LibraryStatus });
      } else {
        await api.removeStatus(userId, gameId);
        const { [gameId]: _removed, ...rest } = statuses;
        setStatuses(rest);
      }
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <section>
      <h2>Katalogas ir biblioteka</h2>
      {error && <p className="error">{error}</p>}
      <table>
        <thead>
          <tr>
            <th>Žaidimas</th>
            <th>Žanras</th>
            <th>Platforma</th>
            <th>Val.</th>
            <th>Būsena</th>
          </tr>
        </thead>
        <tbody>
          {games.map((game) => (
            <tr key={game.id}>
              <td>{game.title}</td>
              <td>{game.genre}</td>
              <td>{game.platforms.join(", ")}</td>
              <td>{game.durationHours}</td>
              <td>
                <select
                  value={statuses[game.id] ?? ""}
                  onChange={(e) => changeStatus(game.id, e.target.value)}
                >
                  <option value="">–</option>
                  {Object.entries(statusLabels).map(([status, label]) => (
                    <option key={status} value={status}>
                      {label}
                    </option>
                  ))}
                </select>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
