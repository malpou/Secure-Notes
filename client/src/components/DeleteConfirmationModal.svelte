<script lang="ts">
  import { Modal, Button, Space, Flex, Title, Text } from "@svelteuidev/core"
  import { createEventDispatcher } from "svelte"

  export let opened: boolean
  export let item: "account" | Note | null
  export let message: string = ""
  export let onClose: () => void
  export let onConfirmDelete: (item?: Note | "account") => Promise<void>

  const dispatch = createEventDispatcher()

  let loading = false

  const confirmDelete = async (item?: Note | "account") => {
    loading = true
    await onConfirmDelete(item)
    loading = false
  }

  $: dispatch("loadingChanged", loading)
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
    <Button
      variant="outline"
      color="gray"
      disabled={loading}
      on:click={onClose}
      ripple>Cancel</Button
    >
    <Space w="sm" />
    <Button
      variant="outline"
      color="red"
      {loading}
      on:click={() => confirmDelete(item)}
      ripple>Confirm</Button
    >
  </Flex>
</Modal>
