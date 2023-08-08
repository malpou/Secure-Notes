<script lang="ts">
  import * as api from "../api"
  import {
    Button,
    Flex,
    Space,
    TextInput,
    Title,
    Text,
  } from "@svelteuidev/core"
  import { usernameStore, userToken } from "../store"
  import { navigate } from "svelte-routing"

  let username = ""
  let password = ""
  let usernameError = ""
  let passwordError = ""

  async function handleSubmit(isLogin: boolean) {
    try {
      const response = isLogin
        ? await api.loginUser(username, password)
        : await api.registerUser(username, password)

      if (response.ok) {
        const data = await response.json()
        userToken.set(data.token)
        usernameStore.set(data.username)
        setCookie("jwtToken", data.token, 3)
        setCookie("username", data.username, 3)
        usernameError = ""
        passwordError = ""
        navigate("/")
      } else if (response.status === 401 && isLogin) {
        usernameError = ""
        passwordError = "Login failed. Please check your username and password."
      } else if (response.status === 409 && !isLogin) {
        usernameError =
          "Username is already in use. Please choose another username."
        passwordError = ""
      } else {
        usernameError = ""
        passwordError = "Error logging in/registering. Please try again."
      }
    } catch (error) {
      usernameError = ""
      passwordError = "An unexpected error occurred. Please try again."
    }
  }

  function setCookie(name: string, value: string, hours: number) {
    let expires = ""
    if (hours) {
      const date = new Date()
      date.setTime(date.getTime() + hours * 60 * 60 * 1000)
      expires = "; expires=" + date.toUTCString()
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/"
  }
</script>

<Title order={1}>Login/Register</Title>
<Space h="xl" />
<Text weight="bold">Username</Text>
<Space h="xs" />
<TextInput
  error={usernameError || passwordError.length > 0}
  bind:value={username}
/>
<Space h="md" />
<Text weight="bold">Password</Text>
<Space h="xs" />
<TextInput error={passwordError} bind:value={password} type="password" />
<Space h="xl" />
<Flex>
  <Button color="green" on:click={() => handleSubmit(true)}>Login</Button>
  <Space w="md" />
  <Button color="green" variant="light" on:click={() => handleSubmit(false)}
    >Register</Button
  >
</Flex>
