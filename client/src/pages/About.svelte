<script>
  import { Title, Image, Text, Space, Flex } from "@svelteuidev/core"
  import keyHierarchyRoot from "../assets/key-hierarchy-root.png"
</script>

<Title order={1}>About Secure Notes</Title>

<Flex justify="center">
  <Image
    src={keyHierarchyRoot}
    alt="Envelope Encryption Illustration"
  />
</Flex>

<Text>
  <strong>Secure Notes</strong> is a application designed to ensure the utmost privacy
  and security for your notes. Leveraging state-of-the-art encryption methods, it
  ensures that every piece of data remains confidential and uncompromised.
</Text>

<Space h="md" />

<Title order={2}>Features</Title>

<ul>
  <li><Text>User-specific registration and login capabilities.</Text></li>
  <li>
    <Text
      >Every note comprises a title and description, encrypted before being
      stored.</Text
    >
  </li>
  <li>
    <Text
      >Utilizes envelope encryption, wherein each user is allocated a distinct
      master key. For every data piece, a new DEK (Data Encryption Key) is
      generated to encrypt the data. The master key is then employed to encrypt
      the DEK. The encrypted DEK, along with the encrypted data, is stored
      securely in the table.</Text
    >
  </li>
  <li><Text>All master keys are securely housed in Azure Key Vault.</Text></li>
</ul>

<Title order={2}>Technological Stack</Title>

<ul>
  <li>
    <Text
      ><strong>Azure Functions:</strong> Core framework for implementing envelope
      encryption, and other backend functionallity.</Text
    >
  </li>
  <li>
    <Text
      ><strong>Azure Key Vault:</strong> For secure storage of secrets and keys.</Text
    >
  </li>
  <li>
    <Text><strong>GitHub Actions:</strong> Enables automatic deployment.</Text>
  </li>
  <li>
    <Text
      ><strong>Terraform:</strong> Facilitates cloud provisioning in Azure.</Text
    >
  </li>
  <li><Text><strong>Azure Tables:</strong> Storage of all data.</Text></li>
  <li><Text><strong>JWT Tokens:</strong> Authentication method.</Text></li>
  <li>
    <Text
      ><strong>Svelte:</strong> The choice for frontend development, offering smooth
      user experience.</Text
    >
  </li>
</ul>

<Text>© 2023 Malthe Poulsen. All rights reserved.</Text>
