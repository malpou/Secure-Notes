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

  export let opened: boolean
  export let note: Note | null
  export let onClose: () => void
  export let onSave: (note: Note | null, title: string, content: string) => void
  
  let title = ""
  let content = ""
  let previousNote: Note | null = null;

  $: {
    if (opened && note !== previousNote) {
      title = note?.title || "";
      content = note?.content || "";
      previousNote = note;
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
    <Button variant="outline" color="gray" on:click={onClose}>Close</Button>
    <Space w="sm" />
    <Button
      variant="outline"
      color="green"
      on:click={() => onSave(note, title, content)}>Save</Button
    >
  </Flex>
</Modal>
