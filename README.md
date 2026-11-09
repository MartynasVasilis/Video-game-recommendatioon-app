# GamePicker – žaidimų rekomendacijų sistema

Kursinio darbo II etapo prototipas. Pagal filtrus ir svertus rekomendacijų variklis grąžina surikiuotą žaidimų sąrašą su taškų paaiškinimu.

## Paleidimas

Reikalinga: .NET SDK 10, Node.js 22, veikiantis Docker Desktop.

```
npm install
npm run setup
npm start
```

- Sąsaja: http://localhost:5173
- API: http://localhost:5080

Migracijos ir pavyzdiniai duomenys pritaikomi paleidus API. Pradiniai duomenys atstatomi su `docker compose down -v`. Duomenų bazės ryšys nurodytas `src/GamePicker.Api/appsettings.json`.

## Testai

```
npm test
```

Prieš testus `npm start` turi būti sustabdytas, nes veikianti programa užrakina failus. Docker turi veikti.

- `RecommendationEngineTests` – variklio taisyklės.
- `RecommendationApiTests` – pagrindinis scenarijus ir klaidos per visą sistemą.
- `ClientIsolationTests` – klientų duomenų atskyrimas su tikra PostgreSQL.

## Pavyzdiniai duomenys

Aprašyti `src/GamePicker.Infrastructure/SampleData.cs`. Klientai yra naudotojai:

| Klientas (id) | Biblioteka |
| --- | --- |
| Aistė (1) | G4 – žaidžiau |
| Mantas (2) | G3 – nenoriu |
| Rokas (3) | G1 – žaidžiau, G7 – turiu |

G1–G7 paimti iš projektavimo dokumento, o jame nenurodyti laukai užpildyti pavyzdinėmis reikšmėmis. G8–G20 yra išgalvoti ir parinkti taip, kad dokumento scenarijų rezultatai nepasikeistų.

## Demonstravimas

Sąsajos formos numatytosios reikšmės atitinka dokumento scenarijų.

| Scenarijus | Veiksmai | Rezultatas |
| --- | --- | --- |
| Pagrindinis | Aistė, „Rekomenduoti“ | G1 – 5 tšk., G7 – 3 tšk., G3 – 2 tšk. |
| Lygūs taškai | Trukmė 20, N = 2 | G1, G7 |
| Nėra rezultatų | Trukmė 6 | Pranešimas „Rekomendacijų pagal duotus filtrus nerasta.“ |
| Pagalbinio filtro šalinimas | Trukmė 12, Pixel Forge, 2D | Rezultatai su pranešimu apie pašalintus filtrus |
| Klaida | Kaina nuo 50, iki 10 | Patikrinimo klaida |
| Biblioteka | Rokas, „Išskirti žaistus ir turimus“ | G1 ir G7 nerodomi |
| Multiplayer | Režimas „Multiplayer“, 8 žaidėjai, tuščia trukmė | G17, G12 |
| Atskyrimas sąsajoje | Aistei G2 pažymimas „Turiu“, perjungiama į Roką | Roko bibliotekoje tik G1 ir G7 |

Atskyrimas API lygiu, apeinant sąsają (PowerShell, `X-User` nurodo klientą):

```
# Savo biblioteka: 200
curl.exe -i -H "X-User: 1" http://localhost:5080/api/users/1/library

# Kito kliento skaitymas: 404
curl.exe -i -H "X-User: 1" http://localhost:5080/api/users/3/library

# Kito kliento keitimas: 404
curl.exe -i -X DELETE -H "X-User: 1" http://localhost:5080/api/users/3/library/1

# Roko biblioteka nepakito: 200
curl.exe -i -H "X-User: 3" http://localhost:5080/api/users/3/library

# Be kliento: 400
curl.exe -i http://localhost:5080/api/users/3/library
```

## Struktūra

| Dalis | Atsakomybė |
| --- | --- |
| `src/GamePicker.Domain` | Rekomendacijų variklis, nepriklauso nuo kitų sluoksnių. |
| `src/GamePicker.Application` | Naudojimo scenarijai ir saugyklų sąsajos. |
| `src/GamePicker.Infrastructure` | EF Core, migracijos, saugyklos, pavyzdiniai duomenys. |
| `src/GamePicker.Api` | Minimal API, kliento nustatymas, klaidų atsakymai. |
| `frontend/` | React sąsaja. |
| `tests/GamePicker.Tests` | xUnit testai. |

## Klientų atskyrimas

Sistema skirta asmeniniam naudojimui, todėl klientas yra naudotojas. Kliento duomenys yra biblioteka (`LibraryEntry.UserId`), o žaidimų katalogas bendras.

- `CurrentUserEndpointFilter` nustato klientą iš antraštės `X-User` ir atmeta nežinomą (400).
- `GamePickerDbContext` EF Core filtru riboja užklausas dabartiniu klientu, todėl kito kliento duomenys nerandami (404).
- `SaveChangesAsync` atmeta kito kliento įrašo pakeitimą.
- Filtras netaikomas tik vardų sąrašui `/api/users`, kuris šiame etape atstoja prisijungimą.

## Ribojimai

- Prisijungimas numatytas III etape, iki tol antrašte `X-User` pasitikima.
- Importo iš išorinio API nėra, katalogą sudaro pavyzdiniai duomenys.
- Svertai neišsaugomi.
