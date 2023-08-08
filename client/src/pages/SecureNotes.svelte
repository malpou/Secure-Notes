<script lang="ts">
  import * as api from "../api"
  import EditNoteModal from "../components/EditNoteModal.svelte"
  import NoteComponent from "../components/NoteComponent.svelte"
  import DeleteConfirmationModal from "../components/DeleteConfirmationModal.svelte"
  import { userToken, usernameStore } from "../store"
  import { Button, Title, Accordion, Space, Flex } from "@svelteuidev/core"
  import { navigate } from "svelte-routing"

  let notes: Note[] = []
  let page = 1
  let allNotesLoaded = false
  let isLoading = false
  let opened = false
  let editingNote: Note | null = null
  let deleteConfirmationOpened = false
  let itemToDelete: "account" | Note | null = null
  let deleteConfirmationMessage: string = ""
  let modalLoading = false

  const closeModal = () => {
    if (!modalLoading) {
      opened = false
    }
  }

  const openModalForEditing = (note: Note) => {
    editingNote = note
    opened = true
  }

  const openModalForNewNote = () => {
    editingNote = null
    opened = true
  }

  const closeDeleteConfirmation = () => {
    if (!modalLoading) {
      deleteConfirmationOpened = false
      itemToDelete = null
    }
  }

  const openNoteDeleteConfirmation = (note: Note) => {
    itemToDelete = note
    deleteConfirmationOpened = true
  }

  const openAccountDeleteConfirmation = () => {
    itemToDelete = "account"
    deleteConfirmationMessage = "Are you sure you want to delete your account?"
    deleteConfirmationOpened = true
  }

  const confirmDelete = async (item?: Note | "account") => {
    if (!item || (typeof item === "string" && item === "account")) {
      await deleteAccount()
      modalLoading = false
      closeDeleteConfirmation()
      return
    }

    await deleteNote(item)
    modalLoading = false
    closeDeleteConfirmation()
  }

  const saveNote = async (
    note: Note | null,
    title: string,
    content: string
  ) => {
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

    modalLoading = false
    closeModal()
  }

  const fetchNotes = async () => {
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

  const deleteNote = async (note: Note) => {
    try {
      await api.deleteNote(note.id, $userToken)
      notes = notes.filter((n) => n.id !== note.id)
    } catch (error) {
      console.error("Error deleting note:", error)
    }
  }

  const deleteAccount = async () => {
    try {
      await api.deleteAccount($userToken)
      $userToken = null
      $usernameStore = null
      navigate("/login")
    } catch (error) {
      console.error("Error deleting account:", error)
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
  <Title order={1}>Secure notes for <i>{$usernameStore}</i></Title>
  {#if $usernameStore !== null}
    <Flex justify="right">
      <Button variant="outline" on:click={openModalForNewNote} ripple
        >New Note</Button
      >
      <Space w="sm" />
      <Button
        variant="outline"
        color="red"
        on:click={openAccountDeleteConfirmation}
        ripple>Delete Account</Button
      >
    </Flex>
  {/if}
</Flex>
<Space h="xl" />
<Accordion>
  {#each notes as note}
    <NoteComponent
      {note}
      onEdit={openModalForEditing}
      onDelete={openNoteDeleteConfirmation}
    />
  {/each}
</Accordion>
<Space h="xl" />
<Button
  variant="outline"
  on:click={loadMore}
  disabled={isLoading || allNotesLoaded}
  ripple
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
  on:loadingChanged={(e) => (modalLoading = e.detail)}
/>

<DeleteConfirmationModal
  opened={deleteConfirmationOpened}
  message={deleteConfirmationMessage}
  item={itemToDelete}
  onClose={closeDeleteConfirmation}
  onConfirmDelete={confirmDelete}
  on:loadingChanged={(e) => (modalLoading = e.detail)}
/>
