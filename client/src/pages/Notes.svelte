<script lang="ts">
  import { Link } from "svelte-routing"
  import { userToken } from "../store"
  import { Button, Title, Text, Accordion, size, Space } from "@svelteuidev/core"

  let notes: Note[] = []
  let page = 1
  let allNotesLoaded = false
  let isLoading = false

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

<Title order={1}>Notes</Title>
<Space h="xl" />
<Accordion>
  {#each notes as note}
    <Accordion.Item value={note.id}>
      <Title slot="control" order={3}>{note.title}</Title>
      <Text>
        {note.content}
      </Text>     
      <Space  h="lg" />
      <Text size={"sm"}>
        Created At: {note.createdAt.toLocaleString()}
      </Text>
      <Text size={"sm"}>
        Updated At: {note.updatedAt.toLocaleString()}
      </Text>
      <Space  h="lg" />
      <Button><Link to={`/note/${note.id}`}>Edit note</Link></Button>
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
