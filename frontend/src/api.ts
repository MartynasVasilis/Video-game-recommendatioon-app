export type User = { id: number; name: string }
export type Game = { id: number; title: string; genre: string; platforms: string[]; durationHours: number }
export type Attributes = { tags: string[]; moods: string[] }
export type FilterOptions = {
  platforms: string[]
  genres: string[]
  developers: string[]
  dimensions: string[]
  releaseYears: number[]
}
export type LibraryStatus = 'Owned' | 'Played' | 'NotInterested'
export type LibraryEntry = { gameId: number; status: LibraryStatus }

export type RecommendedGame = {
  gameId: number
  title: string
  genre: string
  durationHours: number
  score: number
  breakdown: { label: string; points: number }[]
}

export type RecommendationResult = {
  games: RecommendedGame[]
  removedFilters: string[]
  message: string | null
}

export type RecommendationRequest = {
  hard: {
    platform?: string
    maxDurationHours?: number
    minPrice?: number
    maxPrice?: number
    mode?: string
    playerCount?: number
    genre?: string
    releaseYearFrom?: number
    releaseYearTo?: number
    excludeOwnedAndPlayed: boolean
  }
  auxiliary: { developer?: string; dimension?: string }
  weights: Record<string, number>
  count?: number
  maxPerGenre?: number
}

async function request<T>(userId: number, path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`/api${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', 'X-User': String(userId) },
  })

  if (!response.ok) {
    const problem = await response.json().catch(() => null)
    throw new Error(problem?.detail ?? 'Užklausa nepavyko.')
  }

  return response.status === 204 ? (undefined as T) : response.json()
}

export const api = {
  users: async (): Promise<User[]> => {
    const response = await fetch('/api/users')
    if (!response.ok) throw new Error('Nepavyko gauti naudotojų.')
    return response.json()
  },
  games: (userId: number) => request<Game[]>(userId, '/games'),
  attributes: (userId: number) => request<Attributes>(userId, '/attributes'),
  filterOptions: (userId: number) => request<FilterOptions>(userId, '/filter-options'),
  library: (userId: number) => request<LibraryEntry[]>(userId, `/users/${userId}/library`),
  setStatus: (userId: number, gameId: number, status: LibraryStatus) =>
    request<void>(userId, `/users/${userId}/library/${gameId}`, {
      method: 'PUT',
      body: JSON.stringify({ status }),
    }),
  removeStatus: (userId: number, gameId: number) =>
    request<void>(userId, `/users/${userId}/library/${gameId}`, { method: 'DELETE' }),
  recommend: (userId: number, body: RecommendationRequest) =>
    request<RecommendationResult>(userId, `/users/${userId}/recommendations`, {
      method: 'POST',
      body: JSON.stringify(body),
    }),
}
