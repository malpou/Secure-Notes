<script lang="ts">
  import { userToken, usernameStore } from "../store"
  import {
    Button,
    Title,
    Text,
    Accordion,
    Space,
    Modal,
    TextInput,
    Flex,
    Textarea,
  } from "@svelteuidev/core"

  let notes: Note[] = []
  let page = 1
  let allNotesLoaded = false
  let isLoading = false
  let opened = false

  let editTitle = ""
  let editContent = ""

  const openModal = (note: Note) => {
    opened = true
    editTitle = note.title
    editContent = note.content
  }

  const closeModal = () => {
    opened = false
    editTitle = ""
    editContent = ""
  }

  const saveNote = () => {
    closeModal()
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
  <Button>New Note</Button>
</Flex>
<Space h="xl" />
<Accordion>
  {#each notes as note}
    <Accordion.Item value={note.id}>
      <Title slot="control" order={3}>{note.title}</Title>
      <Text>
        {note.content}
      </Text>
      <Space h="lg" />
      <Text size={"sm"}>
        Created At: {note.createdAt.toLocaleString()}
      </Text>
      <Text size={"sm"}>
        Updated At: {note.updatedAt.toLocaleString()}
      </Text>
      {#if $usernameStore === note.author}
        <Flex justify="right">
          <Button on:click={() => openModal(note)}>Edit note</Button>
          <Space w="sm" />
          <Button>Delete note</Button>
        </Flex>
        <Modal {opened} on:close={closeModal} withCloseButton={false}>
          <Title order={3}>Editing: {note.title}</Title>
          <Space h="xl" />
          <Text weight="bold">Title</Text>
          <Space h="xs" />
          <TextInput bind:value={editTitle} />
          <Space h="md" />
          <Text weight="bold">Content</Text>
          <Space h="xs" />
          <Textarea bind:value={editContent} rows={6} />
          <Space h="md" />
          <Flex justify="right">
            <Button on:click={closeModal}>Close</Button>
            <Space w="sm" />
            <Button on:click={saveNote}>Save</Button>
          </Flex>
        </Modal>
      {/if}
    </Accordion.Item>
  {/each}
</Accordion>
<Space h="xl" />
<Button on:click={loadMore} disabled={isLoading || allNotesLoaded}
  >{allNotesLoaded
    ? "All notes are loaded."
    : isLoading
    ? "Loading..."
    : "Load More"}</Button
>
