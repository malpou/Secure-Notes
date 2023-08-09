<script lang="ts">
  import {
    Modal,
    TextInput,
    Textarea,
    Button,
    Space,
    Flex,
    Title,
    Text,
  } from "@svelteuidev/core"
  import { createEventDispatcher } from "svelte"

  export let opened: boolean
  export let note: Note | null
  export let onClose: () => void
  export let onSave: (
    note: Note | null,
    title: string,
    content: string
  ) => Promise<void>

  const dispatch = createEventDispatcher()

  let title = ""
  let content = ""
  let previousNote: Note | null = null
  let loading = false

  const save = async (note: Note | null, title: string, content: string) => {
    loading = true
    await onSave(note, title, content)
    loading = false
  }

  $: dispatch("loadingChanged", loading)

  $: {
    if (opened) {
      if (note !== previousNote) {
        title = note?.title || ""
        content = note?.content || ""
        previousNote = note
      } else if (!note) {
        title = ""
        content = ""
      }
    }
  }
</script>

<Modal {opened} on:close={onClose} withCloseButton={false}>
  <Title order={3}>{note ? `Editing: ${note.title}` : "New Note"}</Title>
  <Space h="xl" />
  <Text weight="bold">Title</Text>
  <Space h="xs" />
  <TextInput bind:value={title} />
  <Space h="md" />
  <Text weight="bold">Content</Text>
  <Space h="xs" />
  <Textarea bind:value={content} rows={6} />
  <Space h="md" />
  <Flex justify="right">
    <Button
      variant="outline"
      color="gray"
      disabled={loading || title.length === 0 || content.length === 0}
      on:click={onClose}
      ripple>Close</Button
    >
    <Space w="sm" />
    <Button
      variant="outline"
      color="green"
      {loading}
      disabled={title.length === 0 || content.length === 0}
      on:click={() => save(note, title, content)}
      ripple
    >
      Save
    </Button>
  </Flex>
</Modal>
