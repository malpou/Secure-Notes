<script lang="ts">
  import { onMount } from "svelte"
  import { Link } from "svelte-routing"
  import { userToken } from "../store"

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
    isLoading = true;
    const fetchedNotes = await fetchData<Note[]>(
      `https://api.secure-notes.net/note?page=${page}`
    );
    isLoading = false;

    notes = [...notes, ...fetchedNotes.map(convertTimeZones)];

    if (fetchedNotes.length < 5) {
      allNotesLoaded = true;
      return;
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

<h1>Notes</h1>
<ul>
  {#each notes as note}
    <li>
      <h2><Link to={`/note/${note.id}`}>{note.title}</Link></h2>
      <p>{note.content}</p>
      <small>Created At: {note.createdAt.toLocaleString()}</small>
      <br />
      <small>Updated At: {note.updatedAt.toLocaleString()}</small>
    </li>
  {/each}
</ul>
{#if allNotesLoaded}
  <p>All notes are loaded.</p>
{:else if isLoading}
  <p>Loading...</p>
{:else}
  <button on:click={loadMore} disabled={isLoading}>Load More</button>
{/if}
