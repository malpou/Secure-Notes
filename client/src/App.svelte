<script lang="ts">
  import "./global.css"
  import { Router, Route, navigate } from "svelte-routing"
  import SecureNotes from "./pages/SecureNotes.svelte"
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
  import { onMount } from "svelte"

  export let url = "/"

  function logout() {
    userToken.set(null)
    usernameStore.set(null)
    deleteCookie("jwtToken")
    deleteCookie("username")
    navigate("/login")
  }

  function getCookie(name: string) {
    const value = "; " + document.cookie
    const parts = value.split("; " + name + "=")
    if (parts.length === 2) return parts.pop().split(";").shift()
  }

  onMount(() => {
    let token = getCookie("jwtToken")
    let username = getCookie("username")

    if (token && username) {
      userToken.set(token)
      usernameStore.set(username)
      if (window.location.pathname === "/login") {
        navigate("/")
      }
    } else if (window.location.pathname !== "/about") {
      navigate("/login")
    } else {
      navigate("/about")
    }
  })

  function deleteCookie(name: string) {
    document.cookie = name + "=; Max-Age=-99999999;"
  }
</script>

<SvelteUIProvider withGlobalStyles>
  <Router {url}>
    <Container>
      <Flex justify="right">
        {#if $usernameStore}
          <Button variant="outline" on:click={() => navigate("/")} ripple
            >Notes</Button
          >
          <Space w="sm" />
        {/if}
        <Button variant="outline" on:click={() => navigate("/about")} ripple
          >About</Button
        >
        <Space w="sm" />
        {#if $usernameStore}
          <Button color="red" variant="outline" on:click={logout} ripple
            >Logout</Button
          >
        {:else}
          <Button
            color="green"
            variant="outline"
            on:click={() => navigate("/login")}
            ripple>Login</Button
          >
        {/if}
      </Flex>
    </Container>
    <Space h="md" />
    <Container>
      {#if $userToken && $usernameStore}
        <Route path="/" component={SecureNotes} />
      {:else}
        <Route path="/login" component={Login} />
      {/if}
      <Route path="/about" component={About} />
    </Container>
  </Router>
</SvelteUIProvider>
