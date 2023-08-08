<script lang="ts">
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
  <Flex justify="right">
    <Button variant="outline" on:click={() => onEdit(note)} ripple
      >Edit note</Button
    >
    <Space w="sm" />
    <Button color="red" variant="outline" on:click={() => onDelete(note)} ripple
      >Delete note</Button
    >
  </Flex>
</Accordion.Item>
