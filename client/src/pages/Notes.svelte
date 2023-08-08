<script lang="ts">
  import EditNoteModal from "../components/EditNoteModal.svelte"
  import NoteComponent from "../components/NoteComponent.svelte"
  import { userToken, usernameStore } from "../store"
  import { Button, Title, Accordion, Space, Flex } from "@svelteuidev/core"

  let notes: Note[] = []
  let page = 1
  let allNotesLoaded = false
  let isLoading = false
  let opened = false
  let editingNote: Note | null = null

  const closeModal = () => {
    opened = false
  }

  const openModalForEditing = (note: Note) => {
    editingNote = note
    opened = true
  }

  const openModalForNewNote = () => {
    editingNote = null
    opened = true
  }

  const saveNote = async (
    note: Note | null,
    title: string,
    content: string
  ) => {
    if (!note) {
      try {
        const headers: Record<string, string> = {}

        if ($userToken !== null) {
          headers["Authorization"] = `Bearer ${$userToken}`
        }

        const newNoteData = {
          title,
          content,
        }

        const response = await fetch(`https://api.secure-notes.net/note`, {
          method: "POST",
          headers: {
            ...headers,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(newNoteData),
        })

        if (!response.ok) {
          throw new Error("Failed to create note")
        }

        const newNote = await response.json()

        notes = [convertTimeZones(newNote), ...notes]
      } catch (error) {
        console.error("Error creating note:", error)
      }
    } else {
      try {
        const headers: Record<string, string> = {}

        if ($userToken !== null) {
          headers["Authorization"] = `Bearer ${$userToken}`
        }

        const updatedNoteData = {
          title: title,
          content: content,
        }

        const response = await fetch(
          `https://api.secure-notes.net/note/${note.id}`,
          {
            method: "PUT",
            headers: {
              ...headers,
              "Content-Type": "application/json",
            },
            body: JSON.stringify(updatedNoteData),
          }
        )

        if (!response.ok) {
          throw new Error("Failed to update note")
        }

        const updatedNote = await response.json()

        const noteIndex = notes.findIndex((note) => note.id === updatedNote.id)
        if (noteIndex !== -1) {
          notes = [
            ...notes.slice(0, noteIndex),
            convertTimeZones(updatedNote),
            ...notes.slice(noteIndex + 1),
          ]
        }
      } catch (error) {
        console.error("Error updating note:", error)
      }
    }

    closeModal()
  }

  const deleteNote = async (note: Note) => {
    try {
      const headers: Record<string, string> = {}

      if ($userToken !== null) {
        headers["Authorization"] = `Bearer ${$userToken}`
      }

      const response = await fetch(
        `https://api.secure-notes.net/note/${note.id}`,
        {
          method: "DELETE",
          headers,
        }
      )

      if (!response.ok) {
        throw new Error("Failed to delete note")
      }

      notes = notes.filter((n) => n.id !== note.id)
    } catch (error) {
      console.error("Error deleting note:", error)
    }
  }

  async function fetchData<T>(url: string): Promise<T> {
    const headers: Record<string, string> = {}

    if ($userToken !== null) {
      headers["Authorization"] = `Bearer ${$userToken}`
    }

    const response = await fetch(url, { headers })

    if (!response.ok) {
      throw new Error("Network response was not ok")
    }

    return response.json()
  }

  async function fetchNotes() {
    isLoading = true
    const fetchedNotes = await fetchData<Note[]>(
      `https://api.secure-notes.net/note?page=${page}`
    )
    isLoading = false

    notes = [...notes, ...fetchedNotes.map(convertTimeZones)]

    if (fetchedNotes.length < 5) {
      allNotesLoaded = true
      return
    }
  }

  function convertTimeZones(note: Note): Note {
    return {
      ...note,
      createdAt: new Date(note.createdAt),
      updatedAt: new Date(note.updatedAt),
    }
  }

  function loadMore() {
    page++
    fetchNotes()
  }

  $: {
    resetNotes()
    fetchNotes()
  }

  function resetNotes() {
    notes = []
    page = 1
    allNotesLoaded = false
  }
</script>

<Flex justify="space-between">
  <Title order={1}>Notes</Title>
  {#if $usernameStore !== null}
    <Button variant="outline" on:click={openModalForNewNote}>New Note</Button>
  {/if}
</Flex>
<Space h="xl" />
<Accordion>
  {#each notes as note}
    <NoteComponent {note} onEdit={openModalForEditing} onDelete={deleteNote} />
  {/each}
</Accordion>
<Space h="xl" />
<Button
  variant="outline"
  on:click={loadMore}
  disabled={isLoading || allNotesLoaded}
>
  {allNotesLoaded
    ? "All notes are loaded."
    : isLoading
    ? "Loading..."
    : "Load More"}
</Button>

<EditNoteModal
  {opened}
  note={editingNote}
  onClose={closeModal}
  onSave={saveNote}
/>
