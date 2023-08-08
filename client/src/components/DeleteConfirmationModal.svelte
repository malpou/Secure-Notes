<script lang="ts">
  import { Modal, Button, Space, Flex, Title, Text } from "@svelteuidev/core"

  export let opened: boolean
  export let item: "account" | Note | null
  export let message: string = ""
  export let onClose: () => void
  export let onConfirmDelete: (item?: Note | "account") => void
</script>

<Modal {opened} on:close={onClose} withCloseButton={false}>
  <Title order={3}>Confirm Deletion</Title>
  <Space h="xl" />
  {#if item === "account"}
    <Text>{message}</Text>
  {:else}
    <Text>Are you sure you want to delete the note titled "{item?.title}"?</Text
    >
  {/if}
  <Space h="md" />
  <Flex justify="right">
    <Button variant="outline" color="gray" on:click={onClose}>Cancel</Button>
    <Space w="sm" />
    <Button variant="outline" color="red" on:click={() => onConfirmDelete(item)}
      >Confirm</Button
    >
  </Flex>
</Modal>
