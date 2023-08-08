<script lang="ts">
  import { usernameStore } from "../store"
  import {
    Button,
    Flex,
    Space,
    Title,
    Text,
    Accordion,
  } from "@svelteuidev/core"

  export let note: Note
  export let onEdit: (note: Note) => void
  export let onDelete: (note: Note) => void
</script>

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
      <Button variant="outline" on:click={() => onEdit(note)}>Edit note</Button>
      <Space w="sm" />
      <Button color="red" variant="outline" on:click={() => onDelete(note)}
        >Delete note</Button
      >
    </Flex>
  {/if}
</Accordion.Item>
