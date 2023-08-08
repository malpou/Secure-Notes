<script lang="ts">
  import "./global.css"
  import { Router, Link, Route } from "svelte-routing"
  import Notes from "./pages/Notes.svelte"
  import About from "./pages/About.svelte"
  import Login from "./pages/Login.svelte"
  import { usernameStore, userToken } from "./store"
  import {
    SvelteUIProvider,
    Container,
    Button,
    Flex,
    Space,
  } from "@svelteuidev/core"
  export let url = "/"

  let token = getCookie("jwtToken")
  let username = getCookie("username")

  if (token && username) {
    userToken.set(token)
    usernameStore.set(username)
  }

  function logout() {
    userToken.set(null)
    usernameStore.set(null)
    deleteCookie("jwtToken")
    deleteCookie("username")
  }

  function getCookie(name: string) {
    const value = "; " + document.cookie
    const parts = value.split("; " + name + "=")
    if (parts.length === 2) return parts.pop().split(";").shift()
  }

  function deleteCookie(name: string) {
    document.cookie = name + "=; Max-Age=-99999999;"
  }
</script>

<SvelteUIProvider withGlobalStyles>
  <Router {url}>
    <Container>
      <Flex justify="right">
        <Button><Link to="/">Notes</Link></Button>
        <Space w="sm" />
        <Button><Link to="/about">About</Link></Button>
        <Space w="sm" />
        {#if $usernameStore}
          <Button on:click={logout}>Logout</Button>
        {:else}
          <Button><Link to="/login">Login</Link></Button>
        {/if}
      </Flex>
    </Container>
    <Space h="md" />
    <Container>
      <Route path="/login" component={Login} />
      <Route path="/about" component={About} />
      <Route path="/"><Notes /></Route>
    </Container>
  </Router>
</SvelteUIProvider>
