<script lang="ts">
  import * as api from '../api';
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

  async function saveNote(note: Note | null, title: string, content: string) {
    try {
      if (!note) {
        const newNote = await api.createNote(title, content, $userToken)
        notes = [convertTimeZones(newNote), ...notes]
      } else {
        const updatedNote = await api.updateNote(
          note.id,
          title,
          content,
          $userToken
        )

        const noteIndex = notes.findIndex((n) => n.id === updatedNote.id)
        if (noteIndex !== -1) {
          notes = [
            ...notes.slice(0, noteIndex),
            convertTimeZones(updatedNote),
            ...notes.slice(noteIndex + 1),
          ]
        }
      }
    } catch (error) {
      console.error("Error:", error.message)
    }

    closeModal()
  }

  async function deleteNote(note: Note) {
    try {
      await api.deleteNote(note.id, $userToken)
      notes = notes.filter((n) => n.id !== note.id)
    } catch (error) {
      console.error("Error deleting note:", error)
    }
  }

  async function fetchNotes() {
    isLoading = true
    try {
      const fetchedNotes = await api.fetchNotes(page, $userToken)
      notes = [...notes, ...fetchedNotes.map(convertTimeZones)]

      if (fetchedNotes.length < 5) {
        allNotesLoaded = true
      }
    } catch (error) {
      console.error("Error fetching notes:", error)
    } finally {
      isLoading = false
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
  <Title order={1}>Secret Notes</Title>
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
