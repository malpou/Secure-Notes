const BASE_URL = "https://api.secure-notes.net"

export async function loginUser(username: string, password: string) {
  const response = await fetch(`${BASE_URL}/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ username, password }),
  })
  if (!response.ok && response.status !== 401) {
    throw new Error("Error logging in")
  }
  return response
}

export async function registerUser(username: string, password: string) {
  const response = await fetch(`${BASE_URL}/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ username, password }),
  })
  if (!response.ok && response.status !== 409) {
    throw new Error("Error registering")
  }
  return response
}

export async function deleteAccount(token: string) {
  const response = await fetch(`${BASE_URL}/account`, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  })
  if (!response.ok) {
    throw new Error("Error deleting account")
  }
  return
}

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

  const response = await fetch(`${BASE_URL}/note`, {
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

  const response = await fetch(`${BASE_URL}/note/${noteId}`, {
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

  const response = await fetch(`${BASE_URL}/note/${noteId}`, {
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

  const response = await fetch(`${BASE_URL}/note?page=${page}`, { headers })

  if (!response.ok) {
    throw new Error("Network response was not ok")
  }

  return response.json()
}
