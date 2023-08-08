const BASE_URL = "https://api.secure-notes.net/note"

export async function createNote(
  title: string,
  content: string,
  token?: string
) {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
  }

  if (token) {
    headers["Authorization"] = `Bearer ${token}`
  }

  const response = await fetch(BASE_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ title, content }),
  })

  if (!response.ok) {
    throw new Error("Failed to create note")
  }

  return response.json()
}

export async function updateNote(
  noteId: string,
  title: string,
  content: string,
  token?: string
) {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
  }

  if (token) {
    headers["Authorization"] = `Bearer ${token}`
  }

  const response = await fetch(`${BASE_URL}/${noteId}`, {
    method: "PUT",
    headers,
    body: JSON.stringify({ title, content }),
  })

  if (!response.ok) {
    throw new Error("Failed to update note")
  }

  return response.json()
}

export async function deleteNote(noteId: string, token?: string) {
  const headers: Record<string, string> = {}

  if (token) {
    headers["Authorization"] = `Bearer ${token}`
  }

  const response = await fetch(`${BASE_URL}/${noteId}`, {
    method: "DELETE",
    headers,
  })

  if (!response.ok) {
    throw new Error("Failed to delete note")
  }

  return
}

export async function fetchNotes(page: number, token?: string) {
  const headers: Record<string, string> = {}

  if (token) {
    headers["Authorization"] = `Bearer ${token}`
  }

  const response = await fetch(`${BASE_URL}?page=${page}`, { headers })

  if (!response.ok) {
    throw new Error("Network response was not ok")
  }

  return response.json()
}
