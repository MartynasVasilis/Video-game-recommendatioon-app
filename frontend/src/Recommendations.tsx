import { useEffect, useState, type FormEvent } from "react";
import {
  api,
  type FilterOptions,
  type RecommendationRequest,
  type RecommendationResult,
} from "./api";
import { Info } from "./Info";

const initialForm = {
  platform: "PC",
  maxDurationHours: "12",
  minPrice: "",
  maxPrice: "",
  mode: "Singleplayer",
  playerCount: "",
  genre: "",
  releaseYearFrom: "",
  releaseYearTo: "",
  excludeOwnedAndPlayed: false,
  developer: "",
  dimension: "",
  count: "3",
  maxPerGenre: "2",
};

const initialWeights: Record<string, string> = {
  Roguelike: "3",
  Atmosferiškas: "2",
};
const weightValues = [0, 1, 2, 3, 4, 5];
const noOptions: FilterOptions = {
  platforms: [],
  genres: [],
  developers: [],
  dimensions: [],
  releaseYears: [],
};

const text = (value: string) => value || undefined;
const number = (value: string) => (value === "" ? undefined : Number(value));

export function Recommendations({ userId }: { userId: number }) {
  const [form, setForm] = useState(initialForm);
  const [options, setOptions] = useState(noOptions);
  const [attributes, setAttributes] = useState<string[]>([]);
  const [weights, setWeights] = useState(initialWeights);
  const [result, setResult] = useState<RecommendationResult | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    Promise.all([api.filterOptions(userId), api.attributes(userId)])
      .then(([loadedOptions, { tags, moods }]) => {
        setOptions(loadedOptions);
        setAttributes([...tags, ...moods]);
      })
      .catch((e: Error) => setError(e.message));
  }, [userId]);

  function buildRequest(): RecommendationRequest {
    const selectedWeights: Record<string, number> = {};
    for (const [name, value] of Object.entries(weights)) {
      if (value !== "") {
        selectedWeights[name] = Number(value);
      }
    }

    return {
      hard: {
        platform: text(form.platform),
        maxDurationHours: number(form.maxDurationHours),
        minPrice: number(form.minPrice),
        maxPrice: number(form.maxPrice),
        mode: text(form.mode),
        playerCount:
          form.mode === "Multiplayer" ? number(form.playerCount) : undefined,
        genre: text(form.genre),
        releaseYearFrom: number(form.releaseYearFrom),
        releaseYearTo: number(form.releaseYearTo),
        excludeOwnedAndPlayed: form.excludeOwnedAndPlayed,
      },
      auxiliary: {
        developer: text(form.developer),
        dimension: text(form.dimension),
      },
      weights: selectedWeights,
      count: number(form.count),
      maxPerGenre: number(form.maxPerGenre),
    };
  }

  async function submit(event: FormEvent) {
    event.preventDefault();
    setError("");
    setResult(null);
    try {
      setResult(await api.recommend(userId, buildRequest()));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  const numberField = (
    label: string,
    name: keyof typeof initialForm,
    min: number,
    info?: string,
  ) => (
    <label>
      <span>
        {label}
        {info && <Info text={info} />}
      </span>
      <input
        type="number"
        min={min}
        value={form[name] as string}
        onChange={(e) => setForm({ ...form, [name]: e.target.value })}
      />
    </label>
  );

  const selectField = (
    label: string,
    name: keyof typeof initialForm,
    values: (string | number)[],
  ) => (
    <label>
      {label}
      <select
        value={form[name] as string}
        onChange={(e) => setForm({ ...form, [name]: e.target.value })}
      >
        <option value="">Bet koks</option>
        {values.map((value) => (
          <option key={value} value={value}>
            {value}
          </option>
        ))}
      </select>
    </label>
  );

  return (
    <section>
      <h2>Rekomendacijos</h2>
      <form onSubmit={submit}>
        <fieldset>
          <legend>Pagrindiniai filtrai</legend>
          {selectField("Platforma", "platform", options.platforms)}
          {numberField("Maks. trukmė (val.)", "maxDurationHours", 1)}
          {numberField("Kaina nuo", "minPrice", 0)}
          {numberField("Kaina iki", "maxPrice", 0)}
          {selectField("Režimas", "mode", ["Singleplayer", "Multiplayer"])}
          <label>
            Žaidėjų sk.
            <input
              type="number"
              min={2}
              disabled={form.mode !== "Multiplayer"}
              value={form.playerCount}
              onChange={(e) =>
                setForm({ ...form, playerCount: e.target.value })
              }
            />
          </label>
          {selectField("Žanras", "genre", options.genres)}
          {selectField("Metai nuo", "releaseYearFrom", options.releaseYears)}
          {selectField("Metai iki", "releaseYearTo", options.releaseYears)}
          <label className="checkbox">
            <input
              type="checkbox"
              checked={form.excludeOwnedAndPlayed}
              onChange={(e) =>
                setForm({ ...form, excludeOwnedAndPlayed: e.target.checked })
              }
            />
            Išskirti žaistus ir turimus
          </label>
        </fieldset>

        <fieldset>
          <legend>Pagalbiniai filtrai</legend>
          {selectField("Kūrėjas", "developer", options.developers)}
          {selectField("2D / 3D", "dimension", options.dimensions)}
        </fieldset>

        <fieldset>
          <legend>Svertai (0–5)</legend>
          {attributes.map((name) => (
            <label key={name}>
              {name}
              <select
                value={weights[name] ?? ""}
                onChange={(e) =>
                  setWeights({ ...weights, [name]: e.target.value })
                }
              >
                <option value="">–</option>
                {weightValues.map((value) => (
                  <option key={value} value={value}>
                    {value}
                  </option>
                ))}
              </select>
            </label>
          ))}
        </fieldset>

        <fieldset>
          <legend>Rezultatas</legend>
          {numberField(
            "Pasiūlymų sk. N",
            "count",
            1,
            "Kiek daugiausia žaidimų rodyti rezultatų sąraše.",
          )}
          {numberField(
            "Įvairovės riba K",
            "maxPerGenre",
            1,
            "Kiek daugiausia to paties žanro žaidimų gali būti sąraše. Pvz., kai K = 1, kiekvienas žanras rodomas tik vieną kartą. Tuščias laukas reiškia, kad ribos nėra.",
          )}
        </fieldset>

        <button type="submit">Rekomenduoti</button>
      </form>

      {error && <p className="error">{error}</p>}

      {result && (
        <div className="results">
          {result.removedFilters.length > 0 && (
            <p className="notice">
              Pašalinti pagalbiniai filtrai: {result.removedFilters.join(", ")}.
            </p>
          )}
          {result.message && <p className="notice">{result.message}</p>}
          <ol>
            {result.games.map((game) => {
              const explanation = game.breakdown
                .map((line) => `${line.label} +${line.points}`)
                .join(", ");
              return (
                <li
                  key={game.gameId}
                  title={explanation || "Nėra sutampančių žymių"}
                >
                  <strong>{game.title}</strong> – {game.score} tšk.
                  {explanation && ` (${explanation})`}
                  <small>
                    {" "}
                    {game.genre}, {game.durationHours} val.
                  </small>
                </li>
              );
            })}
          </ol>
        </div>
      )}
    </section>
  );
}
