<script lang="ts">
    import { Router, Link, Route } from "svelte-routing";
    import Notes from "./pages/Notes.svelte";
    import About from "./pages/About.svelte";
    import Note from "./pages/Note.svelte";
    import Login from "./pages/Login.svelte";
    import {usernameStore, userToken} from "./store";

    export let url = "/";

    let token = getCookie('jwtToken');
    let username = getCookie('username');

    if (token && username) {
        userToken.set(token);
        usernameStore.set(username);
    }

    function logout() {
        userToken.set(null);
        usernameStore.set(null);
        deleteCookie('jwtToken');
        deleteCookie('username');
    }

    function getCookie(name: string) {
        const value = "; " + document.cookie;
        const parts = value.split("; " + name + "=");
        if (parts.length === 2) return parts.pop().split(";").shift();
    }

    function deleteCookie(name: string) {
        document.cookie = name + '=; Max-Age=-99999999;';
    }
</script>

<Router {url}>
    <nav>
        <Link to="/">Notes</Link>
        <Link to="/about">About</Link>
        {#if $usernameStore}
            <button on:click={logout}>Logout</button>
        {:else}
            <Link to="/login">Login</Link>
        {/if}
    </nav>
    <div>
        <Route path="/note/:id" let:params>
            <Note id={params.id} />
        </Route>
        <Route path="/login" component={Login} />
        <Route path="/about" component={About} />
        <Route path="/"><Notes /></Route>
    </div>
</Router>
