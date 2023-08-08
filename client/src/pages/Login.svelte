<script lang="ts">
    import { writable } from 'svelte/store';
    import {usernameStore, userToken} from "../store";

    let username = '';
    let password = '';
    let errorMessage = ''; // Store the error message

    async function handleSubmit(isLogin: boolean) {
        const url = isLogin ? 'https://api.secure-notes.net/login' : 'https://api.secure-notes.net/register';
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });
            if (response.ok) {
                const data = await response.json();
                userToken.set(data.token);
                usernameStore.set(data.username);
                setCookie('jwtToken', data.token, 3);
                setCookie('username', data.username, 3);
                errorMessage = '';
            } else if (response.status === 401 && isLogin) {
                errorMessage = 'Login failed. Please check your username and password.';
            } else if (response.status === 409 && !isLogin) {
                errorMessage = 'Username is already in use. Please choose another username.';
            } else {
                errorMessage = 'Error logging in/registering. Please try again.';
            }
        } catch (error) {
            console.error("Error during fetch:", error);
            errorMessage = 'An unexpected error occurred. Please try again.';
        }
    }

    function setCookie(name: string, value: string, hours: number) {
        let expires = "";
        if (hours) {
            const date = new Date();
            date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

</script>

<h2>Login/Register</h2>
<form>
    {#if errorMessage}
        <p class="error">{errorMessage}</p>
    {/if}
    <div>
        <label>Username: <input bind:value={username} /></label>
    </div>
    <div>
        <label>Password: <input type="password" bind:value={password} /></label>
    </div>
    <div>
        <button type="button" on:click={() => handleSubmit(true)}>Login</button>
        <button type="button" on:click={() => handleSubmit(false)}>Register</button>
    </div>
</form>

<style>
    .error {
        color: red;
    }
</style>
